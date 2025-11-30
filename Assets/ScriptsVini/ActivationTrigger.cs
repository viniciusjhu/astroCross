using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ActivationTrigger : MonoBehaviour
{
    [SerializeField] private DroneController[] dronesToActivate;

    // Inicializa o collider e tenta localizar drones automaticamente se a lista estiver vazia.
    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        if (dronesToActivate == null || dronesToActivate.Length == 0)
        {
            if (transform.parent != null)
            {
                dronesToActivate = transform.parent.GetComponentsInChildren<DroneController>();
            }
        }
    }

    // Ativa os drones associados quando o jogador atravessa o trigger.
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            string parentName = transform.parent != null ? transform.parent.name : "SEM PAI";
            Debug.Log($"[ActivationTrigger] GATILHO ACIONADO no Chunk: '{parentName}' pelo objeto: '{other.name}'");

            if (dronesToActivate != null && dronesToActivate.Length > 0)
            {
                Debug.Log($"[ActivationTrigger] Tentando ativar {dronesToActivate.Length} drones locais...");
                foreach (var drone in dronesToActivate)
                {
                    if (drone != null)
                    {
                        Debug.Log($"   -> Ativando Drone: '{drone.name}' (Parent: {drone.transform.parent.name})");
                        drone.Activate();
                    }
                    else
                    {
                        Debug.LogWarning("   -> Referência de drone nula encontrada na lista!");
                    }
                }
                
                GetComponent<Collider>().enabled = false;
            }
            else
            {
                Debug.LogWarning($"[ActivationTrigger] O Chunk '{parentName}' não encontrou drones na lista para ativar!", this);
            }
        }
    }
}