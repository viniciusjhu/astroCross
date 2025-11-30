using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratedLarissa : MonoBehaviour
{
    [SerializeField] GameObject chunkPrefab;
    [SerializeField] int startingChunksAmount = 8;
    [SerializeField] Transform chunkParent;
    [SerializeField] float chunkLength = 5f;
    [SerializeField] float destroyOffset = 10f;

    [Header("Asteroid Spawning")]
    [SerializeField] GameObject asteroidPrefab;
    [SerializeField, Range(0f, 1f)] float asteroidSpawnChance = 0.4f;
    [SerializeField] float asteroidHeight = 2.5f;
    [SerializeField] Vector2 asteroidXRange = new Vector2(-0.4f, 0.4f);
    
    [Header("Portal Spawning")]
    [SerializeField] GameObject portalChunkPrefab;
    private bool spawnPortalNext = false;

    List<GameObject> activeChunks = new List<GameObject>();
    float nextSpawnZ = 0f;
    Transform cameraTransform;
    float lastCameraZ = 0f;
    int chunkCounter = 0;

    // Inicializa referências da câmera.
    void Awake()
    {
        Debug.Log("=== LEVEL GENERATOR INICIADO ===");
        
        if (chunkPrefab == null)
        {
            Debug.LogError("CHUNK PREFAB NÃO ATRIBUÍDO!");
            return;
        }

        cameraTransform = Camera.main.transform;
        lastCameraZ = cameraTransform.position.z;
        Debug.Log($"Câmera encontrada. Posição Z inicial: {lastCameraZ}");
    }

    // Gera os chunks iniciais do nível.
    void Start()
    {
        Debug.Log($"Criando {startingChunksAmount} chunks iniciais...");
        
        for (int i = 0; i < startingChunksAmount; i++)
        {
            SpawnChunk();
        }
        
        Debug.Log($"Chunks iniciais criados: {activeChunks.Count}");
    }

    // Monitora a posição da câmera para gerar novos chunks e destruir antigos.
    void Update()
    {
        if (cameraTransform == null) return;

        float currentCameraZ = cameraTransform.position.z;
        
        if (Mathf.Abs(currentCameraZ - lastCameraZ) > 0.1f)
        {
            Debug.Log($"📷 Câmera se moveu: {lastCameraZ} → {currentCameraZ}");

            float spawnThreshold = currentCameraZ + (startingChunksAmount * chunkLength * 0.5f);
            
            while (nextSpawnZ <= spawnThreshold)
            {
                GameObject newChunk = SpawnChunk();
                Debug.Log($"✅ NOVO CHUNK CRIADO em Z: {newChunk.transform.position.z}");
            }

            DestroyOldChunks(currentCameraZ);

            lastCameraZ = currentCameraZ;
        }
    }

    // Instancia um novo chunk, possivelmente com portal ou asteroides.
    GameObject SpawnChunk()
    {
        Vector3 spawnPos = new Vector3(0f, 0f, nextSpawnZ);
        GameObject prefabToUse = chunkPrefab;
        bool isPortalChunk = false;

        if (spawnPortalNext && portalChunkPrefab != null)
        {
            Debug.Log("🌀 SPAWNING PORTAL CHUNK NA FASE 1!");
            prefabToUse = portalChunkPrefab;
            isPortalChunk = true;
            spawnPortalNext = false;
        }

        GameObject go = Instantiate(prefabToUse, spawnPos, Quaternion.identity, chunkParent);
        activeChunks.Add(go);

        if (!isPortalChunk && chunkCounter > 0 && asteroidPrefab != null && Random.value < asteroidSpawnChance)
        {
            float randomX = Random.Range(asteroidXRange.x, asteroidXRange.y);
            Vector3 asteroidPos = new Vector3(randomX, asteroidHeight, spawnPos.z);
            GameObject asteroid = Instantiate(asteroidPrefab, asteroidPos, Quaternion.identity, go.transform);
            Debug.Log($"🌑 ASTEROIDE SPAWNADO em X:{randomX:F2} Z:{spawnPos.z}");
        }  
        chunkCounter++;
        nextSpawnZ += chunkLength;
        return go;
    }

    // Substitui um chunk existente por um chunk de portal.
    public void RequestPortalSpawn()
    {
        Debug.Log("LevelGeneratedLarissa: Requisição de Portal recebida! Inserindo portal na meia distância...");
        spawnPortalNext = true; 

        if (activeChunks.Count > 4 && portalChunkPrefab != null)
        {
            int safeIndex = Mathf.Clamp(activeChunks.Count - 4, 2, activeChunks.Count - 1);

            GameObject targetChunk = activeChunks[safeIndex];
            Vector3 pos = targetChunk.transform.position;

            activeChunks.RemoveAt(safeIndex);
            Destroy(targetChunk);

            GameObject portalChunk = Instantiate(portalChunkPrefab, pos, Quaternion.identity, chunkParent);
            activeChunks.Insert(safeIndex, portalChunk);

            Debug.Log($"🌀 Portal Spawnado no índice {safeIndex} (Distância média).");
        }
    }

    // Remove chunks que ficaram muito atrás da câmera.
    void DestroyOldChunks(float cameraZ)
    {
        if (activeChunks.Count == 0) return;

        bool destroyedAny = false;
        
        while (activeChunks.Count > 0)
        {
            GameObject oldestChunk = activeChunks[0];
            if (oldestChunk == null)
            {
                activeChunks.RemoveAt(0);
                continue;
            }

            float chunkStartZ = oldestChunk.transform.position.z;
            float chunkEndZ = chunkStartZ + chunkLength;
            
            if (chunkEndZ < cameraZ - destroyOffset)
            {
                GameObject toDestroy = activeChunks[0];
                activeChunks.RemoveAt(0);
                Destroy(toDestroy);
                Debug.Log($"🗑️ CHUNK DESTRUÍDO em Z: {toDestroy.transform.position.z}");
                destroyedAny = true;
            }
            else
            {
                break;
            }
        }

        if (!destroyedAny)
        {
            // Debug.Log("Nenhum chunk para destruir ainda");
        }
    }

    // Desenha gizmos de debug para visualizar áreas de spawn e câmera.
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0, 0, nextSpawnZ), new Vector3(10, 1, 1));
        
        Gizmos.color = Color.green;
        if (cameraTransform != null)
        {
            Gizmos.DrawWireCube(new Vector3(0, 2, cameraTransform.position.z), new Vector3(8, 1, 1));
        }
    }
}