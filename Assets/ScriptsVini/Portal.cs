using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    // Detecta a entrada do jogador no portal para carregar o próximo nível.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }

    // Calcula e carrega a próxima cena na ordem de Build.
    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        string sceneName = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
        sceneName = System.IO.Path.GetFileNameWithoutExtension(sceneName);

        Debug.Log($"Portal Entered! Transitioning to Scene Index {nextSceneIndex} ({sceneName})");

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}