using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            // �ν��Ͻ��� ���� ��� ���� ����
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go); // �� ��ȯ �� �ı����� �ʵ��� ����
            }
            return instance;
        }
    }

    public GameObject playerPrefab;

    private List<OtherPlayer_Script> otherPlayerComponents;
    //private List<GameObject> otherPlayerPool;
    //private int otherPlayerPoolCount = 10;

    void Start()
    {

        // instance�� null�� �ƴ� ��� ���� �ν��Ͻ��� GameManager Ŭ������ ���� �ν��Ͻ��� �ǵ��� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            
            Destroy(gameObject);
        }

        //�ٸ� �÷��̾�鸸 �̸� ����� ����
        otherPlayerComponents = new List<OtherPlayer_Script>();
        //otherPlayerPool = new List<GameObject>();
        //for (int i = 0; i < otherPlayerPoolCount; i++)
        //{
        //    GameObject player = Instantiate(playerPrefab);
        //    player.SetActive(false);
        //    player.GetComponent<PlayerScript>().enabled = false;
        //    otherPlayerPool.Add(player);
        //}

    }

    void Update()
    {
        
    }

    //�α��� �� �� �÷��̾� ������ �޾ƿ���
    public void GetLoginPlayerData(string PlayerID, int PlayerLv, int PlayerMaxHp,int PlayerNowHp,int PlayerAtk,float PlayerPosX,float PlayerPosY) 
    {
        StartCoroutine(DelayedLoginPlayer(PlayerID, PlayerLv, PlayerMaxHp, PlayerNowHp, PlayerAtk, PlayerPosX, PlayerPosY));
    }

    IEnumerator DelayedLoginPlayer(string pName, int pLv, int pMaxHp, int pNowHp, int pAtk, float pPosX, float pPosY)
    {
        yield return new WaitForSeconds(1f);
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<OtherPlayer_Script>().enabled = false;
        player.GetComponent<PlayerScript>().LoginSettingPlayer(pName, pLv, pMaxHp, pNowHp, pAtk, pPosX, pPosY);
    }


    public void NewPlayerData(string PlayerID)
    {
        StartCoroutine(DelayedNewPlayer(PlayerID));
    }

    IEnumerator DelayedNewPlayer(string PlayerId)
    {
        yield return new WaitForSeconds(1f);
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<OtherPlayer_Script>().enabled = false;
        player.GetComponent<PlayerScript>().NewSettingPlayer(PlayerId);

    }



    public void GetOtherPlayerData(string PlayerID, int PlayerLv, int PlayerMaxHp, int PlayerNowHp, int PlayerAtk, float PlayerPosX, float PlayerPosY)
    {
        StartCoroutine(DelayOtherPlayer(PlayerID, PlayerLv, PlayerMaxHp, PlayerNowHp, PlayerAtk, PlayerPosX, PlayerPosY));
    }

    IEnumerator DelayOtherPlayer(string pName, int pLv, int pMaxHp, int pNowHp, int pAtk, float pPosX, float pPosY)
    {
        yield return new WaitForSeconds(1f);
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<PlayerScript>().enabled = false;
        player.GetComponent<OtherPlayer_Script>().SettingOhterPlayer(pName, pLv, pMaxHp, pNowHp, pAtk, pPosX, pPosY);
        otherPlayerComponents.Add(player.GetComponent<OtherPlayer_Script>());
    }
    public void UpdateOtherPlayers(string pName, int pLv, int pMaxHp, int pNowHp, int pAtk, float pPosX, float pPosY) 
    {
        foreach (OtherPlayer_Script playerID in otherPlayerComponents) 
        {
            if (playerID.GetPlayerName() == pName) 
            {
                playerID.SettingOhterPlayer(pName, pLv, pMaxHp, pNowHp, pAtk, pPosX, pPosY);
            }
        }
    }

    public void DestroyOtherPlayer(string pName) 
    {
        for (int i = 0; i < otherPlayerComponents.Count; i++)
        {
            OtherPlayer_Script playerID = otherPlayerComponents[i];
            if (playerID.GetPlayerName() == pName)
            {
                otherPlayerComponents.RemoveAt(i);
                playerID.RemovePlayer();
                break; // �÷��̾ ã�����Ƿ� ������ �����մϴ�.
            }
        }
    }

}
