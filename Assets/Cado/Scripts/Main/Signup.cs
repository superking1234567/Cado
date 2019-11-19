using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signup : MonoBehaviour
{
    public MainManager mm;

    public InputField FirstName;
    public InputField LastName;
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

    public void onbtnSignup()
    {
        string firstname = FirstName.text;
        string lastname = LastName.text;
        string email = Email.text;
        string password = Password.text;

        if (string.IsNullOrEmpty(firstname))
        {
            FirstName.Select();
            FirstName.ActivateInputField();
            return;
        }

        if (string.IsNullOrEmpty(lastname))
        {
            LastName.Select();
            LastName.ActivateInputField();
            return;
        }

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

        mm.EmailSignup(firstname, lastname, email, password);
    }

    public void goToLogin()
    {
        Global.screenID = 2;
        mm.btnHome.SetActive(true);
        mm.Signup.GetComponent<SwipeUI>().hideUI(1);
        mm.Login.GetComponent<SwipeUI>().showUI(-1);
    }
}
