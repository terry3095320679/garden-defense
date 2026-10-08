using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class GameTimer : MonoBehaviour
{
    private TMP_Text timerText;
    private float elapsedTime;

    private void Awake()
    {
        timerText = GetComponent<TMP_Text>();
        RefreshDisplay();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        int totalSeconds = Mathf.FloorToInt(elapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = $"TIME {minutes:00}:{seconds:00}";
    }
}
