using UnityEngine;
using UnityEngine.Events;

public class RandomFrameManager : MonoBehaviour
{
    [Header("Cuadros disponibles")]
    [Tooltip("Arrastra aquí todos los cuadros que tienen BoxCollider (IsTrigger)")]
    [SerializeField] private GameObject[] frames;

    [Header("Objeto a activar (por ejemplo, la llave)")]
    [SerializeField] private GameObject objectToActivate;

    [Header("Sonidos al tocar el cuadro correcto")]
    [Tooltip("Se pueden poner uno o varios AudioSource")]
    [SerializeField] private AudioSource[] audioSources;

    [Header("Eventos extra (opcional)")]
    [Tooltip("Se dispara cuando el jugador toca el cuadro ACTIVO")]
    public UnityEvent onActiveFrameTouched;

    private int currentIndex = -1;
    private bool hasActivatedObject = false;

    private void Start()
    {
        // 1) Desactivar todos los cuadros al inicio
        DeactivateAllFrames();

        // 2) Opcional: desactivar el objeto a activar (llave, etc.)
        if (objectToActivate != null)
            objectToActivate.SetActive(false);

        // 3) Elegimos y activamos un cuadro aleatorio
        PickRandomActiveFrame();
    }

    private void DeactivateAllFrames()
    {
        if (frames == null) return;

        for (int i = 0; i < frames.Length; i++)
        {
            if (frames[i] != null)
                frames[i].SetActive(false);
        }
    }

    /// <summary>
    /// Función pública para elegir un cuadro aleatorio.
    /// Llámala desde el parpadeo cada 25 segundos.
    /// </summary>
    public void PickRandomActiveFrame()
    {
        if (frames == null || frames.Length == 0) return;

        // Elegimos nuevo índice
        int newIndex = Random.Range(0, frames.Length);

        // Intentamos no repetir el mismo si hay más de uno
        if (frames.Length > 1 && currentIndex != -1)
        {
            int safety = 0;
            while (newIndex == currentIndex && safety < 20)
            {
                newIndex = Random.Range(0, frames.Length);
                safety++;
            }
        }

        currentIndex = newIndex;

        // Desactivar todos y activar solo el elegido
        for (int i = 0; i < frames.Length; i++)
        {
            if (frames[i] == null) continue;
            frames[i].SetActive(i == currentIndex);
        }

        // (Si quieres debug:)
        // Debug.Log("Frame activo: " + currentIndex + " -> " + frames[currentIndex].name);
    }

    /// <summary>
    /// Llamado por los cuadros cuando el jugador entra en su collider.
    /// </summary>
    public void NotifyFrameTouched(GameObject frameGO)
    {
        if (frames == null || frames.Length == 0) return;
        if (currentIndex < 0 || currentIndex >= frames.Length) return;

        // ¿Este es el cuadro activo?
        if (frameGO == frames[currentIndex])
        {
            // Activar la llave (u objeto) solo la primera vez
            if (!hasActivatedObject)
            {
                if (objectToActivate != null)
                    objectToActivate.SetActive(true);

                hasActivatedObject = true;
            }

            // Reproducir todos los sonidos asignados
            if (audioSources != null)
            {
                foreach (var a in audioSources)
                {
                    if (a != null)
                        a.Play();
                }
            }

            // Eventos adicionales
            onActiveFrameTouched?.Invoke();
        }
        else
        {
            // Aquí podrías poner un sonido de error si tocan un cuadro incorrecto
            // Debug.Log("Tocaste el cuadro incorrecto: " + frameGO.name);
        }
    }
}
