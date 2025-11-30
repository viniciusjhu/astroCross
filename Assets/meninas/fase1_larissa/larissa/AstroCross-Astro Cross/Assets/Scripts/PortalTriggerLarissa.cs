using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTriggerLarissa : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Nome exato da próxima cena para carregar")]
    [SerializeField] private string nextSceneName = "Level2";

    private bool activated = false;

    // Carrega a próxima cena quando o jogador entra no trigger do portal.
    void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("🌀 Portal da Fase 1 ativado! Indo para: " + nextSceneName);
            activated = true;
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(500); 
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }
}