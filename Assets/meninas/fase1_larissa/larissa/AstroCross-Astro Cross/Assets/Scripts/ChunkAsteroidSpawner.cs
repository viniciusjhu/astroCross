using System.Collections.Generic;
using UnityEngine;

public class ChunkAsteroidSpawner : MonoBehaviour
{
    [SerializeField] private AsteroidMover asteroidPrefab;
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField, Range(0f, 1f)] private float spawnChancePerPoint = 0.5f;
    [SerializeField] private bool spawnOnEnable = true;

    private readonly List<AsteroidMover> spawnedAsteroids = new();

    // Spawna os asteroides ao ativar o objeto se configurado.
    private void OnEnable()
    {
        if (spawnOnEnable)
        {
            SpawnAsteroids();
        }
    }

    // Limpa os asteroides gerados ao desativar o objeto.
    private void OnDisable()
    {
        ClearSpawned();
    }

    // Gera asteroides nos pontos de spawn baseados na chance configurada.
    public void SpawnAsteroids()
    {
        ClearSpawned();

        if (asteroidPrefab == null)
        {
            Debug.LogWarning($"{nameof(ChunkAsteroidSpawner)} precisa do prefab do asteroide.", this);
            return;
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning($"{nameof(ChunkAsteroidSpawner)} não possui pontos de spawn configurados.", this);
            return;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point == null)
            {
                continue;
            }

            if (Random.value > spawnChancePerPoint)
            {
                continue;
            }

            AsteroidMover asteroid = Instantiate(asteroidPrefab, point.position, point.rotation, transform);
            spawnedAsteroids.Add(asteroid);
        }
    }

    // Destrói todos os asteroides spawnados anteriormente.
    private void ClearSpawned()
    {
        for (int i = spawnedAsteroids.Count - 1; i >= 0; i--)
        {
            if (spawnedAsteroids[i] != null)
            {
                Destroy(spawnedAsteroids[i].gameObject);
            }
        }

        spawnedAsteroids.Clear();
    }
}