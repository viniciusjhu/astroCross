using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    
    [Header("Configurações")]
    public float fadeDuration = 1.0f;
    public Color fadeColor = Color.black;
    
    private Canvas fadeCanvas;
    private Image fadeImage;

    // Garante Singleton e inicializa o canvas de fade.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupFadeCanvas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Cria dinamicamente a estrutura de UI necessária para o efeito de fade.
    private void SetupFadeCanvas()
    {
        GameObject canvasGO = new GameObject("TransitionCanvas");
        canvasGO.transform.SetParent(transform);
        
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        fadeImage.raycastTarget = false;

        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    // Registra o evento de carregamento de cena.
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Desregistra o evento de carregamento de cena.
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Inicia o efeito de clareamento (Fade In) ao carregar uma nova cena.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeRoutine(1f, 0f));
    }

    // Inicia o processo de transição para uma nova cena.
    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    // Executa a sequência de Fade Out, carregamento e Fade In.
    private IEnumerator TransitionRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeRoutine(0f, 1f));

        SceneManager.LoadScene(sceneName);
    }

    // Interpola a opacidade da imagem de fade ao longo do tempo.
    private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        
        if (fadeImage != null)
        {
            fadeImage.raycastTarget = (endAlpha > 0.1f);
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, newAlpha);
                yield return null;
            }

            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, endAlpha);
        }
    }
}