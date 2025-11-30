using UnityEngine;

public class MovimentLarissa : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isMoving = false;

    public float stepSize = 5f;
    public float stepDuration = 0.18f;

    private float maxZReached;
    private Transform cameraTransform;
    [SerializeField] private float gameOverDistanceBehindCamera = 8f;

    // Inicializa referências e ponto de partida.
    void Start()
    {
        maxZReached = transform.position.z;
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    // Processa inputs de movimento e condições de game over a cada frame.
    void Update()
    {
        transform.rotation = Quaternion.identity;

        if (transform.position.y < -2f)
        {
            TriggerGameOver("Caiu no buraco!");
            return;
        }

        if (cameraTransform != null)
        {
            if (transform.position.z < cameraTransform.position.z - gameOverDistanceBehindCamera)
            {
                TriggerGameOver("Ficou para trás da câmera!");
                return;
            }
        }

        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                StartCoroutine(MoveStep(Vector3.forward));
            if (Input.GetKeyDown(KeyCode.DownArrow))
                StartCoroutine(MoveStep(Vector3.back));
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                StartCoroutine(MoveStep(Vector3.left));
            if (Input.GetKeyDown(KeyCode.RightArrow))
                StartCoroutine(MoveStep(Vector3.right));
        }
    }

    // Executa o movimento suave do personagem e atualiza a pontuação.
    System.Collections.IEnumerator MoveStep(Vector3 direction)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * stepSize;

        float elapsed = 0;

        while (elapsed < stepDuration)
        {
            float t = elapsed / stepDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        SnapToGrid();

        if (transform.position.z > maxZReached)
        {
            float distanceGained = transform.position.z - maxZReached;
            int points = Mathf.RoundToInt(distanceGained);
            
            if (points > 0 && GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(points * 1);
            }
            
            maxZReached = transform.position.z;
        }

        isMoving = false;
    }

    // Aciona o estado de Game Over.
    void TriggerGameOver(string reason)
    {
        Debug.Log($"Game Over Fase 1: {reason}");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
            enabled = false;
        }
    }

    // Alinha o personagem à grade do cenário.
    void SnapToGrid()
    {
        float g = stepSize;

        Vector3 p = transform.position;

        p.x = Mathf.Round(p.x / g) * g;
        p.z = Mathf.Round(p.z / g) * g;

        transform.position = p;
    }

    // Detecta colisão com obstáculos letais.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid") || collision.gameObject.CompareTag("Obstacle"))
        {
            TriggerGameOver("Bateu no asteroide!");
        }
    }
}