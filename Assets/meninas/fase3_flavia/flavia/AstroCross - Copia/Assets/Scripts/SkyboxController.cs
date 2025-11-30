using UnityEngine;

[AddComponentMenu("Rendering/Skybox Controller")]
public class SkyboxController : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    [Tooltip("Define a rotação base do skybox. Você pode ajustar este valor em tempo real.")]
    [SerializeField] [Range(0f, 360f)] private float rotacaoBase = 0f;

    [Tooltip("Define a velocidade de rotação contínua do skybox. Deixe em 0 para um céu estático.")]
    [SerializeField] private float velocidadeDeRotacao = 0.5f;

    private Material skyboxMaterial;
    private float rotacaoDinamicaAcumulada = 0f;

    // Inicializa o material do Skybox duplicando-o para evitar alterações no asset original.
    void Start()
    {
        if (RenderSettings.skybox != null)
        {
            skyboxMaterial = new Material(RenderSettings.skybox);
            RenderSettings.skybox = skyboxMaterial;
        }
        else
        {
            Debug.LogWarning("Nenhum Skybox Material encontrado nas configurações de ambiente (Lighting > Environment).", this);
            enabled = false;
        }
    }

    // Atualiza a rotação do skybox frame a frame.
    void Update()
    {
        if (skyboxMaterial == null) return;

        rotacaoDinamicaAcumulada += velocidadeDeRotacao * Time.deltaTime;
        rotacaoDinamicaAcumulada %= 360f;

        float rotacaoFinal = rotacaoBase + rotacaoDinamicaAcumulada;
        rotacaoFinal %= 360f;

        skyboxMaterial.SetFloat("_Rotation", rotacaoFinal);
    }
}