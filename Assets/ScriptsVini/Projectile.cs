using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 80f; // Aumentado de 40 para 80 para acertar alvos em movimento
    [SerializeField] private float lifetime = 10f;

    private Rigidbody rb;
    private bool hasHitGround = false;

    // Inicializa o projétil com gravidade e define tempo de vida.
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Desligado para tiro reto (estilo laser) já que a nave voa alto
        }
        
        Destroy(gameObject, lifetime);
    }

    // Aplica movimento ao projétil enquanto ele não tiver atingido o chão.
    void FixedUpdate()
    {
        if (rb != null && !hasHitGround)
        {
            rb.linearVelocity = transform.forward * speed;
        }
    }

    // Gerencia colisão com o jogador (dano) ou com o cenário (física normal).
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
        else
        {
            hasHitGround = true;
        }
    }
}