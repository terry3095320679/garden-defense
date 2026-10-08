using TMPro;
using UnityEngine;

public class CurrencyHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text diamondsText;

    private void OnEnable()
    {
        PlayerProgress.CurrencyChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        PlayerProgress.CurrencyChanged -= Refresh;
    }

    private void Refresh()
    {
        if (coinsText != null)
        {
            coinsText.text = $"Coins: {PlayerProgress.Coins}";
        }

        if (diamondsText != null)
        {
            diamondsText.text = $"Diamonds: {PlayerProgress.Diamonds}";
        }
    }
}
