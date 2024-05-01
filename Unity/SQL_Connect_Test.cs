using UnityEngine;
using System;
using System.Data;
using System.Data.SqlClient;

public class SQL_Connect_Test : MonoBehaviour
{
    // MS SQL 서버 연결 문자열
    string connectionString = "Data Source=?;Initial Catalog=?;User ID=?;Password=?";

    private static SQL_Connect_Test instance;

    public static SQL_Connect_Test Instance
    {
        get
        {
            // 인스턴스가 없는 경우 새로 생성
            if (instance == null)
            {
                GameObject go = new GameObject("SQL_Connect_Test");
                instance = go.AddComponent<SQL_Connect_Test>();
            }
            return instance;
        }
    }

    void Start()
    {
        instance = this;
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
                    Debug.Log("로그인 성공!");
                }
                else if (loginStatus == "New ID created")
                {
                    // 새로운 ID 생성
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
}
