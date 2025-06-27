using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    private List<EnemyBehaviour> _enemies = new List<EnemyBehaviour>();
    [SerializeField] private Transform[] points;
    [SerializeField] private EnemyBehaviour enemyPrefab;
    [Space]
    [SerializeField] private float spawnTime = 3;
    [SerializeField] private int maxAmount = 3;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerReady;
    }

    private void OnServerReady()
    {
        StartCoroutine(EnemyCreation());
    }

    private void CreateEnemy()
    {
        // we need two random not same points
        // that is why we are creating list and then removing pointA
        var pointsList = new List<Transform>(points);
        var pointA = points[Random.Range(0, pointsList.Count)];

        pointsList.Remove(pointA);

        var pointB = points[Random.Range(0, pointsList.Count)];

        var enemy = Instantiate(enemyPrefab);

        enemy.transform.position = pointA.position;
        enemy.SetPatrolingPoints(new Transform[] { pointA, pointB });
        enemy.GetComponent<NetworkObject>().Spawn();

        _enemies.Add(enemy);
    }


    private IEnumerator EnemyCreation()
    {
        while (true)
        {
            if (_enemies.Count == maxAmount)
            {
                yield return new WaitForSeconds(spawnTime);
                continue;
            }

            yield return new WaitForSeconds(spawnTime);
            CreateEnemy();
        }
    }
}
