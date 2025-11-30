using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 2f;
    public float movementRangeX = 4f;

    private Vector3 startPosition;
    private float minX;
    private float maxX;
    private int direction = 1;

    // Inicializa limites de movimento baseados na posição inicial.
    void Start()
    {
        startPosition = transform.position;
        minX = startPosition.x - movementRangeX / 2;
        maxX = startPosition.x + movementRangeX / 2;
    }

    // Move o obstáculo lateralmente estilo ping-pong dentro dos limites.
    void Update()
    {
        transform.Translate(Vector3.right * speed * direction * Time.deltaTime);

        if (transform.position.x >= maxX)
        {
            direction = -1;
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= minX)
        {
            direction = 1;
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }
    }

    // Configura velocidade e alcance dinamicamente.
    public void Initialize(float newSpeed, float range)
    {
        this.speed = newSpeed;
        this.movementRangeX = range;

        startPosition = transform.position;
        minX = startPosition.x - movementRangeX / 2;
        maxX = startPosition.x + movementRangeX / 2;

        direction = Random.value > 0.5f ? 1 : -1;
    }
}