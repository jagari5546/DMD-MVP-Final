using UnityEngine;
using UnityEngine.Events;

public class ClockScript : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private float startAngleZ = -40f;
    [SerializeField] private float rotationDuration = 300f; // 5 minutos

    [Header("Timing de Campanada")]
    [SerializeField] private float tickInterval = 25f;

    [Header("Sonido constante TAC-TAC (loop)")]
    [SerializeField] private AudioSource loopTickSource;
    [SerializeField] private AudioClip loopTickClip;

    [Header("Sonido campanada TIN")]
    [SerializeField] private AudioSource bellSource;
    [SerializeField] private AudioClip bellClip;
    [SerializeField] private float bellTotalTime = 5f;
    [SerializeField] private float bellFadeOutTime = 1.5f;

    [Header("Evento de Game Over")]
    [SerializeField] private UnityEvent onGameOver;

    private float elapsedTime = 0f;
    private float nextTickTime = 0f;
    private bool finished = false;
    private Coroutine bellRoutine;

    private void Start()
    {
        elapsedTime = 0f;
        nextTickTime = tickInterval;

        // Rotación inicial
        Vector3 euler = transform.localEulerAngles;
        euler.z = startAngleZ;
        transform.localEulerAngles = euler;

        Configure3DAudio();

        // Iniciar TAC TAC en loop
        if (loopTickSource != null && loopTickClip != null)
        {
            loopTickSource.clip = loopTickClip;
            loopTickSource.loop = true;
            loopTickSource.Play();
        }
    }

    private void Configure3DAudio()
    {
        // TAC TAC (cercano)
        if (loopTickSource != null)
        {
            loopTickSource.spatialBlend = 1f; // 3D
            loopTickSource.minDistance = 1.5f;
            loopTickSource.maxDistance = 6f;
            loopTickSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        // Campanada (se escucha más lejos)
        if (bellSource != null)
        {
            bellSource.spatialBlend = 1f; // 3D
            bellSource.minDistance = 3f;
            bellSource.maxDistance = 15f;
            bellSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }
    }

    private void Update()
    {
        if (finished) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / rotationDuration);
        float currentAngle = startAngleZ + 360f * t;

        Vector3 euler = transform.localEulerAngles;
        euler.z = currentAngle;
        transform.localEulerAngles = euler;

        if (elapsedTime >= nextTickTime && nextTickTime <= rotationDuration)
        {
            TriggerBell();
            nextTickTime += tickInterval;
        }

        if (elapsedTime >= rotationDuration)
        {
            finished = true;

            euler = transform.localEulerAngles;
            euler.z = startAngleZ + 360f;
            transform.localEulerAngles = euler;

            StopAllAudio();
            GameOver();
        }
    }

    private void TriggerBell()
    {
        if (bellSource == null || bellClip == null)
            return;

        if (bellRoutine != null)
            StopCoroutine(bellRoutine);

        bellRoutine = StartCoroutine(BellRoutine());
    }

    private System.Collections.IEnumerator BellRoutine()
    {
        bellSource.clip = bellClip;
        bellSource.loop = false;
        bellSource.volume = 1f;
        bellSource.Play();

        float playTime = Mathf.Max(0f, bellTotalTime - bellFadeOutTime);
        if (playTime > 0f)
            yield return new WaitForSeconds(playTime);

        float t = 0f;
        float startVol = bellSource.volume;
        float fadeTime = Mathf.Max(0.01f, bellFadeOutTime);

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeTime);
            bellSource.volume = Mathf.Lerp(startVol, 0f, k);
            yield return null;
        }

        bellSource.Stop();
        bellSource.volume = startVol;
        bellRoutine = null;
    }

    private void StopAllAudio()
    {
        if (loopTickSource != null)
            loopTickSource.Stop();

        if (bellSource != null)
        {
            if (bellRoutine != null)
            {
                StopCoroutine(bellRoutine);
                bellRoutine = null;
            }
            bellSource.Stop();
        }
    }

    private void GameOver()
    {
        Debug.Log("Reloj terminó su vuelta completa. Game Over.");
        onGameOver?.Invoke();
    }
}
