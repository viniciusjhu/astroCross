using UnityEngine;

public class Moviment : MonoBehaviour
{
    private CharacterController character;
    [SerializeField] private Animator animator;
    private Vector3 inputs;

    private float velocity = 5.0f;

    // Inicializa o CharacterController e tenta localizar o componente Animator.
    void Start()
    {
        character = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogWarning("Animator não encontrado em " + gameObject.name + ". Arraste o Animator no Inspector ou coloque um Animator no GameObject/filho.");
    }

    // Processa inputs de movimento e atualiza os parâmetros do Animator conforme a direção.
    void Update()
    {
        inputs.Set(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        character?.Move(inputs * Time.deltaTime * velocity);

        bool isMoving = inputs.sqrMagnitude > 0.0001f;

        if (animator != null)
            animator.SetBool("andando", isMoving);

        if (Input.GetAxis("Vertical") > 0)
        {
            if (animator != null)
                animator.SetBool("frente", true);
        }
        else
        {
            if (animator != null)
                animator.SetBool("frente", false);
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            if (animator != null)
                animator.SetBool("traz", true);
        }
        else
        {
            if (animator != null)
                animator.SetBool("traz", false);
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            if (animator != null)
                animator.SetBool("direita", true);
        }
        else
        {
            if (animator != null)
                animator.SetBool("direita", false);
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            if (animator != null)
                animator.SetBool("esquerda", true);
        }
        else
        {
            if (animator != null)
                animator.SetBool("esquerda", false);
        }
    }
}