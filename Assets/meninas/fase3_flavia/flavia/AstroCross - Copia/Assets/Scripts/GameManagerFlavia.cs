using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerFlavia : MonoBehaviour
{
    public static GameManagerFlavia Instance { get; private set; }

    [Header("Referências")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;

    [Header("Configurações de Jogo")]
    [SerializeField] private float scorePerUnit = 1f;
    [SerializeField] private float gameOverBehindCameraDistance = 10f;

    public bool IsGameOver { get; private set; } = false;

    public int Score { get; private set; } = 0;
    private float maxZReached;

    // Inicializa o singleton local.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Configura estado inicial e valida referências.
    private void Start()
    {
        IsGameOver = false;
        Score = 0;

        if (cameraTransform == null) 
        {
            if (Camera.main != null) cameraTransform = Camera.main.transform;
            else Debug.LogError("GameManagerFlavia: 'cameraTransform' não atribuído!");
        }
        
        if (playerTransform == null) 
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            else Debug.LogError("GameManagerFlavia: 'playerTransform' não atribuído!");
        }

        if (playerTransform != null) {
            maxZReached = playerTransform.position.z;
        }
        
        if (GameManager.Instance != null)
        {
             // Placeholder para sincronização futura
        }
    }

    // Monitora progresso do jogador para pontuação e condições de game over.
    private void Update()
    {
        if (IsGameOver) return;

        if (playerTransform != null)
        {
            float currentPlayerZ = playerTransform.position.z;

            if (currentPlayerZ > maxZReached)
            {
                float progressMade = currentPlayerZ - maxZReached;
                
                int pointsToAdd = Mathf.RoundToInt(progressMade * scorePerUnit);
                
                if (pointsToAdd > 0)
                {
                    AddScore(pointsToAdd);
                    maxZReached = currentPlayerZ;
                }
            }
        }

        if (playerTransform != null && cameraTransform != null)
        {
            if (playerTransform.position.z < cameraTransform.position.z - gameOverBehindCameraDistance)
            {
                Debug.Log("Game Over Flavia: Jogador ficou para trás da câmera.");
                EndGame();
            }
        }
    }

    // Adiciona pontos localmente e reporta ao GameManager global.
    public void AddScore(int points)
    {
        if (IsGameOver) return;
        
        Score += points;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(points);
        }
    }

    // Subtrai pontos localmente e reporta ao GameManager global.
    public bool SubtractScore(int points)
    {
        if (IsGameOver) return false;

        Score -= points;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemoveScore(points);
            return true;
        }
        
        return false;
    }

    // Inicia a sequência de Game Over.
    public void EndGame()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        
        Debug.Log("GameManagerFlavia: Solicitando Game Over Global.");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager Global não encontrado para executar Game Over!");
            Time.timeScale = 0; 
        }
    }

    // Solicita o reinício do jogo.
    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}