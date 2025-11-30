using UnityEngine;

public class LevelGenerated : MonoBehaviour
{
    [SerializeField] GameObject chunkPrefab;

    // Instancia o prefab do chunk na posição inicial do gerador.
    void Start()
    {
        Instantiate(chunkPrefab, transform.position, Quaternion.identity);
    }
}