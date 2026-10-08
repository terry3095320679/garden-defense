using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;

    private void Start()
    {
        Time.timeScale = 1f;
        SetPaused(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetPaused(!isPaused);
        }
    }

    public void ResumeGame()
    {
        SetPaused(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(paused);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
