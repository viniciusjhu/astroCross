using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Configuração da UI")]
    [Tooltip("O Painel de Pause que contém os botões e textos.")]
    public GameObject pausePanel;

    private bool isPaused = false;

    void Update()
    {
        // SEGURANÇA: Não permite pausar se o jogo já acabou (Game Over).
        // Isso evita conflitos de Time.timeScale com o GameManager.
        if (GameManager.Instance != null && 
            GameManager.Instance.gameOverPanel != null && 
            GameManager.Instance.gameOverPanel.activeSelf)
        {
            return;
        }

        // Detecta a tecla ESC para alternar o pause.
        // Usa Keyboard.current diretamente para evitar dependência de arquivos .inputactions específicos,
        // garantindo que funcione sem alterar os Assets de Input existentes.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Para o tempo do jogo
            if (pausePanel != null) pausePanel.SetActive(true);
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1f; // Retoma o tempo normal
            if (pausePanel != null) pausePanel.SetActive(false);
            Debug.Log("Game Resumed");
        }
    }

    // Função para ser chamada pelo botão 'Resume' na UI
    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    // Função para ser chamada pelo botão 'Menu' na UI
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Garante que o tempo volte ao normal antes de sair da cena
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
        else
        {
            // Fallback de segurança caso o GameManager não esteja presente (testes isolados)
            Debug.LogWarning("GameManager não encontrado. Carregando Menu Principal diretamente.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
