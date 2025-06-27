using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : NetworkBehaviour
{
    private enum State
    {
        None,
        Idle,
        Patroling,
        Suspicion,
        Attacking,
        PlayerLost,
        Searching
    }

    [Header("Navigation")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private NavMeshAgent agent;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    [Header("Components")]
    [SerializeField] private PlayerDetector detector;
    [SerializeField] private EnemyRotation rotation;
    [SerializeField] private EnemyAttackAbility attackAbility;

    [Header("Settings")]
    [SerializeField] private float triggerTime = 2f;
    [SerializeField] private float detectionThreshold = 0.5f;
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private float minAttackDistance = 2f;

    private State _currentState;
    private int _currentPatrolIndex;

    private Player _player;
    private Vector3 _playerLastPos;

    public float triggerValue { get; private set; }
    public Player Player
    {
        get => _player;
    }

    private void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        animator.enabled = false;
        TransitionToState(State.Patroling);
        ContinuePatroling();
    }

    public void SetPatrolingPoints(Transform[] points)
    {
        patrolPoints = points;
    }

    private void Update()
    {
        HandleState();
    }

    private void HandleState()
    {
        switch (_currentState)
        {
            case State.Patroling:
                HandlePatrolling();
                break;
            case State.Suspicion:
                HandleSuspicion();
                break;
            case State.PlayerLost:
                HandlePlayerLost();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
            case State.Idle:
            case State.Searching:
                // waiting for animation "LookAround" to end
                break;
        }
    }

    private void HandlePatrolling()
    {
        var targetPoint = patrolPoints[_currentPatrolIndex].position;
        var distance = Vector3.Distance(transform.position, targetPoint);

        if (distance < stopDistance)
        {
            TransitionToState(State.Idle);
            animator.enabled = true;
            animator.SetTrigger("LookAround");
        }
    }

    // Look around and follow last known player position
    private void HandleSuspicion()
    {
        rotation.RotateTowards(_playerLastPos);
        var distance = Vector3.Distance(transform.position, _playerLastPos);

        if (triggerValue >= 1f)
        {
            TransitionToState(State.Attacking);
            attackAbility.SetAttackMode(true);
            agent.SetDestination(transform.position);
            return;
        }

        if (triggerValue > detectionThreshold)
            agent.SetDestination(_playerLastPos);

        if (detector.isAnyPlayerDetected)
        {
            if (distance < minAttackDistance)
                KeepDistanceFromPlayer(minAttackDistance);
        }
        else if (distance < stopDistance)
        {
            TransitionToState(State.Searching);
            animator.enabled = true;
            animator.SetTrigger("LookAround");
        }
        else if (triggerValue <= 0f)
        {
            TransitionToState(State.Patroling);
            ContinuePatroling();
        }
    }

    private void HandlePlayerLost()
    {
        rotation.RotateTowards(_playerLastPos);
        agent.SetDestination(_playerLastPos);

        var distance = Vector3.Distance(transform.position, _playerLastPos);
        if (distance < stopDistance)
        {
            TransitionToState(State.Searching);
            animator.enabled = true;
            animator.SetTrigger("LookAround");
        }
    }

    private void HandleAttacking()
    {
        KeepDistanceFromPlayer(minAttackDistance);
    }

    private void KeepDistanceFromPlayer(float distance)
    {
        var dir = (transform.position - _player.transform.position).normalized;
        var targetPos = _player.transform.position + dir * distance;
        agent.SetDestination(targetPos);
    }

    private void ContinuePatroling()
    {
        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
    }

    private void TransitionToState(State newState)
    {
        _currentState = newState;

        ChangeStateClientRpc(newState);
    }

    [ClientRpc]
    private void ChangeStateClientRpc(State newState)
    {
        _currentState = newState;
    }

    // detectionValue from low value to 1f means how good enemy sees player
    // if value is low, then enemy need to see player for couple seconds to get triggered
    // if value is max (1f) it means player is close and enemy gets trigered faster
    public void OnPlayerDetected(Player player, float detectionValue, float deltaTime)
    {
        _player = player;
        _playerLastPos = player.transform.position;

        triggerValue = Mathf.Clamp01(triggerValue + deltaTime / triggerTime * detectionValue); ;

        if (_currentState == State.PlayerLost)
        {
            TransitionToState(State.Attacking);
            attackAbility.SetAttackMode(true);
        }

        if (_currentState == State.Patroling)
        {
            TransitionToState(State.Suspicion);
            agent.SetDestination(transform.position);
        }

        UpdateTriggerValueClientRpc(triggerValue);
    }

    [ClientRpc]
    private void UpdateTriggerValueClientRpc(float value)
    {
        triggerValue = value;
    }

    public void OnPlayerLost(float deltaTime)
    {
        if (_currentState == State.Attacking)
        {
            TransitionToState(State.PlayerLost);
            attackAbility.SetAttackMode(false);
            agent.SetDestination(_playerLastPos);
            return;
        }

        if (triggerValue > detectionThreshold) return;

        triggerValue = Mathf.Max(0f, triggerValue - deltaTime);
        UpdateTriggerValueClientRpc(triggerValue);
    }

    private void OnLookAnimationEnd()
    {
        if (_currentState == State.Idle || _currentState == State.Searching)
        {
            TransitionToState(State.Patroling);
            animator.enabled = false;
            ContinuePatroling();
            triggerValue = 0f;
            UpdateTriggerValueClientRpc(triggerValue);
        }
    }
}
