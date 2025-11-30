using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawnerFlavia : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnChance = 1f;
    
    [Header("References")]
    [SerializeField] private Transform playerTransform;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeedX = 5f;
    [SerializeField] private float obstacleMovementRange = 8f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    private float spawnTimer = 0f;
    private List<GameObject> groundTiles = new List<GameObject>();
    private int spawnCount = 0;
    private GameObject lastSpawnedTile = null;
    
    // Inicializa o spawner e valida configurações.
    private void Start()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogError("❌ Nenhum prefab de obstáculo foi atribuído no array 'obstaclePrefabs'! O Spawner não pode funcionar.");
            return;
        }

        if (debugMode)
        {
            Debug.Log($"\n🎮 === OBSTACLE SPAWNER INICIADO === 🎮");
        }
    }
    
    // Controla o intervalo de tempo entre tentativas de spawn.
    private void Update()
    {
        if (groundTiles.Count == 0)
            return;
        
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnInterval)
        {
            TrySpawnObstacle();
            spawnTimer = 0f;
        }
    }
    
    // Tenta instanciar um obstáculo em um ground tile válido à frente do jogador.
    private void TrySpawnObstacle()
    {
        if (Random.value > spawnChance)
        {
            if (debugMode)
                Debug.Log($"⏭️ Spawn pulado pela chance. Count: {spawnCount}");
            return;
        }
        
        try
        {
            List<GameObject> validGroundTiles = new List<GameObject>();
            if (playerTransform != null)
            {
                foreach (GameObject tile in groundTiles)
                {
                    if (tile.transform.position.z > playerTransform.position.z - 20f)
                    {
                        validGroundTiles.Add(tile);
                    }
                }
            }
            else
            {
                validGroundTiles.AddRange(groundTiles);
                if (debugMode) Debug.LogWarning("ObstacleSpawner: playerTransform não atribuído! Spawning em todos os tiles disponíveis.");
            }

            if (lastSpawnedTile != null && validGroundTiles.Count > 1)
            {
                validGroundTiles.Remove(lastSpawnedTile);
            }

            if (validGroundTiles.Count == 0)
            {
                if (debugMode) Debug.Log($"⚠️ Nenhum ground tile válido encontrado à frente do jogador para fazer spawn.");
                return;
            }

            GameObject randomGround = validGroundTiles[Random.Range(0, validGroundTiles.Count)];
            
            GameObject prefabToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            GameObject newObstacleParent = Instantiate(prefabToSpawn, randomGround.transform.position, Quaternion.identity);
            
            newObstacleParent.name = $"Obstacle_{spawnCount + 1}_{prefabToSpawn.name}";
            
            if (!prefabToSpawn.CompareTag("PointObstacle"))
            {
                ApplyMovement(newObstacleParent, randomGround);
            }
            
            spawnCount++;
            lastSpawnedTile = randomGround;
            
            if (debugMode)
                Debug.Log($"✅ Spawn #{spawnCount}: [{prefabToSpawn.name}] em {randomGround.name} | Pos: {randomGround.transform.position}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Erro ao fazer spawn: {e.Message}");
        }
    }
    
    // Adiciona script de movimento ao obstáculo spawnado.
    private void ApplyMovement(GameObject obstacle, GameObject groundReference)
    {
        ObstacleMovement movementScript = obstacle.AddComponent<ObstacleMovement>();
        movementScript.Initialize(moveSpeedX, obstacleMovementRange);
        
        if (debugMode)
            Debug.Log($"   🏃 Movimento em loop ativado. Velocidade: {moveSpeedX}, Range: {obstacleMovementRange}");
    }
    
    // Registra um novo tile de chão como potencial local de spawn.
    public void RegisterGroundTile(GameObject groundTile)
    {
        if (!groundTiles.Contains(groundTile))
        {
            groundTiles.Add(groundTile);
            if (debugMode)
                Debug.Log($"📍 Ground tile '{groundTile.name}' registrado. Total: {groundTiles.Count}");
        }
    }
    
    // Remove um tile de chão da lista de locais de spawn.
    public void UnregisterGroundTile(GameObject groundTile)
    {
        if (groundTiles.Remove(groundTile))
        {
            if (debugMode)
                Debug.Log($"❌ Ground tile '{groundTile.name}' removido. Total: {groundTiles.Count}");
            
            if (groundTile == lastSpawnedTile)
            {
                lastSpawnedTile = null;
            }
        }
    }
    
    // Retorna o total de spawns realizados.
    public int GetSpawnCount()
    {
        return spawnCount;
    }
    
    // Imprime o status atual do spawner no console.
    public void LogStatus()
    {
        Debug.Log($"\n📊 === STATUS DO SPAWNER ===");
        Debug.Log($"   Total de spawns: {spawnCount}");
        Debug.Log($"   Ground tiles ativos: {groundTiles.Count}");
        Debug.Log($"   Spawn chance: {spawnChance * 100}%");
        Debug.Log($"   Move speed X: {moveSpeedX}");
        Debug.Log($"   =========================\n");
    }
}
