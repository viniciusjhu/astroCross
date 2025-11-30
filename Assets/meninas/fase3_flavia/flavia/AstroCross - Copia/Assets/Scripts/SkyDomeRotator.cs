using UnityEngine;

public class SkyDomeRotator : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    [Tooltip("Define a velocidade de rotação contínua para cada eixo (X, Y, Z).")]
    [SerializeField] private Vector3 velocidadeDeRotacao = new Vector3(0f, 0.5f, 0f);

    // Rotaciona o domo do céu continuamente.
    void Update()
    {
        transform.Rotate(velocidadeDeRotacao * Time.deltaTime);
    }
}