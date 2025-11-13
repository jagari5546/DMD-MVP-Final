using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FrameTrigger : MonoBehaviour
{
    [Tooltip("Referencia al manager que controla qué cuadro está activo")]
    [SerializeField] private RandomFrameManager manager;

    [SerializeField] private string playerTag = "Player";

    private void Start()
    {
        // Aseguramos que sea trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Asegúrate de que es el player
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag))
            return;

        // Con el Rigidbody kinemático en el Player, OnTriggerEnter ya se va a disparar
        if (manager == null) return;

        // Avisar al manager
        manager.NotifyFrameTouched(gameObject);
        // Debug opcional:
        Debug.Log("FrameTrigger: tocaste " + gameObject.name);
    }
}