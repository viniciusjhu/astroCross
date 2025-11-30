using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] public Transform playerTransform;

    [Header("Follow Settings")]
    [SerializeField] private float followDistanceZ = 40f;
    [SerializeField] private float followOffsetX = 10f;
    [SerializeField] private float spawnHeight = 12f;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 3f;

    [Header("Asteroid Specifics")]
    [SerializeField] private float threatSpawnRangeX = 7f;
    [SerializeField] private float threatSpawnRangeY = 3f;
    [SerializeField] private float threatSpawnRangeZ = 4f;
    [Range(0, 1)] [SerializeField] private float threatChance = 0.7f;
    [SerializeField] private float ambientSpawnRangeX = 25f;
    [SerializeField] private float ambientSpawnRangeY = 5f;
    [SerializeField] private float ambientSpawnRangeZ = 10f;

    [Header("Difficulty Scaling")]
    [SerializeField] private float timeToIncreaseDifficulty = 15f;
    [SerializeField] private float speedMultiplierIncrease = 0.1f;
    [SerializeField] private float maxSpeedMultiplier = 3f;
    private float currentSpeedMultiplier = 1f;

    // Inicia as rotinas de spawn e aumento de dificuldade.
    void Start()
    {
        if (asteroidPrefab == null) Debug.LogError("ObstacleSpawner: Prefab do Asteroide não atribuído!");
        if (playerTransform == null) Debug.LogError("ObstacleSpawner: Transform do Jogador não atribuído!");

        if (playerTransform != null && asteroidPrefab != null)
        {
            StartCoroutine(SpawnObstaclesRoutine());
            StartCoroutine(IncreaseDifficultyRoutine());
        }
    }
    
    // Corrotina que spawna obstáculos em intervalos aleatórios.
    IEnumerator SpawnObstaclesRoutine()
    {
        while (true)
        {
            float randomWaitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(randomWaitTime);

            UpdateSpawnerPosition();
            SpawnAsteroid();
        }
    }

    // Mantém o spawner seguindo o jogador a uma distância fixa.
    void UpdateSpawnerPosition()
    {
        if (playerTransform != null)
        {
            float randomXOffset = Random.Range(-followOffsetX, followOffsetX);
            Vector3 newPosition = new Vector3(
                playerTransform.position.x + randomXOffset,
                playerTransform.position.y + spawnHeight,
                playerTransform.position.z + followDistanceZ
            );
            transform.position = newPosition;
        }
    }

    // Aumenta progressivamente a velocidade dos obstáculos.
    IEnumerator IncreaseDifficultyRoutine()
    {
        while (currentSpeedMultiplier < maxSpeedMultiplier)
        {
            yield return new WaitForSeconds(timeToIncreaseDifficulty);
            currentSpeedMultiplier += speedMultiplierIncrease;
            currentSpeedMultiplier = Mathf.Min(currentSpeedMultiplier, maxSpeedMultiplier);
        }
    }

    // Instancia um asteroide e define se ele será uma ameaça ou ambiente.
    void SpawnAsteroid()
    {
        bool isThreat = Random.value < threatChance;
        float randomX, randomY, randomZ;

        if (isThreat)
        {
            randomX = Random.Range(-threatSpawnRangeX, threatSpawnRangeX);
            randomY = Random.Range(-threatSpawnRangeY, threatSpawnRangeY);
            randomZ = Random.Range(0f, threatSpawnRangeZ);
        }
        else
        {
            randomX = Random.Range(-ambientSpawnRangeX, ambientSpawnRangeX);
            randomY = Random.Range(-ambientSpawnRangeY, ambientSpawnRangeY);
            randomZ = Random.Range(0f, ambientSpawnRangeZ);
        }

        Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);
        Vector3 spawnPosition = transform.position + randomOffset;

        GameObject asteroidInstance = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        ObstacleController obstacleController = asteroidInstance.GetComponent<ObstacleController>();
        if (obstacleController != null)
        {
            if (isThreat)
            {
                obstacleController.Initialize(playerTransform, currentSpeedMultiplier);
            }
            else
            {
                obstacleController.InitializeAmbient(currentSpeedMultiplier);
            }
        }
    }
}