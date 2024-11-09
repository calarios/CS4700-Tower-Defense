using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyDisplayText;
    [SerializeField] private int StartingMoney;
    private int CurrentMoney;

    [SerializeField] private TextMeshProUGUI HealthDisplayText;
    [SerializeField] private int StartingHealth;
    private int CurrentHealth;

    // Start is called before the first frame update
    private void Start()
    {
        CurrentMoney = StartingMoney;
        MoneyDisplayText.SetText($"${StartingMoney}");

        CurrentHealth = StartingHealth;
        HealthDisplayText.SetText($"Health Percentage: {StartingHealth}%");
    }

    public void AddMoney(int MoneyToAdd)
    {
        if (!(CurrentHealth <= 0))
        {
            CurrentMoney += MoneyToAdd;
            MoneyDisplayText.SetText($"${CurrentMoney}");
        }
        else
        {
            MoneyDisplayText.SetText("THE CASTLE IS DESTROYED!");
        }
    }

    public int GetMoney()
    {
        return CurrentMoney;
    }

    public void DamageCastle(int castleDamage)
    {

        
        CurrentHealth -= castleDamage;
        HealthDisplayText.SetText($"Health Percentage: {CurrentHealth}%");
        
        if (CurrentHealth <= 0)
        {
            HealthDisplayText.SetText("GAME OVER");

            CurrentMoney = 0;
        }
    }

    public int GetHealth()
    {
        return CurrentHealth;
    }
}
