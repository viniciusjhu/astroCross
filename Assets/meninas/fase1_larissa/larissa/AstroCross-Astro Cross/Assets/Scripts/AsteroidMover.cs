using UnityEngine;

public class AsteroidMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float travelDistance = 10f;
    [SerializeField] private float speed = 15f;
    [SerializeField] private Vector3 axis = Vector3.right;
    [SerializeField] private bool useLocalAxis = true;
    [SerializeField] private float pauseAtEnds = 0.25f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private int directionSign = 1;
    private float pauseTimer;

    // Inicializa o componente Rigidbody e define a posição inicial.
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{nameof(AsteroidMover)} precisa de um Rigidbody no mesmo objeto.");
        }

        startPosition = transform.position;
        axis = axis.sqrMagnitude > 0f ? axis.normalized : Vector3.right;
    }

    // Move o asteroide linearmente e inverte a direção ao atingir o limite.
    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 baseAxis = useLocalAxis ? transform.TransformDirection(axis) : axis;
        Vector3 target = startPosition + baseAxis * travelDistance * directionSign;

        Vector3 nextPosition = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(nextPosition);

        if ((nextPosition - target).sqrMagnitude <= 0.0001f)
        {
            directionSign *= -1;
            pauseTimer = pauseAtEnds;
        }
    }

    // Desenha gizmos no editor para visualizar o trajeto do asteroide.
    private void OnDrawGizmosSelected()
    {
        Vector3 previewAxis = axis.sqrMagnitude > 0f ? axis.normalized : Vector3.right;
        if (useLocalAxis)
        {
            previewAxis = transform.TransformDirection(previewAxis);
        }
        Vector3 origin = Application.isPlaying ? startPosition : transform.position;
        Vector3 endA = origin + previewAxis * travelDistance;
        Vector3 endB = origin - previewAxis * travelDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(endA, endB);
        Gizmos.DrawSphere(endA, 0.15f);
        Gizmos.DrawSphere(endB, 0.15f);
    }
}