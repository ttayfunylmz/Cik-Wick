using UnityEngine;
using Zenject;
using System.Collections;

public class CounterTriggerController : MonoBehaviour
{
    [SerializeField] private GameObject[] _obstaclePrefabs;
    [SerializeField] private Transform _knifeSpawnPoint;
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private float _spawnOffsetRange = 2f;

    private bool _isSpawning = false;

    private DiContainer _container;

    [Inject]
    private void ZenjectSetup(DiContainer container)
    {
        _container = container;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out var controller))
        {
            if (!_isSpawning)
            {
                _isSpawning = true;
                StartCoroutine(SpawnKnifesCoroutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out var controller))
        {
            _isSpawning = false;
            StopCoroutine(SpawnKnifesCoroutine());
        }
    }

    private IEnumerator SpawnKnifesCoroutine()
    {
        while (_isSpawning)
        {
            SpawnKnifeWithOffset();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnKnifeWithOffset()
    {
        float randomOffsetX = Random.Range(-_spawnOffsetRange, _spawnOffsetRange);
        float randomOffsetZ = Random.Range(-_spawnOffsetRange, _spawnOffsetRange);

        Vector3 spawnPosition = new Vector3(
            _knifeSpawnPoint.position.x + randomOffsetX,
            _knifeSpawnPoint.position.y,
            _knifeSpawnPoint.position.z + randomOffsetZ
        );

        int randomIndex = Random.Range(0, _obstaclePrefabs.Length);
        _container.InstantiatePrefab(_obstaclePrefabs[randomIndex], spawnPosition, _knifeSpawnPoint.rotation, _knifeSpawnPoint);
    }
}
