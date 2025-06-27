using Unity.Netcode;
using UnityEngine;

public class EnemyAttackAbility : MonoBehaviour
{
    [SerializeField] private Transform bulletSpawnTransform;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private EnemyBehaviour enemyBehaviour;
    [SerializeField] private float reloadTime = 3f;

    private float _timer;

    private void Start()
    {
        enabled = false;
    }

    public void SetAttackMode(bool value)
    {
        enabled = value;
    }

    private void Update()
    {
        HandleShooting();
    }

    private void HandleShooting()
    {
        _timer += Time.deltaTime;

        if (_timer < reloadTime)
            return;

        _timer = 0;
        Shoot();
    }


    private void Shoot()
    {
        var direction = enemyBehaviour.Player.transform.position - bulletSpawnTransform.position;
        direction.Normalize();
        var newBullet = Instantiate(bulletPrefab);
        newBullet.GetComponent<NetworkObject>().Spawn();
        newBullet.transform.position = bulletSpawnTransform.position;
        newBullet.SetUp(direction);
    }
}
