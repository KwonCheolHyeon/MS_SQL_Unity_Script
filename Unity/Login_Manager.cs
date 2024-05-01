using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Login_Manager : MonoBehaviour
{
    public TMP_InputField loginIDInput;
    public TMP_InputField loginPasswordInput;
    public GameObject PasswordErrorPanel;

    private static Login_Manager instance;

    public static Login_Manager Instance
    {
        get
        {
            // 인스턴스가 없는 경우 새로 생성
            if (instance == null)
            {
                GameObject go = new GameObject("Login_Manager");
                instance = go.AddComponent<Login_Manager>();
            }
            return instance;
        }
    }
    void Start()
    {
        // 인스턴스 초기화
        instance = this;
    }

    // Update is called once per frame
    public void Login(string loginID, string loginPassword)
    {
        SQL_Connect_Test.Instance.Login(loginID, loginPassword);
    }

    // 로그인 버튼 클릭 시 호출
    public void OnLoginButtonClick()
    {
        string loginID = loginIDInput.text;
        string loginPassword = loginPasswordInput.text;
        // 로그인 함수 호출
        Login(loginID, loginPassword);
        loginIDInput.text = "";
        loginPasswordInput.text = "";
    }

    public void LoginPasswordError() 
    {
        PasswordErrorPanel.active = true;
    }

}
