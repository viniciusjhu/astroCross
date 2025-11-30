using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManagerFlavia : MonoBehaviour
{
    // Carrega a cena específica da fase 3.
    public void StartGame()
    {
        SceneManager.LoadScene("scene_3");
    }

    // Encerra a aplicação.
    public void QuitGame()
    {
        Debug.Log("SAINDO DO JOGO...");
        Application.Quit();
    }
}