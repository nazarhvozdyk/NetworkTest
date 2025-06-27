using Unity.Netcode;
using UnityEngine;

public class PlayerDetector : NetworkBehaviour
{
    [SerializeField] private FieldOfViewMesh fieldOfViewMesh;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private EnemyBehaviour enemyBehaviour;

    [Space()]

    [Range(0.1f, 0.8f)][SerializeField] private float minDetectionValue = 0.1f;
    [Range(0.6f, 1f)][SerializeField] private float maxDetectionValue = 1f;

    public bool isAnyPlayerDetected { get; private set; }

    private void Start()
    {
        enabled = IsServer;
    }

    private void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;

        foreach (var item in Player.players)
        {
            if (IsInFOV(item.transform))
            {
                enemyBehaviour.OnPlayerDetected(item, GetTriggerValue(item), deltaTime);
                isAnyPlayerDetected = true;
                return;
            }
        }

        isAnyPlayerDetected = false;
        enemyBehaviour.OnPlayerLost(deltaTime);
    }

    private float GetTriggerValue(Player player)
    {
        var distance = Vector3.Distance(transform.position, player.transform.position);
        var t = Mathf.Clamp01(distance / fieldOfViewMesh.viewRadius);
        var value = Mathf.Lerp(maxDetectionValue, minDetectionValue, t);

        return value;
    }

    public bool IsInFOV(Transform target)
    {
        // ignore Y position
        var targetPos = target.position;
        targetPos.y = 0;

        var startPos = transform.position;
        startPos.y = 0;

        Vector3 dirToTarget = (targetPos - startPos).normalized;
        float distanceToTarget = Vector3.Distance(startPos, targetPos);

        // check distance
        if (distanceToTarget > fieldOfViewMesh.viewRadius)
            return false;

        // check angle
        float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
        if (angleToTarget > fieldOfViewMesh.viewAngle / 2)
            return false;

        // check physical obstacles
        if (Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
            return false;

        return true;
    }

}
