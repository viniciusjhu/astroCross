using UnityEngine;
using System.Collections.Generic;

public class ScenarioManager : MonoBehaviour
{
    public GameObject inicioPrefab;
    public GameObject[] trechosPrefabs;
    public int preSpawnCount = 5;
    public Transform cameraTransform;

    [Tooltip("A que distância do final da pista um novo trecho deve ser gerado?")]
    [SerializeField] private float distanciaParaGerar = 50f;
    [SerializeField] private ObstacleSpawnerFlavia obstacleSpawner;
    [SerializeField] private bool debugMode = true;
    [SerializeField] private Transform playerTransform;
    private List<GameObject> activeTrechos = new List<GameObject>();
    private float nextSpawnZ = 0f;
    private bool ultimoFoiRio = false;

    // Inicializa o cenário, encontrando referências e spawnando os trechos iniciais.
    void Start()
    {
        if (obstacleSpawner == null)
        {
            obstacleSpawner = FindAnyObjectByType<ObstacleSpawnerFlavia>();
            if (obstacleSpawner == null)
            {
                Debug.LogError("ScenarioManager: ObstacleSpawnerFlavia não encontrado na cena!");
            }
        }
        
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if(mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                Debug.LogWarning("ScenarioManager: 'cameraTransform' não foi atribuído. Usando a câmera principal encontrada na cena.");
            }
            else
            {
                Debug.LogError("ScenarioManager: 'cameraTransform' não atribuído e nenhuma câmera principal foi encontrada! A geração de trechos não funcionará.");
                return;
            }
        }

        GameObject inicio = Instantiate(inicioPrefab, Vector3.zero, Quaternion.identity);
        activeTrechos.Add(inicio);
        
        if (obstacleSpawner != null && !inicio.name.Contains("rio"))
        {
            obstacleSpawner.RegisterGroundTile(inicio);
        }
        nextSpawnZ += GetTrechoLength(inicio);

        for (int i = 1; i < preSpawnCount; i++)
            SpawnTrecho();
    }

    // Monitora a posição da câmera para gerar novos trechos infinitamente.
    void Update()
    {
        if (debugMode && cameraTransform != null)
        {
            // float distanciaRestante = nextSpawnZ - cameraTransform.position.z;
        }

        if (cameraTransform != null && cameraTransform.position.z > nextSpawnZ - distanciaParaGerar)
        {
            SpawnTrecho();
            RemoveFirstTrecho();
        }
    }

    // Instancia um novo trecho de cenário, evitando repetição de rios.
    void SpawnTrecho()
    {
        int index = Random.Range(0, trechosPrefabs.Length);

        if (ultimoFoiRio && trechosPrefabs[index].name.Contains("rio"))
        {
            do
            {
                index = Random.Range(0, trechosPrefabs.Length);
            } while (trechosPrefabs[index].name.Contains("rio"));
        }

        bool isRio = trechosPrefabs[index].name.Contains("rio");

        Quaternion rotation = Quaternion.identity;
        if (trechosPrefabs[index].name.Contains("Vegetation"))
        {
            if (Random.value > 0.5f)
                rotation = Quaternion.Euler(0, 180, 0);
        }

        float yPosition = isRio ? 0.5f : 0f;

        yPosition = trechosPrefabs[index].name.Contains("rio_3") ? -0.5f : yPosition;

        GameObject trecho = Instantiate(
            trechosPrefabs[index],
            new Vector3(0, yPosition, nextSpawnZ),
            rotation
        );

        activeTrechos.Add(trecho);
        
        if (obstacleSpawner != null && !isRio)
        {
            obstacleSpawner.RegisterGroundTile(trecho);
        }

        nextSpawnZ += GetTrechoLength(trecho);

        ultimoFoiRio = isRio;
    }

    // Retorna o comprimento do trecho baseado no componente TrechoInfo.
    float GetTrechoLength(GameObject trecho)
    {
        TrechoInfo info = trecho.GetComponent<TrechoInfo>();
        if (info != null)
            return info.tamanho;
        return 10f;
    }

    // Retorna o comprimento do último trecho adicionado.
    float GetLastTrechoLength()
    {
        if (activeTrechos.Count == 0)
            return 10f;
        return GetTrechoLength(activeTrechos[activeTrechos.Count - 1]);
    }

    // Remove o trecho mais antigo para liberar memória.
    void RemoveFirstTrecho()
    {
        if (activeTrechos.Count > preSpawnCount)
        {
            if (obstacleSpawner != null)
            {
                obstacleSpawner.UnregisterGroundTile(activeTrechos[0]);
            }
            Destroy(activeTrechos[0]);
            activeTrechos.RemoveAt(0);
        }
    }
}