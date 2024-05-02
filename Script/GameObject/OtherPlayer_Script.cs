using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer_Script : MonoBehaviour
{
    private string PlayerName;
    public string GetPlayerName() { return PlayerName; }
    private int PlayerLv;
    private int PlayerMaxHp;
    private int PlayerNowHp;
    private int PlayerAtk;
    private float PlayerPosX;
    private float PlayerPosY;
    private Vector3 playerPosition;

    public Image hpBar;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;

    public void SettingOhterPlayer(string pName, int pLv, int pMaxHp, int pNowHp, int pAtk, float pPosX, float pPosY)
    {
        PlayerName = pName;
        PlayerLv = pLv;
        PlayerMaxHp = pMaxHp;
        PlayerNowHp = pNowHp;
        PlayerAtk = pAtk;
        PlayerPosX = pPosX;
        PlayerPosY = pPosY;

        StateUpdate();

    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void StateUpdate()
    {
        if (hpText != null)
        {
            hpText.text = PlayerNowHp.ToString() + " / " + PlayerMaxHp.ToString();
        }
        if (nameText != null)
        {
            nameText.text = PlayerName;
        }
        if (levelText != null)
        {
            levelText.text = "Lv " + PlayerLv.ToString();
        }
        transform.position = new Vector3(PlayerPosX, PlayerPosY, transform.position.z);

        //SQL_Connect_Manager.Instance.UpdatePlayerData(PlayerName, PlayerLv, PlayerMaxHp, PlayerNowHp, PlayerAtk, PlayerPosX, PlayerPosY, true);
    }


    public void RemovePlayer()
    {
        Destroy(gameObject);
    }
}
