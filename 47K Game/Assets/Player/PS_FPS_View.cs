using UnityEngine;

public class PS_FPS_View : MonoBehaviour
{
    private void Start()
    {
        // Access PlayerStats to initialize text UI
        // MoneyDisplayText = GameObject.FindGameObjectWithTag("MoneyDisplay").GetComponent<TextMeshProUGUI>();
        // HealthDisplayText = GameObject.FindGameObjectWithTag("HealthDisplay").GetComponent<TextMeshProUGUI>();

        // The display is now handled automatically by PlayerStats
    }

    public void AddMoney(int amount)
    {
        PlayerStats.Instance.AddMoney(amount);
    }

    public void DamageCastle(int damage)
    {
        PlayerStats.Instance.DamageCastle(damage);
    }
}
