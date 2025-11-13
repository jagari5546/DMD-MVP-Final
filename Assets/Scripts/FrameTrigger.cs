using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FrameTrigger : MonoBehaviour
{
    [Tooltip("Referencia al manager que controla qué cuadro está activo")]
    [SerializeField] private RandomFrameManager manager;

    private void Start()
    {
        // Aseguramos que sea trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo reaccionar si lo que entra tiene Rigidbody (player u otro)
        if (other.attachedRigidbody == null)
            return;

        if (manager == null)
            return;

        // Avisarle al manager que este cuadro fue tocado
        manager.NotifyFrameTouched(gameObject);
    }
}