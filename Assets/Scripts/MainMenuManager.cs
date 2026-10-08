using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        Transform canvas = FindFirstObjectByType<Canvas>()?.transform;
        WireButton(canvas, "StartButton", StartGame);
        WireButton(canvas, "QuitButton", QuitGame);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private static void WireButton(
        Transform root,
        string objectName,
        UnityEngine.Events.UnityAction action)
    {
        Button button = root != null ? root.Find(objectName)?.GetComponent<Button>() : null;
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
