using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObstacleController : MonoBehaviour
{
    [Header("Threat Settings")]
    [SerializeField] private float baseSpeed = 25f;
    [SerializeField] private float trajectoryRandomness = 3f;
    [SerializeField] private float downwardForce = 5f;

    [Header("Ambient Settings")]
    [SerializeField] private Vector3 ambientForce = new Vector3(0, -2, -10);

    private Rigidbody rb;
    public bool IsAmbient { get; private set; } = false;

    // Inicializa o Rigidbody ao acordar.
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody não encontrado no obstáculo!", this);
        }
    }

    // Configura o obstáculo para perseguir o alvo (jogador) com variações de trajetória.
    public void Initialize(Transform target, float speedMultiplier)
    {
        IsAmbient = false;
        if (target == null) return;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        
        float randomX = Random.Range(-trajectoryRandomness, trajectoryRandomness);
        float randomY = Random.Range(-trajectoryRandomness, trajectoryRandomness);
        Vector3 randomOffset = new Vector3(randomX, randomY, 0);

        Vector3 finalDirection = (directionToTarget + randomOffset).normalized;
        
        float finalSpeed = baseSpeed * speedMultiplier;
        Vector3 forceVector = finalDirection * finalSpeed;
        forceVector.y -= downwardForce;

        rb.AddForce(forceVector, ForceMode.Impulse);
    }

    // Configura o obstáculo apenas com força ambiental, sem perseguir o jogador.
    public void InitializeAmbient(float speedMultiplier)
    {
        IsAmbient = true;
        Vector3 forceVector = ambientForce * speedMultiplier;
        rb.AddForce(forceVector, ForceMode.Impulse);
    }

    // Detecta colisão com o jogador para causar dano e reiniciar posição.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ResetPosition();
            }
            
            Destroy(gameObject);
        }
    }
}