using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private string PlayerName;
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

    //�α��ν� ���
    public void LoginSettingPlayer(string pName, int pLv, int pMaxHp,int pNowHp, int pAtk, float pPosX,float pPosY) 
    {
        PlayerName = pName;
        PlayerLv = pLv;
        PlayerMaxHp = pMaxHp;
        PlayerNowHp = pNowHp;
        PlayerAtk = pAtk;
        PlayerPosX = pPosX;
        PlayerPosY = pPosY;
    }
    //���ο� ���� ������ ���
    public void NewSettingPlayer(string playerName) 
    {
        PlayerName = playerName;
        PlayerLv = 1;
        PlayerMaxHp = 10;
        PlayerNowHp = 10;
        PlayerAtk = 1;
        PlayerPosX = 0;
        PlayerPosY = 0;
    }

    void Start()
    {
        playerPosition.x = PlayerPosX;
        playerPosition.y = PlayerPosY;
        playerPosition.z = 0.0f;

        StateUpdate();
    }

    
    void Update()
    {
        
    }

    //���� ������Ʈ �Ѳ����� ó��
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

        //������ ����
        SQL_Connect_Manager.Instance.UpdatePlayerData(PlayerName, PlayerLv,PlayerMaxHp,PlayerNowHp,PlayerAtk,PlayerPosX,PlayerPosY);

    }

   

}
