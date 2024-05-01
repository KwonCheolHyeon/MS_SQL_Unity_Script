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
            // �ν��Ͻ��� ���� ��� ���� ����
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
        // �ν��Ͻ� �ʱ�ȭ
        instance = this;
    }

    // Update is called once per frame
    public void Login(string loginID, string loginPassword)
    {
        SQL_Connect_Test.Instance.Login(loginID, loginPassword);
    }

    // �α��� ��ư Ŭ�� �� ȣ��
    public void OnLoginButtonClick()
    {
        string loginID = loginIDInput.text;
        string loginPassword = loginPasswordInput.text;
        // �α��� �Լ� ȣ��
        Login(loginID, loginPassword);
        loginIDInput.text = "";
        loginPasswordInput.text = "";
    }

    public void LoginPasswordError() 
    {
        PasswordErrorPanel.active = true;
    }

}
