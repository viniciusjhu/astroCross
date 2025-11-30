using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float xClamp = 3f;
    [SerializeField] private float scorePerSecond = 10f;
    [SerializeField] private int hitPenalty = 200;
    [SerializeField] private int ambientHitPenalty = 50; // Penalidade menor para asteroides de ambiente
    [SerializeField] private LevelGenerator levelGenerator;

    private Vector2 movement;
    private Rigidbody rb;
    private Vector3 startPosition;
    private float scoreAccumulator = 0f;

    // Inicializa componentes e armazena a posição inicial.
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    // Executa a lógica de física e pontuação a cada frame fixo.
    void FixedUpdate()
    {
        HandleMovement();
        UpdateScore();
    }

    // Callback do Input System para capturar entrada de movimento.
    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    // Calcula e aplica pontuação baseada no movimento para frente.
    void UpdateScore()
    {
        if (movement.y != 0)
        {
            scoreAccumulator += movement.y * scorePerSecond * Time.fixedDeltaTime;

            if (Mathf.Abs(scoreAccumulator) >= 1f)
            {
                int pointsToAdd = (int)scoreAccumulator;
                
                GameManager.Instance.AddScore(pointsToAdd);
                scoreAccumulator -= pointsToAdd;
            }
        }
    }

    // Detecta colisões físicas e aplica penalidade de pontuação.
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision with: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
        
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Projectile"))
        {
            int finalPenalty = hitPenalty;

            // Verifica se é um asteroide de ambiente para aplicar dano reduzido
            ObstacleController obstacle = collision.gameObject.GetComponent<ObstacleController>();
            if (obstacle != null && obstacle.IsAmbient)
            {
                finalPenalty = ambientHitPenalty;
            }

            Debug.Log($"Player Hit! Deducting {finalPenalty} points.");
            GameManager.Instance.RemoveScore(finalPenalty);
        }
    }

    // Detecta triggers (projéteis) e aplica penalidade de pontuação.
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger with: {other.gameObject.name} (Tag: {other.gameObject.tag})");
        
        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy") || other.CompareTag("Projectile"))
        {
            int finalPenalty = hitPenalty;

            if (other.CompareTag("Projectile"))
            {
                Destroy(other.gameObject);
            }
            else
            {
                // Verifica também em triggers se é um obstáculo de ambiente
                ObstacleController obstacle = other.GetComponent<ObstacleController>();
                if (obstacle != null && obstacle.IsAmbient)
                {
                    finalPenalty = ambientHitPenalty;
                }
            }

            Debug.Log($"Player Hit! Deducting {finalPenalty} points.");
            GameManager.Instance.RemoveScore(finalPenalty);
        }
    }

    // Move o jogador aplicando velocidade e limitando a posição (clamp).
    void HandleMovement()
    {
        Vector3 currentPosition = rb.position;
        Vector3 moveDirection = new Vector3(movement.x, 0f, movement.y);
        Vector3 newPosition = currentPosition + moveDirection * (moveSpeed * Time.fixedDeltaTime);

        newPosition.x = Mathf.Clamp(newPosition.x, -xClamp, xClamp);

        if (levelGenerator != null)
        {
            float minZ = Camera.main.transform.position.z + 5f;
            float maxZ = levelGenerator.FurthestChunkZ - 5f;
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        }

        rb.MovePosition(newPosition);
    }

    // Reinicia a posição do jogador para o ponto inicial.
    public void ResetPosition()
    {
        rb.position = startPosition;
    }
}