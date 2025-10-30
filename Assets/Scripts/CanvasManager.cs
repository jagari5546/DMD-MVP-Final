using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CanvasManager : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] private GameObject player;
    [SerializeField] private LoseOrWinScript loseOrWin;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int gameTimeInSeconds = 300;

    [Header("Events")]
    public UnityEvent OnTimeUp;

    private PlayerManager playerManager;
    private float _remaining;
    private bool _running = false;
    private bool _ended = false;

    private void Start()
    {
        Time.timeScale = 0f;

        playerManager = player.GetComponent<PlayerManager>();
        playerManager.gameObject.SetActive(false);

        _remaining = Mathf.Max(0, gameTimeInSeconds);
        UpdateTimerText();
    }

    private void Update()
    {
        if (!_running || _ended) return;

        _remaining -= Time.deltaTime;

        if (_remaining <= 0f)
        {
            _remaining = 0f;
            UpdateTimerText();

            if (!_ended)
            {
                _ended = true;
                _running = false;

                OnTimeUp?.Invoke();

                if (loseOrWin != null)
                    loseOrWin.Lose();
            }
            return;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int total = Mathf.CeilToInt(_remaining);
        int minutes = total / 60;
        int seconds = total % 60;
        timerText.text = $"{minutes}:{seconds:00}";
    }

    public void StartGame()
    {
        if (_ended) return;
        Time.timeScale = 1f;
        playerManager.gameObject.SetActive(true);
        _running = true;
    }

    public void PauseGame()
    {
        _running = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (_ended) return;
        _running = true;
        Time.timeScale = 1f;
    }

    public void ResetTimer(int newTimeSeconds = -1)
    {
        if (newTimeSeconds > -1) gameTimeInSeconds = newTimeSeconds;

        _remaining = Mathf.Max(0, gameTimeInSeconds);
        _ended = false;
        _running = false;

        Time.timeScale = 0f;
        playerManager.gameObject.SetActive(false);

        UpdateTimerText();
    }
}
