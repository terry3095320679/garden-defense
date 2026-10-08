using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maximumHealth = 50;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject gameOverPanel;

    private int currentHealth;
    private bool isDefeated;

    private void Start()
    {
        maximumHealth = PermanentUpgrades.ApplyHealth(maximumHealth);
        currentHealth = maximumHealth;
        isDefeated = false;
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        RefreshHealthText();
    }

    public void TakeDamage(int amount)
    {
        if (isDefeated || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        RefreshHealthText();

        if (currentHealth == 0)
        {
            isDefeated = true;

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void RefreshHealthText()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maximumHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"HP ({currentHealth} / {maximumHealth})";
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
