using UnityEngine;

public class EndlessCameraFlavia : MonoBehaviour
{
    public Transform target;
    public float advanceSpeed = 10f;
    public float minDistance = 12f;
    public float maxDistance = 6f;
    public float approachSpeed = 20f;

    // Define a posição inicial fixa da câmera.
    void Start()
    {
        transform.position = new Vector3(4f, 13f, 0f);
    }

    // Move a câmera para frente mantendo altura e alinhamento horizontal fixos.
    void Update()
    {
        if (target == null) return;

        float speed = advanceSpeed;
        float desiredZ = transform.position.z + speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, transform.position.y, desiredZ);
    }
}