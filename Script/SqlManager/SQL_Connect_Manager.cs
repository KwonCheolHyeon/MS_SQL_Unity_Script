using UnityEngine;
using System;
using System.Data;
using System.Data.SqlClient;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
public class SQL_Connect_Manager : MonoBehaviour
{
    // MS SQL 서버 연결 문자열
    string connectionString = "";
    
    private static SQL_Connect_Manager instance;
    private List<string> currentOnlinePlayers = new List<string>();
    private string MyPlayerID;
    public static SQL_Connect_Manager Instance
    {
        get
        {
            // 인스턴스가 없는 경우 새로 생성
            if (instance == null)
            {
                GameObject go = new GameObject("SQL_Connect_Test");
                instance = go.AddComponent<SQL_Connect_Manager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    IEnumerator StartOnlineCoroutine()
    {
        // 1.1초 대기 후에 코루틴 시작
        yield return new WaitForSeconds(1.1f);
        StartCoroutine(Online());
    }

    IEnumerator Online()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (true)
        {
            CheckOnlineStatus();
            yield return wait;
        }
    }
    void Start()
    {
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

        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
            Debug.Log("Connected to MS SQL Server");

            SqlCommand command = new SqlCommand("SELECT 1", connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Debug.Log("Test query executed successfully");
            }
            else
            {
                Debug.LogError("Test query failed");
            }
            reader.Close();

            connection.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error connecting to MS SQL Server: " + ex.Message);
            Login_Manager.Instance.ServerError(ex);
        }
        finally
        {
            // 연결 해제
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }


    public void Login(string playerID, string password)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("PlayerLogin", connection);
                command.CommandType = CommandType.StoredProcedure;

                // 파라미터 추가
                command.Parameters.AddWithValue("@PlayerID", playerID);
                command.Parameters.AddWithValue("@Password", password);

                // 로그인 결과 받기
                string loginStatus = (string)command.ExecuteScalar();
                Debug.Log("Login Status: " + loginStatus);

                // 로그인 성공 여부에 따라 처리
                if (loginStatus == "Login successful")
                {
                    // 로그인 성공
                    SceneManager.LoadScene("GameScene");
                    MyPlayerID = playerID;
                    LoginPlayerData(playerID);
                    StartCoroutine(StartOnlineCoroutine()); // Start the coroutine here
                    Debug.Log("로그인 성공!");
                }
                else if (loginStatus == "New ID created")
                {
                    // 새로운 ID 생성
                    SceneManager.LoadScene("GameScene");
                    GameManager.Instance.NewPlayerData(playerID);
                    MyPlayerID = playerID;
                    StartCoroutine(StartOnlineCoroutine()); // Start the coroutine here
                    Debug.Log("새로운 ID가 생성되었습니다!");
                }
                else
                {
                    Login_Manager.Instance.LoginPasswordError();
                }
            }
            catch (SqlException ex)
            {
                Debug.LogError("SQL 오류: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.LogError("오류: " + ex.Message);
            }
        }
    }


