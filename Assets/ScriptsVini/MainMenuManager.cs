using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Painéis")]
    [Tooltip("Arraste o GameObject do painel principal (com o título e botões Play/Tutorial/Settings)")]
    public GameObject mainPanel;
    
    [Tooltip("Arraste o GameObject do painel de Tutorial")]
    public GameObject tutorialPanel;
    
    [Tooltip("Arraste o GameObject do painel de Configurações")]
    public GameObject settingsPanel;
    
    [Tooltip("Arraste o painel da História (Narrativa)")]
    public GameObject storyPanel;

    [Header("Configurações de Cena")]
    [Tooltip("Nome da cena do jogo (ex: MainLevel)")]
    public string gameSceneName = "MainLevel";
    
    [Tooltip("Tempo de leitura da história em segundos")]
    public float storyDuration = 12f;

    // Inicializa o menu exibindo apenas o painel principal.
    private void Start()
    {
        ShowMainPanel();
        if (storyPanel != null) storyPanel.SetActive(false);
    }

    // Reseta o jogo e inicia o fluxo de história ou missão.
    public void PlayGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }

        if (storyPanel != null)
        {
            ShowStoryPanel();
        }
        else
        {
            StartMission();
        }
    }

    // Exibe o painel de história e oculta os demais.
    private void ShowStoryPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        
        storyPanel.SetActive(true);
    }
    
    // Carrega a cena do jogo.
    public void StartMission()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(gameSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    // Wrapper para carregar a cena do jogo (compatibilidade).
    public void LoadGameScene()
    {
        StartMission();
    }

    // Exibe o painel de tutorial.
    public void ShowTutorial()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    // Exibe o painel de configurações.
    public void ShowSettings()
    {
        mainPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Exibe o painel principal do menu.
    public void ShowMainPanel()
    {
        tutorialPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // Encerra a aplicação.
    public void QuitGame()
    {
        Debug.Log("Saindo do Jogo...");
        Application.Quit();
    }

    // Ajusta o volume da música de fundo.
    public void SetMusicVolume(float volume)
    {
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.SetVolume(volume);
        }
    }

    // Define a qualidade gráfica do jogo.
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    // Abre uma URL no navegador padrão.
    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }
}