using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    // Calcula o offset inicial entre a câmera e o jogador.
    void Start()
    {
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    // Atualiza a posição da câmera após o movimento do jogador, mantendo o offset.
    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}