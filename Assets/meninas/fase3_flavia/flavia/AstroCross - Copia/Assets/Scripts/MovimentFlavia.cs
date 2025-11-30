using UnityEngine;
using System.Collections;

public class MovimentFlavia : MonoBehaviour
{
    private CharacterController character;
    [SerializeField] private Animator animator;
    private Vector3 inputs;
    public float jumpForce = 5f;
    [SerializeField] private float gravityValue = -9.81f;
    
    [Header("Configurações de Colisão")]
    [SerializeField] private int pointsToSubtractOnObstacleHit = 1;
    
    [Header("Configurações de Game Over")]
    [SerializeField] private float maxDistanceBehindCamera = 10f;

    private bool isMoving = false;
    public float stepSize = 1f;
    public float stepDuration = 0.18f;

    // Inicializa CharacterController e Animator.
    void Start()
    {
        character = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogWarning("Animator não encontrado em " + gameObject.name + ". Arraste o Animator no Inspector ou coloque um Animator no GameObject/filho.");
    }

    // Gerencia input de movimento, gravidade e verificação de game over por câmera.
    void Update()
    {
        if (Camera.main != null)
        {
            if (transform.position.z < Camera.main.transform.position.z - maxDistanceBehindCamera)
            {
                if (GameManager.Instance != null)
                {
                    Debug.Log("Game Over! Ficou para trás da câmera.");
                    GameManager.Instance.GameOver();
                    return;
                }
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
            
            character.Move(Vector3.up * gravityValue * Time.deltaTime);
        }
    }

    // Corrotina para movimento suave em arco (pulinho).
    System.Collections.IEnumerator MoveStep(Vector3 direction)
    {
        isMoving = true;
        float elapsed = 0;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * stepSize;

        float minAllowedZ = Camera.main.transform.position.z - 7f;
        if (endPos.z < minAllowedZ)
        {
            endPos.z = minAllowedZ;
        }

        float hopHeight = jumpForce;

        while (elapsed < stepDuration)
        {
            float t = elapsed / stepDuration;

            Vector3 currentHorizontalPos = Vector3.Lerp(startPos, endPos, t);

            float currentY = startPos.y + (Mathf.Sin(t * Mathf.PI) * hopHeight);

            Vector3 targetPos = new Vector3(currentHorizontalPos.x, currentY, currentHorizontalPos.z);

            character.Move(targetPos - transform.position);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (GameManager.Instance != null)
        {
            // Lógica de pontuação: só ganha pontos se mover para frente ou lados
            // Se mover para trás, perde pontos (penalidade)
            if (direction == Vector3.back)
            {
                GameManager.Instance.RemoveScore((int)stepSize);
            }
            else
            {
                GameManager.Instance.AddScore((int)stepSize);
            }
        }

        transform.position = new Vector3(endPos.x, startPos.y, endPos.z);
        isMoving = false;
    }

    // Detecta colisões físicas com obstáculos e trata penalidades.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("ObjectSpawn"))
        {
            if (GameManager.Instance != null)
            {
                Debug.Log("Game Over! Colidiu com um obstáculo fatal.");
                GameManager.Instance.GameOver();
            }
            Destroy(hit.gameObject);
        }
        else if (hit.gameObject.CompareTag("PointObstacle"))
        {
            if (GameManager.Instance != null)
            {
                Debug.Log($"Pontos subtraídos! Colidiu com obstáculo de pontos.");
                GameManager.Instance.RemoveScore(pointsToSubtractOnObstacleHit);
            }
            Destroy(hit.gameObject);
        }
    }

    // Detecta triggers de ambiente (água).
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (GameManager.Instance != null)
            {
                Debug.Log("Game Over! Caiu na água.");
                GameManager.Instance.GameOver();
            }
        }
    }
}