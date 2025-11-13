using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RedDoor : MonoBehaviour
{
    [Header("Canvas / lógica de ganar")]
    [SerializeField] private CanvasManager canvasManager;

    [Header("Tag del jugador (opcional)")]
    [SerializeField] private string playerTag = "Player";

    private Collider triggerCollider;
    private bool doorUnlocked = false;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
        triggerCollider.enabled = false; // al inicio NO se puede ganar por aquí
    }

    // Llamado desde la llave roja (KeyScript) cuando se recoge
    public void EnableDoorTrigger()
    {
        doorUnlocked = true;

        if (triggerCollider != null)
            triggerCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!doorUnlocked) return; // si no se ha recogido la llave roja, no hace nada

        // Si usas tag de Player:
        if (!string.IsNullOrEmpty(playerTag))
        {
            if (!other.CompareTag(playerTag)) return;
        }
        else
        {
            // fallback: al menos que tenga Rigidbody
            if (other.attachedRigidbody == null) return;
        }

        if (canvasManager != null)
        {
            canvasManager.Win();
        }
    }
}