    public void LoginPlayerData(string playerID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("GetPlayerData", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@PlayerID", playerID);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int Lv = reader.GetInt32(reader.GetOrdinal("Lv"));
                        int hp = reader.GetInt32(reader.GetOrdinal("hp"));
                        int nowHP = reader.GetInt32(reader.GetOrdinal("nowHP"));
                        int ATK = reader.GetInt32(reader.GetOrdinal("ATK"));
                        float posX = (float)reader.GetDouble(reader.GetOrdinal("posX"));
                        float posY = (float)reader.GetDouble(reader.GetOrdinal("posY"));

                        GameManager.Instance.GetLoginPlayerData(playerID, Lv,  hp,nowHP,ATK,posX,posY);
                        Debug.Log("Lv: " + Lv + ", hp: " + hp + ", nowHP: " + nowHP + ", ATK: " + ATK + ", posX: " + posX + ", posY: " + posY);
                    }
                }
                else
                {
                    Debug.Log("No rows found.");
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving player data: " + ex.Message);
        }

    }

    public void UpdatePlayerData(string playerID, int lv, int hp, int nowHp, int ATK, float posX, float posY, bool onlineStatus)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(@"
                DECLARE @PlayerID NVARCHAR(50);
                DECLARE @Lv INT;
                DECLARE @hp INT;
                DECLARE @nowHP INT;
                DECLARE @ATK INT;
                DECLARE @posX FLOAT;
                DECLARE @posY FLOAT;
                DECLARE @OnlineStatus BIT; -- Assuming BIT data type for online status

                SET @PlayerID = @PlayerIDParam;
                SET @Lv = @LvParam;
                SET @hp = @hpParam;
                SET @nowHP = @nowHpParam;
                SET @ATK = @ATKParam;
                SET @posX = @posXParam;
                SET @posY = @posYParam;
                SET @OnlineStatus = @OnlineStatusParam;

                UPDATE [MetalDB].[dbo].[Players]
                SET [Lv] = @Lv,
                    [hp] = @hp,
                    [nowHp] = @nowHP,
                    [ATK] = @ATK,
                    [posX] = @posX,
                    [posY] = @posY,
                    [OnlineStatus] = @OnlineStatus
                WHERE [PlayerID] = @PlayerID;
                
                WITH RankedPlayers AS (
                    SELECT [PlayerID], [Lv], [Ranking],
                        ROW_NUMBER() OVER(PARTITION BY [Lv] ORDER BY [Ranking] ASC) AS LvRank
                    FROM [MetalDB].[dbo].[Players]
                )
                UPDATE RankedPlayers
                SET [Ranking] = LvRank;
            ", connection);

                command.Parameters.AddWithValue("@PlayerIDParam", playerID);
                command.Parameters.AddWithValue("@LvParam", lv);
                command.Parameters.AddWithValue("@hpParam", hp);
                command.Parameters.AddWithValue("@nowHpParam", nowHp);
                command.Parameters.AddWithValue("@ATKParam", ATK);
                command.Parameters.AddWithValue("@posXParam", posX);
                command.Parameters.AddWithValue("@posYParam", posY);
                command.Parameters.AddWithValue("@OnlineStatusParam", onlineStatus);

                int rowsAffected = command.ExecuteNonQuery();
                Debug.Log("Rows Affected: " + rowsAffected);

                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error updating player data: " + ex.Message);
        }
    }

    //해당 함수는 로그인 후 사용
    void CheckOnlineStatus()
    {
        try
        {
            List<string> newOnlinePlayers = GetNewOnlinePlayerIDs(MyPlayerID);


            foreach (string playerID in newOnlinePlayers)
            {
                if (!currentOnlinePlayers.Contains(playerID))
                {

                    SummonPlayer(playerID);
                    currentOnlinePlayers.Add(playerID);
                }
            }


            foreach (string playerID in currentOnlinePlayers)
            {
                
                UpdatePlayerStatus(playerID);
            }


            List<string> playersToRemove = new List<string>();
            foreach (string playerID in currentOnlinePlayers)
            {
                if (!newOnlinePlayers.Contains(playerID))
                {
                    playersToRemove.Add(playerID);
                }
            }
            foreach (string playerID in playersToRemove)
            {
                currentOnlinePlayers.Remove(playerID);
                RemovePlayer(playerID);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error checking online status: " + ex.Message);
        }
    }

    List<string> GetNewOnlinePlayerIDs(string myPlayerID)
    {
        List<string> newOnlinePlayers = new List<string>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand("SELECT PlayerID FROM Players WHERE OnlineStatus = 1 AND PlayerID != @MyPlayerID", connection);
            command.Parameters.AddWithValue("@MyPlayerID", myPlayerID);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string playerID = reader.GetString(0);
                newOnlinePlayers.Add(playerID);
            }

            reader.Close();
        }

        return newOnlinePlayers;
    }

    void SummonPlayer(string playerID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("GetPlayerData", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@PlayerID", playerID);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int Lv = reader.GetInt32(reader.GetOrdinal("Lv"));
                        int hp = reader.GetInt32(reader.GetOrdinal("hp"));
                        int nowHP = reader.GetInt32(reader.GetOrdinal("nowHP"));
                        int ATK = reader.GetInt32(reader.GetOrdinal("ATK"));
                        float posX = (float)reader.GetDouble(reader.GetOrdinal("posX"));
                        float posY = (float)reader.GetDouble(reader.GetOrdinal("posY"));

                        GameManager.Instance.GetOtherPlayerData(playerID, Lv, hp, nowHP, ATK, posX, posY);
                    }
                }
                else
                {
                    Debug.Log("No rows found.");
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving player data: " + ex.Message);
        }
    }

    void UpdatePlayerStatus(string playerID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("GetPlayerData", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@PlayerID", playerID);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int Lv = reader.GetInt32(reader.GetOrdinal("Lv"));
                        int hp = reader.GetInt32(reader.GetOrdinal("hp"));
                        int nowHP = reader.GetInt32(reader.GetOrdinal("nowHP"));
                        int ATK = reader.GetInt32(reader.GetOrdinal("ATK"));
                        float posX = (float)reader.GetDouble(reader.GetOrdinal("posX"));
                        float posY = (float)reader.GetDouble(reader.GetOrdinal("posY"));

                        GameManager.Instance.UpdateOtherPlayers(playerID, Lv, hp, nowHP, ATK, posX, posY);
                    }
                }
                else
                {
                    Debug.Log("No rows found.");
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving player data: " + ex.Message);
        }
    }

    void RemovePlayer(string playerID)
    {
        GameManager.Instance.DestroyOtherPlayer(playerID);
    }
}
