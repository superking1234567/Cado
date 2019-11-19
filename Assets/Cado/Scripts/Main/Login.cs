using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public MainManager mm;

    public InputField Email;
    public InputField Password;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onbtnLogin()
    {
        string email = Email.text;
        string password = Password.text;

        if (string.IsNullOrEmpty(email))
        {
            Email.Select();
            Email.ActivateInputField();
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Password.Select();
            Password.GetComponent<InputField>().ActivateInputField();
            return;
        }

        mm.EmailLogin(email, password);
    }

    public void goToSignup()
    {
        Global.screenID = 3;
        mm.btnHome.SetActive(false);
        mm.Login.GetComponent<SwipeUI>().hideUI(-1);
        mm.Signup.GetComponent<SwipeUI>().showUI(1);
    }
}
