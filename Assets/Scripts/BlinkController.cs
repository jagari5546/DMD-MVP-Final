using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BlinkController : MonoBehaviour
{
    [Header("UI Lids")]
    [SerializeField] private RectTransform topLid;
    [SerializeField] private RectTransform bottomLid;

    [Header("Positions")]
    [Tooltip("Posición de los párpados cuando el ojo está ABIERTO")]
    [SerializeField] private Vector2 topOpenPos;
    [SerializeField] private Vector2 bottomOpenPos;

    [Tooltip("Posición de los párpados cuando el ojo está CERRADO")]
    [SerializeField] private Vector2 topClosedPos;
    [SerializeField] private Vector2 bottomClosedPos;

    [Header("Timing parpadeo")]
    [Tooltip("Cada cuántos segundos parpadea automáticamente")]
    [SerializeField] private float blinkInterval = 25f;

    [SerializeField] private float closeTime = 0.08f;
    [SerializeField] private float holdClosed = 0.05f;
    [SerializeField] private float openTime = 0.10f;

    [Header("Curves (opcional)")]
    public AnimationCurve closeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve openCurve  = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Events")]
    public UnityEvent OnBlinkStart;
    public UnityEvent OnBlinkPeak;
    public UnityEvent OnBlinkEnd;

    private bool isBlinking = false;
    private float blinkTimer = 0f;

    private void Start()
    {
        // Aseguramos iniciar con el ojo ABIERTO
        if (topLid != null)    topLid.anchoredPosition    = topOpenPos;
        if (bottomLid != null) bottomLid.anchoredPosition = bottomOpenPos;
    }

    private void Update()
    {
        // Timer para parpadeo automático
        blinkTimer += Time.deltaTime;

        if (!isBlinking && blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            TriggerBlink();
        }
    }

    /// <summary>
    /// Llamar esto desde otros scripts o desde un UnityEvent para forzar un parpadeo.
    /// </summary>
    public void TriggerBlink()
    {
        if (!isBlinking && topLid != null && bottomLid != null)
        {
            StartCoroutine(BlinkRoutine());
        }
    }

    private IEnumerator BlinkRoutine()
    {
        isBlinking = true;
        OnBlinkStart?.Invoke();

        // CERRAR OJO
        float t = 0f;
        while (t < closeTime)
        {
            t += Time.deltaTime;
            float k = closeCurve.Evaluate(Mathf.Clamp01(t / closeTime));

            topLid.anchoredPosition    = Vector2.Lerp(topOpenPos,    topClosedPos,    k);
            bottomLid.anchoredPosition = Vector2.Lerp(bottomOpenPos, bottomClosedPos, k);

            yield return null;
        }
        topLid.anchoredPosition    = topClosedPos;
        bottomLid.anchoredPosition = bottomClosedPos;

        OnBlinkPeak?.Invoke();

        // Mantener cerrado un momento
        yield return new WaitForSeconds(holdClosed);

        // ABRIR OJO
        t = 0f;
        while (t < openTime)
        {
            t += Time.deltaTime;
            float k = openCurve.Evaluate(Mathf.Clamp01(t / openTime));

            topLid.anchoredPosition    = Vector2.Lerp(topClosedPos,    topOpenPos,    k);
            bottomLid.anchoredPosition = Vector2.Lerp(bottomClosedPos, bottomOpenPos, k);

            yield return null;
        }
        topLid.anchoredPosition    = topOpenPos;
        bottomLid.anchoredPosition = bottomOpenPos;

        OnBlinkEnd?.Invoke();
        isBlinking = false;
    }

    // Opcional: para probar desde el menú contextual en el inspector
    [ContextMenu("Test Blink")]
    private void TestBlink()
    {
        TriggerBlink();
    }
}
