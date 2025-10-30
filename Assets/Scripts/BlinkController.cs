using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BlinkController : MonoBehaviour
{
    [Header("UI Lids")]
    [SerializeField] private RectTransform topLid;
    [SerializeField] private RectTransform bottomLid;

    [Header("Timing")]
    [SerializeField] private float closeTime = 0.08f;
    [SerializeField] private float holdClosed = 0.05f;
    [SerializeField] private float openTime = 0.10f;

    [Header("Curves (opcional)")]
    public AnimationCurve closeCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public AnimationCurve openCurve  = AnimationCurve.EaseInOut(0,0,1,1);

    [Header("Events")]
    public UnityEvent OnBlinkStart;
    public UnityEvent OnBlinkPeak;
    public UnityEvent OnBlinkEnd;

    Vector2 topStart, bottomStart;
    Vector2 topClosed, bottomClosed;
    bool isBlinking;

    void Awake()
    {
        topStart    = new Vector2(0f, 0f);
        bottomStart = new Vector2(0f, 0f);

        float halfScreen = topLid.rect.height;
        topStart.y    =  halfScreen;
        bottomStart.y = -halfScreen;
        topClosed     = Vector2.zero;
        bottomClosed  = Vector2.zero;

        // Inicial
        topLid.anchoredPosition    = topStart;
        bottomLid.anchoredPosition = bottomStart;
    }

    public void TriggerBlink()
    {
        if (!isBlinking) StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        isBlinking = true;
        OnBlinkStart?.Invoke();

        // Cerrar
        float t = 0;
        while (t < closeTime)
        {
            t += Time.deltaTime;
            float k = closeCurve.Evaluate(Mathf.Clamp01(t / closeTime));
            topLid.anchoredPosition    = Vector2.Lerp(topStart,    topClosed,    k);
            bottomLid.anchoredPosition = Vector2.Lerp(bottomStart, bottomClosed, k);
            yield return null;
        }
        topLid.anchoredPosition = topClosed;
        bottomLid.anchoredPosition = bottomClosed;

        OnBlinkPeak?.Invoke();

        yield return new WaitForSeconds(holdClosed);

        t = 0;
        while (t < openTime)
        {
            t += Time.deltaTime;
            float k = openCurve.Evaluate(Mathf.Clamp01(t / openTime));
            topLid.anchoredPosition    = Vector2.Lerp(topClosed,    topStart,    k);
            bottomLid.anchoredPosition = Vector2.Lerp(bottomClosed, bottomStart, k);
            yield return null;
        }
        topLid.anchoredPosition = topStart;
        bottomLid.anchoredPosition = bottomStart;

        OnBlinkEnd?.Invoke();
        isBlinking = false;
    }
}
