using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyDisplayText;
    [SerializeField] private int StartingMoney;
    private int CurrentMoney;

    // Start is called before the first frame update
    private void Start()
    {
        CurrentMoney = StartingMoney;
        MoneyDisplayText.SetText($"${StartingMoney}");
    }

    public void AddMoney(int MoneyToAdd)
    {
        CurrentMoney += MoneyToAdd;
        MoneyDisplayText.SetText($"${CurrentMoney}");
    }

    public int GetMoney()
    {
        return CurrentMoney;
    }
}
