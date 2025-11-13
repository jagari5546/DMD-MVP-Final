using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class KeyScript : MonoBehaviour
{
    [Header("Movimiento visual")]
    [SerializeField] private float floatAmplitude = 0.1f;   // qué tanto sube y baja
    [SerializeField] private float floatSpeed = 2f;         // velocidad del movimiento vertical
    [SerializeField] private float rotateSpeed = 90f;       // grados por segundo

    [Header("Sonido al recoger (opcional)")]
    [SerializeField] private AudioSource pickupAudioSource; // opcional
    [SerializeField] private AudioClip pickupClip;          // opcional

    [Header("Acciones al recoger (abrir puerta, etc.)")]
    public UnityEvent onKeyCollected;

    [Header("Llave roja (opcional)")]
    [SerializeField] private bool isRedKey = false;
    [SerializeField] private RedDoor redDoor;   // Puerta roja a desbloquear

    private Vector3 startPosition;
    private bool collected = false;

    private void Start()
    {
        // Guardamos la posición inicial para hacer el "flotar" alrededor de ese punto
        startPosition = transform.position;

        // Aseguramos que el collider sea trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (collected) return;

        // Movimiento de flotar (arriba/abajo) con seno
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = startPosition + Vector3.up * offset;

        // Rotación constante
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        // Solo reaccionar si lo que entra tiene un Rigidbody (player u otro)
        if (other.attachedRigidbody == null)
            return;

        collected = true;

        // Si es la llave roja, avisar a la puerta roja
        if (isRedKey && redDoor != null)
        {
            redDoor.EnableDoorTrigger();
        }

        // Reproducir sonido si se configuró (si no pones nada en la roja, no sonará)
        if (pickupAudioSource != null)
        {
            if (pickupClip != null)
                pickupAudioSource.PlayOneShot(pickupClip);
            else
                pickupAudioSource.Play();
        }

        // Ejecutar acciones (abrir puerta, sumar llaves, etc.)
        onKeyCollected?.Invoke();

        // Desactivar la llave visualmente y el collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        foreach (var rend in GetComponentsInChildren<Renderer>())
            rend.enabled = false;
    }
}
