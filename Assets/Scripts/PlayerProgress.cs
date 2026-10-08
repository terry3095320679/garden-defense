using System;
using UnityEngine;

public static class PlayerProgress
{
    private const string CoinsKey = "biubiubiu_Coins";
    private const string DiamondsKey = "biubiubiu_Diamonds";

    public static event Action CurrencyChanged;

    public static int Coins => PlayerPrefs.GetInt(CoinsKey, 0);
    public static int Diamonds => PlayerPrefs.GetInt(DiamondsKey, 0);

    public static void AddCoins(int amount)
    {
        PlayerPrefs.SetInt(CoinsKey, Mathf.Max(0, Coins + amount));
        PlayerPrefs.Save();
        CurrencyChanged?.Invoke();
    }

    public static void AddDiamonds(int amount)
    {
        PlayerPrefs.SetInt(DiamondsKey, Mathf.Max(0, Diamonds + amount));
        PlayerPrefs.Save();
        CurrencyChanged?.Invoke();
    }

    public static bool TrySpendCoins(int amount)
    {
        if (amount < 0 || Coins < amount)
        {
            return false;
        }

        PlayerPrefs.SetInt(CoinsKey, Coins - amount);
        PlayerPrefs.Save();
        CurrencyChanged?.Invoke();
        return true;
    }

    public static bool TrySpendDiamonds(int amount)
    {
        if (amount < 0 || Diamonds < amount)
        {
            return false;
        }

        PlayerPrefs.SetInt(DiamondsKey, Diamonds - amount);
        PlayerPrefs.Save();
        CurrencyChanged?.Invoke();
        return true;
    }
}
