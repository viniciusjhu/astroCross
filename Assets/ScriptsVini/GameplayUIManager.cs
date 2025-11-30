using UnityEngine;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    [Header("Referências da UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public GameObject restartButton; // Arraste o Botão de Restart para cá

    // Registra a UI desta cena no GameManager ao iniciar.
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterGameplayUI(this);
        }
    }
    
    // Solicita ao GameManager o reinício do jogo.
    public void OnRestartClick()
    {
        GameManager.Instance.RestartGame();
    }

    // Solicita ao GameManager o retorno ao menu.
    public void OnMenuClick()
    {
        GameManager.Instance.ReturnToMenu();
    }
}