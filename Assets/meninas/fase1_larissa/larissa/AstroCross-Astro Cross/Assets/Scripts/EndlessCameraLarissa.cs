using UnityEngine;

public class EndlessCameraLarissa : MonoBehaviour
{
    public float advanceSpeed = 5f;
    public float fixedX = 4f;
    public float fixedY = 13f;

    // Move a câmera para frente constantemente no eixo Z.
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = fixedX;
        pos.y = fixedY;
        pos.z += advanceSpeed * Time.deltaTime;

        transform.position = pos;
    }
}