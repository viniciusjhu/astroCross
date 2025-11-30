using UnityEngine;

public class EndlessCamera : MonoBehaviour
{
    public float advanceSpeed = 5f;
    public float fixedX = 4f;
    public float fixedY = 13f;

    // Move a câmera constantemente para frente no eixo Z, mantendo X e Y fixos.
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = fixedX;
        pos.y = fixedY;
        pos.z += advanceSpeed * Time.deltaTime;

        transform.position = pos;
    }
}