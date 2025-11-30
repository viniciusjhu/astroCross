using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] chunkPrefabs;
    [SerializeField] private GameObject portalChunkPrefab;
    [SerializeField] private int startingChunksAmount = 12;
    [SerializeField] private Transform chunkParent;
    [SerializeField] private float chunkLenght = 10f;
    [SerializeField] private float moveSpeed = 8f;
    private List<GameObject> chunks = new List<GameObject>();
    private bool shouldSpawnPortal = false;

    public float FurthestChunkZ { get; private set; }

    // Sinaliza que o próximo chunk a ser gerado deve ser o portal.
    public void RequestPortalSpawn()
    {
        shouldSpawnPortal = true;
    }

    // Verifica os prefabs e inicia a geração dos chunks iniciais.
    void Start()
    {
        if (chunkPrefabs == null || chunkPrefabs.Length == 0)
        {
            Debug.LogError("Nenhum prefab de chunk atribuído ao LevelGenerator.");
            return;
        }
        SpawnStartingChunks();
    }

    // Atualiza o movimento dos chunks a cada passo de física.
    void FixedUpdate()
    {
        MoveChunks();
    }

    // Gera a quantidade inicial de chunks definida.
    void SpawnStartingChunks()
    {
        for (int i = 0; i < startingChunksAmount; i++)
        {
            SpawnChunks();
        }
    }

    // Instancia um novo chunk (normal ou portal) e o adiciona à lista.
    void SpawnChunks()
    {
        GameObject prefabToSpawn;

        if (shouldSpawnPortal && portalChunkPrefab != null)
        {
            prefabToSpawn = portalChunkPrefab;
            shouldSpawnPortal = false;
        }
        else
        {
            int randomIndex = Random.Range(0, chunkPrefabs.Length);
            prefabToSpawn = chunkPrefabs[randomIndex];
        }

        float spawnPositionZ = CalculateSpawnPositionZ();

        Vector3 chunkSpawnPos = new Vector3(transform.position.x, transform.position.y, spawnPositionZ);
        GameObject newChunk = Instantiate(prefabToSpawn, chunkSpawnPos, Quaternion.identity, chunkParent);

        chunks.Add(newChunk);
        FurthestChunkZ = newChunk.transform.position.z;
    }

    // Calcula a posição Z onde o próximo chunk deve ser instanciado.
    float CalculateSpawnPositionZ()
        {
            float spawnPositionZ;

            if (chunks.Count == 0)
            {
                spawnPositionZ = transform.position.z;
            }
            else
            {
                spawnPositionZ = chunks[chunks.Count - 1].transform.position.z + chunkLenght;
                
            }

            return spawnPositionZ;
        }
    
    // Move os chunks, destrói os que saíram da tela e gera novos.
    void MoveChunks()
    {
        for (int i = chunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = chunks[i];
            Rigidbody rb = chunk.GetComponent<Rigidbody>();

            Vector3 newPosition = rb.position + (-transform.forward * (moveSpeed * Time.fixedDeltaTime));
            
            rb.MovePosition(newPosition);

            if(chunk.transform.position.z <= Camera.main.transform.position.z - chunkLenght)
            {
                if (chunk.GetComponentInChildren<Portal>() != null)
                {
                    GameManager.Instance.PortalMissed();
                }

                chunks.RemoveAt(i);
                Destroy(chunk);
                SpawnChunks();
            }
        }
    }
}