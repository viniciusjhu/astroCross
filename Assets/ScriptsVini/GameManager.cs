using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int TotalScore { get; private set; }
    public int CurrentLevelScore { get; private set; }

    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Debug / Testing")]
    public bool debugMode = false;
    public int debugStartingScore = 1000;

    // Configuration
    public int Phase1Target = 1000;
    public int Phase2Target = 2000;
    public float PortalRespawnDelay = 10f;
    
    private bool portalSpawned = false;
    private float nextPortalAttemptTime = 0f;
    private bool isGameOver = false;
    private GameObject currentRestartButton;

    // Configura o Singleton para persistir entre cenas.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Monitora inputs de debug se o modo estiver ativo.
    void Update()
    {
        if (isGameOver) return;

        if (debugMode && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Debug.Log("Debug: Forcing Portal Spawn");
            SpawnPortal();
        }
    }

    // Inicializa o score e registra callbacks de cena.
    void Start()
    {
        if (debugMode && TotalScore == 0)
        {
            TotalScore = debugStartingScore;
            CurrentLevelScore = debugStartingScore;
            Debug.Log($"Debug Mode: Starting with {TotalScore} points.");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    // Reseta estados e configura metas ao carregar uma nova cena.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        portalSpawned = false;
        isGameOver = false;
        Time.timeScale = 1f;
        nextPortalAttemptTime = 0f;
        CurrentLevelScore = 0;
        
        Debug.Log($"Scene Loaded: '{scene.name}'");
        
        int target = int.MaxValue;
        string lowerName = scene.name.ToLower();

        if (lowerName.Contains("mainlevel") || lowerName.Contains("level1")) target = Phase1Target;
        else if (lowerName.Contains("level2")) target = Phase2Target;
        
        Debug.Log($"Current Total Score: {TotalScore}. Target for this level ('{scene.name}'): {target}");
    }

    // Conecta a UI da cena atual ao GameManager.
    public void RegisterGameplayUI(GameplayUIManager ui)
    {
        Debug.Log("UI Registered for this scene.");
        this.scoreText = ui.scoreText;
        this.gameOverPanel = ui.gameOverPanel;
        this.finalScoreText = ui.finalScoreText;
        this.currentRestartButton = ui.restartButton;

        if (gameOverPanel != null) 
            gameOverPanel.SetActive(false);

        UpdateScoreUI();
    }

    // Ativa o estado de Game Over e pausa o jogo.
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Time.timeScale = 0f;
        Debug.Log("GAME OVER!");

        string sceneName = SceneManager.GetActiveScene().name.ToLower();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = "FINAL SCORE: " + TotalScore.ToString();
            }

            // Oculta o botão de Try Again se for a Fase 3 (Fim de Jogo real)
            if (currentRestartButton != null)
            {
                if (sceneName.Contains("scene_3") || sceneName.Contains("level3") || sceneName.Contains("fase3"))
                {
                    currentRestartButton.SetActive(false);
                }
                else
                {
                    currentRestartButton.SetActive(true);
                }
            }
        }
    }

    // Reinicia as variáveis de estado do jogo.
    public void ResetGame()
    {
        TotalScore = 0;
        CurrentLevelScore = 0;
        isGameOver = false;
        portalSpawned = false;
        Time.timeScale = 1f;
        
        Debug.Log("Game State Reset.");
    }

    // Reinicia a cena atual.
    public void RestartGame()
    {
        ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Retorna ao menu principal.
    public void ReturnToMenu()
    {
        ResetGame();
        SceneManager.LoadScene("MainMenu");
    }

    // Adiciona pontos e verifica se a meta foi atingida.
    public void AddScore(int amount)
    {
        TotalScore += amount;
        CurrentLevelScore += amount;

        UpdateScoreUI();
        CheckScoreMilestone();

        if (TotalScore < 0)
        {
            GameOver();
        }
    }

    // Remove pontos e verifica condição de Game Over.
    public void RemoveScore(int amount)
    {
        TotalScore -= amount;
        
        UpdateScoreUI();

        if (TotalScore < 0)
        {
            GameOver();
        }
    }

    // Atualiza o texto da UI de pontuação.
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + TotalScore.ToString();
        }
    }

    // Verifica se a pontuação atingiu a meta da fase para spawnar o portal.
    private void CheckScoreMilestone()
    {
        if (portalSpawned) return;
        
        if (Time.time < nextPortalAttemptTime) return;

        string sceneName = SceneManager.GetActiveScene().name;
        string lowerName = sceneName.ToLower();
        int target = int.MaxValue;

        if (lowerName.Contains("mainlevel") || lowerName.Contains("level1")) 
        {
            target = Phase1Target;
        }
        else if (lowerName.Contains("level2"))
        {
            target = Phase2Target;
        }

        if (TotalScore >= target)
        {
            Debug.Log($"Milestone Reached! Score: {TotalScore} >= Target: {target}. Spawning...");
            SpawnPortal();
        }
    }

    // Solicita ao gerador de nível que crie o portal.
    private void SpawnPortal()
    {
        Debug.Log("Target Score Reached! Spawning Portal...");
        portalSpawned = true;

        LevelGenerator generator = FindFirstObjectByType<LevelGenerator>();
        if (generator != null)
        {
            generator.RequestPortalSpawn();
            return;
        }

        LevelGeneratedLarissa generatorLarissa = FindFirstObjectByType<LevelGeneratedLarissa>();
        if (generatorLarissa != null)
        {
            generatorLarissa.RequestPortalSpawn();
            return;
        }
        
        Debug.LogWarning("Nenhum Level Generator encontrado para spawnar o portal!");
    }

    // Lida com o caso de o jogador perder o portal, agendando nova tentativa.
    public void PortalMissed()
    {
        portalSpawned = false; 
        nextPortalAttemptTime = Time.time + PortalRespawnDelay;
        Debug.Log($"Portal Missed! Will spawn again in {PortalRespawnDelay} seconds.");
    }
}