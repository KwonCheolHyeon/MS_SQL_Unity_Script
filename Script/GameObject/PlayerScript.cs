using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private string PlayerName;
    public string GetPlayerName() { return PlayerName; }
    private int PlayerLv;
    private int PlayerMaxHp;
    private int PlayerNowHp;
    private int PlayerAtk;
    private float PlayerPosX;
    private float PlayerPosY;

    public Image hpBar;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;

    public float moveSpeed = 5f;
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
        transform.position = new Vector3(PlayerPosX, PlayerPosY, transform.position.z);
        InvokeRepeating("StateUpdate", 0.1f, 0.1f);
        if (nameText != null)
        {
            nameText.text = PlayerName;
        }
        //StateUpdate();
    }

    
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �̵� ���� ���
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        // �÷��̾� �̵�
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    //���� ������Ʈ �Ѳ����� ó��
    void StateUpdate() 
    {
        if (hpText != null)
        {
            hpText.text = PlayerNowHp.ToString() + " / " + PlayerMaxHp.ToString();
        }
       
        if (levelText != null)
        {
            levelText.text = "Lv " + PlayerLv.ToString();
        }

        //������ ����
        SQL_Connect_Manager.Instance.UpdatePlayerData(PlayerName, PlayerLv,PlayerMaxHp,PlayerNowHp,PlayerAtk, transform.position.x, transform.position.y, true);
    }

    private void OnApplicationQuit()
    {
        SQL_Connect_Manager.Instance.UpdatePlayerData(nameText.text, PlayerLv, PlayerMaxHp, PlayerNowHp, PlayerAtk, transform.position.x, transform.position.y, false);
    }

}
