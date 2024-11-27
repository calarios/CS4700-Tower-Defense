using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyDisplayText;
    [SerializeField] private TextMeshProUGUI HealthDisplayText;

    [SerializeField] private TextMeshProUGUI MoneyDisplayText2;
    [SerializeField] private TextMeshProUGUI HealthDisplayText2;
    [SerializeField] private int StartingMoney = 1000;
    [SerializeField] private int StartingHealth = 100;
    private int CurrentMoney;
    private int CurrentHealth;

    // Singleton pattern for centralized access
    public static PlayerStats Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of PlayerStats exists in the scene
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CurrentMoney = StartingMoney;
        CurrentHealth = StartingHealth;

        // Set initial values for UI
        UpdateMoneyDisplay();
        UpdateHealthDisplay();
    }

    public void AddMoney(int amount)
    {
        if (CurrentHealth > 0)  // Prevent adding money if the castle is destroyed
        {
            CurrentMoney += amount;
            UpdateMoneyDisplay();
        }
    }

    public int GetMoney()
    {
        return CurrentMoney;
    }

    public void DamageCastle(int amount)
    {
        CurrentHealth -= amount;
        UpdateHealthDisplay();

        if (CurrentHealth <= 0)
        {
            HealthDisplayText.SetText("GAME OVER");
            HealthDisplayText2.SetText("GAME OVER");
            CurrentMoney = 0;
        }
    }

    public int GetHealth()
    {
        return CurrentHealth;
    }

    private void UpdateMoneyDisplay()
    {
        MoneyDisplayText.SetText($"${CurrentMoney}");
        MoneyDisplayText2.SetText($"${CurrentMoney}");
    }

    private void UpdateHealthDisplay()
    {
        HealthDisplayText.SetText($"Health: {CurrentHealth}%");
        HealthDisplayText2.SetText($"Health: {CurrentHealth}%");
    }
}
