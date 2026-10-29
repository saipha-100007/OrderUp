using UnityEngine;
using TMPro;

public class GameWallet : MonoBehaviour
{
    [SerializeField] private int money;
    [SerializeField] private TMP_Text moneyText;

    private bool roundActive;

    public int Money => money;
    public bool RoundActive => roundActive;

    public void BeginRound()
    {
        money = 50;
        roundActive = true;
        RefreshDisplay();
    }

    public void FinishRound()
    {
        roundActive = false;
    }

    private void Start()
    {
        RefreshDisplay();
    }

    public void AddMoney(int amount)
    {
        if (!roundActive || money <= 0 || amount <= 0)
            return;

        money += amount;
        RefreshDisplay();
    }

    public bool TryDeductMoney(int amount)
    {
        if (!roundActive || money <= 0 || amount <= 0)
            return false;

        money = Mathf.Max(0, money - amount);
        RefreshDisplay();

        Debug.Log($"ของตก หัก {amount} บาท | เงินเหลือ {money}", this);
        return true;
    }

    private void RefreshDisplay()
    {
        if (moneyText != null)
            moneyText.text = $"Money: {money}";
    }
}