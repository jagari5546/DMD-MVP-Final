using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ColliderScript : MonoBehaviour
{
    [Tooltip("Solo se dispara si el que entra tiene este tag. Deja vacío para aceptar cualquiera.")]
    [SerializeField] private string requiredTag = "Player";

    [Header("Eventos")]
    public UnityEvent onEnter;
    public UnityEvent onExit;

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        onEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        onExit?.Invoke();
    }
}