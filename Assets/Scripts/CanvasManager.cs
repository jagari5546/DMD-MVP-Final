using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject FirstMessagePanel;

    [Header("Player (opcional)")]
    [SerializeField] private GameObject player;

    private AudioSource[] allAudioSources;
    private bool gameEnded = false;

    private void Awake()
    {
        // Juego totalmente pausado al iniciar
        Time.timeScale = 0f;

        if (startPanel != null)        startPanel.SetActive(true);
        if (FirstMessagePanel != null) FirstMessagePanel.SetActive(true);
        if (winPanel != null)          winPanel.SetActive(false);
        if (losePanel != null)         losePanel.SetActive(false);

        if (player != null) player.SetActive(false);

        // ✅ MOSTRAR CURSOR PARA PODER USAR LA UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        // Pausar todos los audios que hayan empezado con PlayOnAwake
        allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var a in allAudioSources)
        {
            if (a != null && a.isPlaying)
                a.Pause();
        }
    }

    // Llamar desde el botón de "Start"
    public void StartGame()
    {
        if (gameEnded) return;

        Time.timeScale = 1f;

        if (startPanel != null)        startPanel.SetActive(false);
        if (FirstMessagePanel != null) FirstMessagePanel.SetActive(false);
        if (player != null)            player.SetActive(true);

        // 🔒 OCULTAR CURSOR AL EMPEZAR A JUGAR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reactivar todos los audios que estaban pausados
        if (allAudioSources == null || allAudioSources.Length == 0)
            allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (var a in allAudioSources)
        {
            if (a != null)
                a.UnPause();
        }
    }

    public void Win()
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;

        if (winPanel != null)  winPanel.SetActive(true);
        if (losePanel != null) losePanel.SetActive(false);
        if (player != null)    player.SetActive(false);

        // Volvemos a mostrar el cursor en la victoria
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Lose()
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;

        if (losePanel != null) losePanel.SetActive(true);
        if (winPanel != null)  winPanel.SetActive(false);
        if (player != null)    player.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}
