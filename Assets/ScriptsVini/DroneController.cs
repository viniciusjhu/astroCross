using System.Collections;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;

    [Header("Aiming")]
    [SerializeField] private float telegraphTime = 0.5f;
    [SerializeField] [Range(0, 10)] private float inaccuracy = 1.0f;
    [SerializeField] [Range(0f, 1f)] private float precisionChance = 0.4f;
    [SerializeField] private Vector3 aimOffset = new Vector3(0, 1.0f, 0); // Ajuste de altura para não atirar no chão

    [Header("Behavior")]
    [SerializeField] private float droneLifetime = 10f;

    [Header("Patrol")]
    [SerializeField] private float bobbingSpeed = 2f;
    [SerializeField] private float bobbingHeight = 0.5f;

    private Transform playerTransform;
    private bool isActivated = false;

    // Localiza o jogador pela tag 'Player'.
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("DroneController: Não foi possível encontrar o objeto do jogador. Verifique se ele tem a tag 'Player'.");
        }
    }

    // Controla a flutuação do drone enquanto inativo.
    void Update()
    {
        if (!isActivated)
        {
            float bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + bobbingOffset * Time.deltaTime, transform.localPosition.z);
        }
    }

    // Ativa o comportamento de ataque e agenda a destruição do drone.
    public void Activate()
    {
        if (isActivated) return;

        isActivated = true;
        
        StartCoroutine(ShootRoutine());
        Destroy(gameObject, droneLifetime);
        Debug.Log("Drone ativado!");
    }

    // Rotina de disparo com chance de tiro preciso (Sniper Shot).
    IEnumerator ShootRoutine()
    {
        while (true)
        {
            if (playerTransform != null)
            {
                // Rola o dado para ver se este será um tiro preciso
                bool isPreciseShot = Random.value < precisionChance;
                
                // Randomiza a altura do tiro entre 6 e 8 para equilibrar a dificuldade
                float randomY = Random.Range(6f, 8f);
                Vector3 dynamicOffset = new Vector3(aimOffset.x, randomY, aimOffset.z);

                // Calcula o ponto exato de mira (Posição do Player + Offset Dinâmico + Compensação de Movimento Z)
                Vector3 targetPosition = playerTransform.position + dynamicOffset + (Vector3.forward * 5f);

                // Mira inicial (Telégrafo)
                firePoint.LookAt(targetPosition);

                yield return new WaitForSeconds(telegraphTime);

                // Recalcula a mira final exata com o mesmo offset (para manter a consistência do tiro preparado)
                targetPosition = playerTransform.position + dynamicOffset + (Vector3.forward * 5f);
                firePoint.LookAt(targetPosition);

                Quaternion finalRotation;

                if (isPreciseShot)
                {
                    // TIRO PRECISO: Ignora a imprecisão
                    finalRotation = firePoint.rotation;
                }
                else
                {
                    // TIRO NORMAL: Adiciona o erro aleatório configurado
                    Quaternion aimRotation = firePoint.rotation;
                    Quaternion randomRotation = Random.rotation;
                    finalRotation = Quaternion.RotateTowards(aimRotation, randomRotation, inaccuracy);
                }

                Instantiate(projectilePrefab, firePoint.position, finalRotation);
            }

            yield return new WaitForSeconds(fireRate);
        }
    }
}