using UnityEngine;

public class LevelGeneratedFlavia : MonoBehaviour
{
    [SerializeField] GameObject chunkPrefab;

    // Instancia um chunk inicial na posição do gerador.
    void Start()
    {
        Instantiate(chunkPrefab, transform.position, Quaternion.identity);
    }
}