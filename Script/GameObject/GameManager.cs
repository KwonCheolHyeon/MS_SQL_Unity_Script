using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        // ������ �ε� �� ������ �Ҵ�
        //playerPrefab = Resources.Load<GameObject>("Object/PlayerPrefab.prefab");

        // instance�� null�� �ƴ� ��� ���� �ν��Ͻ��� GameManager Ŭ������ ���� �ν��Ͻ��� �ǵ��� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            // ���� �ν��Ͻ��� �̹� �����ϹǷ� ���� ������ �ν��Ͻ� �ı�
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
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
        player.GetComponent<PlayerScript>().NewSettingPlayer(PlayerId);

    }

}
