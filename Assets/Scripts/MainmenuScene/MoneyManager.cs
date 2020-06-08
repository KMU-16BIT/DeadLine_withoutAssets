using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoneyManager:MonoBehaviour
{
    public static MoneyManager instance;

    [SerializeField] Text money_t;

    private void Awake() {
        instance = this;
    }

    public void setMoneyText(int money) {
        if (SceneManager.GetActiveScene().name == "Ingame")
            return;

        money_t.text = money.ToString(); 

        DataManager.instance.LoadPopUpScreen(false);
    } 

    public int getMoneyText()
    {
        return int.Parse(money_t.text);
    }

}
