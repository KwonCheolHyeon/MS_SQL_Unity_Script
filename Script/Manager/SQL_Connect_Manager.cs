using UnityEngine;
using System;
using System.Data;
using System.Data.SqlClient;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
public class SQL_Connect_Manager : MonoBehaviour
{
    // MS SQL 서버 연결 문자열
    string connectionString = "비밀";

    private static SQL_Connect_Manager instance;

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

            // Execute a test query
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
                    LoginPlayerData(playerID);
                   
                    Debug.Log("로그인 성공!");
                }
                else if (loginStatus == "New ID created")
                {
                    // 새로운 ID 생성
                    SceneManager.LoadScene("GameScene");
                    GameManager.Instance.NewPlayerData(playerID);
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

    public void UpdatePlayerData(string playerID, int lv, int hp, int nowHp, int ATK, float posX, float posY)
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

                    SET @PlayerID = @PlayerIDParam;
                    SET @Lv = @LvParam;
                    SET @hp = @hpParam;
                    SET @nowHP = @nowHpParam;
                    SET @ATK = @ATKParam;
                    SET @posX = @posXParam;
                    SET @posY = @posYParam;

                    UPDATE [MetalDB].[dbo].[Players]
                    SET [Lv] = @Lv,
                        [hp] = @hp,
                        [nowHp] = @nowHP,
                        [ATK] = @ATK,
                        [posX] = @posX,
                        [posY] = @posY
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
}
