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
            // 인스턴스가 없는 경우 새로 생성
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go); // 씬 전환 시 파괴되지 않도록 설정
            }
            return instance;
        }
    }

    public GameObject playerPrefab;

    void Start()
    {
        // 프리팹 로드 및 변수에 할당
        //playerPrefab = Resources.Load<GameObject>("Object/PlayerPrefab.prefab");

        // instance가 null이 아닌 경우 현재 인스턴스가 GameManager 클래스에 대한 인스턴스가 되도록 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            // 현재 인스턴스가 이미 존재하므로 새로 생성된 인스턴스 파괴
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //로그인 할 때 플레이어 데이터 받아오기
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
