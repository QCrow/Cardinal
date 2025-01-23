using UnityEngine;

public static class PlayerData
{
    // Keys for PlayerPrefs
    private const string HealthKey = "PlayerHealth";
    private const string GoldKey = "PlayerGold";

    // --- Health Management ---
    public static void SetHealth(int health)
    {
        PlayerPrefs.SetInt(HealthKey, Mathf.Max(0, health)); // Ensure health is non-negative
        PlayerPrefs.Save();
    }

    public static int GetHealth()
    {
        return PlayerPrefs.GetInt(HealthKey, 100); // Default health is 100
    }

    public static void IncreaseHealth(int amount)
    {
        SetHealth(GetHealth() + amount);
    }

    public static void DecreaseHealth(int amount)
    {
        SetHealth(GetHealth() - amount);
    }

    public static void ResetHealth()
    {
        SetHealth(100); // Default value
    }

    // --- Gold Management ---
    public static void SetGold(int gold)
    {
        PlayerPrefs.SetInt(GoldKey, Mathf.Max(0, gold)); // Ensure gold is non-negative
        PlayerPrefs.Save();
    }

    public static int GetGold()
    {
        return PlayerPrefs.GetInt(GoldKey, 0); // Default gold is 0
    }

    public static void IncreaseGold(int amount)
    {
        SetGold(GetGold() + amount);
    }

    public static void DecreaseGold(int amount)
    {
        SetGold(GetGold() - amount);
    }

    public static void ResetGold()
    {
        SetGold(0); // Default value
    }

    // --- Reset All Player Data ---
    public static void ResetAll()
    {
        ResetHealth();
        ResetGold();
    }
}
