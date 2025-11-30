using UnityEngine;

public class RioPedraMove : MonoBehaviour
{
    public float speed = 3f;
    public float minX = -30f;
    public float maxX = 35f;
    public bool moveRight = true;

    // Move o objeto lateralmente e o teleporta ao atingir os limites (loop).
    void Update()
    {
        float direction = moveRight ? 1f : -1f;
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if (moveRight && transform.position.x > maxX)
        {
            Vector3 pos = transform.position;
            pos.x = minX;
            transform.position = pos;
        }
        else if (!moveRight && transform.position.x < minX)
        {
            Vector3 pos = transform.position;
            pos.x = maxX;
            transform.position = pos;
        }
    }
}