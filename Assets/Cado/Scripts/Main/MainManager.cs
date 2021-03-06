﻿using Facebook.Unity;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public GameObject btnHome;
    public GameObject Splash;
    public GameObject Main;
    public GameObject Home;
    public GameObject Signup;
    public GameObject Login;
    public GameObject Question;

    public GameObject Dashboard;
    public GameObject Categories;
    public GameObject Products;
    public GameObject MyItems;
    public GameObject FriendItems;

    public GameObject AlertPopup;
    public GameObject LoadingPopup;
    public GameObject DelPopup;

    public bool isQuit = false;
    private string push_token = "test";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("showSplash");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator showSplash()
    {
        yield return new WaitForSeconds(3.0f);
        int index = 1;
        while (index <= 100)
        {
            yield return new WaitForSeconds(0.01f);
            float val = (float)index / 100.0f;
            Splash.transform.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1, 1-val);
            index++;
        }
        //yield return new WaitForSeconds(0.3f);
        FB.Init(this.OnFBInitComplete, this.OnHideUnity);
    }

    private void OnFBInitComplete()
    {
        Debug.Log("Success - Check log for details");
        Debug.Log("Success Response: OnFBInitComplete Called\n");

        string logMessage = string.Format(
            "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
            FB.IsLoggedIn,
            FB.IsInitialized);
        Debug.Log(logMessage);

        if (AccessToken.CurrentAccessToken != null)
        {
            Debug.Log(AccessToken.CurrentAccessToken.ToString());
        }

        if(FB.IsInitialized && FB.IsLoggedIn)
        {
            FBLogin(AccessToken.CurrentAccessToken.UserId, AccessToken.CurrentAccessToken.TokenString);
        }
        else
        {
            Main.GetComponent<SwipeUI>().showUI(1);
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log("Success - Check log for details");
        Debug.Log("Success Response: OnHideUnity Called {0}\n" + isGameShown);
        Debug.Log("Is game shown: " + isGameShown);
    }

    public void gotoQuestion(int uiNumber = 1)
    {
        Global.screenID = 4;
        if(uiNumber == 1)
        {//Facebook Login
            Home.GetComponent<SwipeUI>().hideUI(-1);
        }
        else if(uiNumber == 2)
        {//Email Login
            Signup.GetComponent<SwipeUI>().hideUI(-1);
        }
        Question.GetComponent<SwipeUI>().showUI(1);
    }

    public void gotoDashboard(int uiNumber = 1)
    {
        if(uiNumber == 1)
        {//Categories
            Global.screenID = 5;
            Categories.transform.localPosition = new Vector3(0, 0, 0);
            Products.transform.localPosition = new Vector3(512, 0, 0);
        }
        else if(uiNumber == 2)
        {//ProductPanel
            Global.screenID = 6;
            Global.productList = new List<Product>();
            Categories.transform.localPosition = new Vector3(512, 0, 0);
            Products.transform.localPosition = new Vector3(0, 0, 0);
        }
        Dashboard.GetComponent<SwipeUI>().showUI(1);
    }

    public void ShowAlertPopup(string strString)
    {
        AlertPopup.transform.Find("Text").GetComponent<Text>().text = strString;
        AlertPopup.SetActive(true);
    }

    public void HideAlertPopup()
    {
        if (isQuit)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        else
        {
            AlertPopup.SetActive(false);
        }
    }

    public void FBSignup(string fb_user_id, string fb_access_token)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("is_fb_login", "1"));
        formData.Add(new MultipartFormDataSection("fb_user_id", UnityWebRequest.EscapeURL(fb_user_id)));
        formData.Add(new MultipartFormDataSection("fb_access_token", UnityWebRequest.EscapeURL(fb_access_token)));

        int device_type = 0; //unknown
#if UNITY_ANDROID
        device_type = 1;
        formData.Add(new MultipartFormDataSection("device_type", "1"));
#elif UNITY_IOS
        device_type = 2;
        formData.Add(new MultipartFormDataSection("device_type", "2"));
#endif

        formData.Add(new MultipartFormDataSection("push_token", push_token));

        string requestURL = Global.DOMAIN + "/API/Signup.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        StartCoroutine(ResponseFBSignup(www, device_type));
    }

    IEnumerator ResponseFBSignup(UnityWebRequest www, int device_type)
    {
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            isQuit = true;
            ShowAlertPopup("Please confirm internet.");
            yield break;
        }

        string resultData = www.downloadHandler.text;
        if (string.IsNullOrEmpty(resultData))
        {
            ShowAlertPopup("Server api error!");
            yield break;
        }

        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            string resText = json["responseText"].ToString();
            ShowAlertPopup(resText);
            yield break;
        }

        long id = long.Parse(json["id"].ToString());
        int is_use = int.Parse(json["is_use"].ToString());
        string reg_date = json["reg_date"].ToString();

        //move to Question
        Global.m_user = new User();
        Global.m_user.id = id;
        Global.m_user.email = "";
        Global.m_user.password = "";
        Global.m_user.firstname = "";
        Global.m_user.lastname = "";
        Global.m_user.avatar = "";
        Global.m_user.question_list = "";
        Global.m_user.is_fb_login = 0;
        Global.m_user.fb_user_id = -1;
        Global.m_user.fb_access_token = "";
        Global.m_user.is_use = is_use;
        Global.m_user.last_login = DateTime.Now;
        Global.m_user.device_type = device_type;
        Global.m_user.reg_date = DateTime.Parse(reg_date);
        Global.m_user.children_id = 0;

        gotoQuestion(1);
    }

    public void FBLogin(string fb_user_id, string fb_access_token)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("is_fb_login", "1"));
        formData.Add(new MultipartFormDataSection("fb_user_id", UnityWebRequest.EscapeURL(fb_user_id)));
        formData.Add(new MultipartFormDataSection("fb_access_token", UnityWebRequest.EscapeURL(fb_access_token)));

        int device_type = 0; //unknown
#if UNITY_ANDROID
        device_type = 1;
        formData.Add(new MultipartFormDataSection("device_type", "1"));
#elif UNITY_IOS
        device_type = 2;
        formData.Add(new MultipartFormDataSection("device_type", "2"));
#endif

        formData.Add(new MultipartFormDataSection("push_token", push_token));

        string requestURL = Global.DOMAIN + "/API/Login.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        StartCoroutine(ResponseFBLogin(www, fb_user_id, fb_access_token, device_type));
    }

    IEnumerator ResponseFBLogin(UnityWebRequest www, string fb_user_id, string fb_access_token,  int device_type)
    {
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            isQuit = true;
            ShowAlertPopup("Please confirm internet.");
            yield break;
        }

        string resultData = www.downloadHandler.text;
        if (string.IsNullOrEmpty(resultData))
        {
            ShowAlertPopup("Server api error!");
            yield break;
        }

        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            string resText = json["responseText"].ToString();
            ShowAlertPopup(resText);
            yield break;
        }

        long id = long.Parse(json["id"].ToString());
        string firstname = UnityWebRequest.UnEscapeURL(json["firstname"].ToString());
        string lastname = UnityWebRequest.UnEscapeURL(json["lastname"].ToString());
        string question_list = UnityWebRequest.UnEscapeURL(json["question_list"].ToString());
        string category_list = UnityWebRequest.UnEscapeURL(json["category_list"].ToString());
        string last_login = UnityWebRequest.UnEscapeURL(json["last_login"].ToString());
        string reg_date = UnityWebRequest.UnEscapeURL(json["reg_date"].ToString());
        long children_id = long.Parse(json["children_id"].ToString());
        JsonData children_list = json["children_list"];

        Global.m_user = new User();
        Global.m_user.id = id;
        Global.m_user.email = "";
        Global.m_user.password = "";
        Global.m_user.firstname = firstname;
        Global.m_user.lastname = lastname;
        Global.m_user.avatar = "";
        Global.m_user.question_list = "";
        Global.m_user.is_fb_login = 1;
        Global.m_user.fb_user_id = long.Parse(fb_user_id);
        Global.m_user.fb_access_token = fb_access_token;
        Global.m_user.question_list = question_list;
        Global.m_user.is_use = 1;
        Global.m_user.last_login = DateTime.Parse(last_login);
        Global.m_user.device_type = device_type;
        Global.m_user.reg_date = DateTime.Parse(reg_date);
        Global.m_user.children_id = children_id;

        Global.m_childrenList.Clear();
        for(int i=0; i<children_list.Count; i++)
        {
            Children children = new Children();
            children.id = long.Parse(children_list[i]["id"].ToString());
            children.firstname = UnityWebRequest.UnEscapeURL(children_list[i]["firstname"].ToString());
            children.lastname = UnityWebRequest.UnEscapeURL(children_list[i]["lastname"].ToString());
            children.reg_date = UnityWebRequest.UnEscapeURL(children_list[i]["reg_date"].ToString());

            Global.m_childrenList.Add(children);
        }

        Global.categoryList.Clear();
        JsonData categories = JsonMapper.ToObject(category_list);
        for (int i = 0; i < categories.Count; i++)
        {
            Category ct = new Category();
            ct.id = categories[i]["category_id"].ToString();
            ct.name = categories[i]["category_name"].ToString();
            ct.market_id = int.Parse(categories[i]["market_id"].ToString());
            Global.categoryList.Add(ct);
        }

        PlayerPrefs.SetInt("auto_login", 1);
        PlayerPrefs.Save();
        Global.SaveUserInfo(Global.m_user);

        gotoDashboard(2);
    }

    public void EmailSignup(string firstname, string lastname, string email, string password)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("is_fb_login", "0"));
        formData.Add(new MultipartFormDataSection("firstname", UnityWebRequest.EscapeURL(firstname)));
        formData.Add(new MultipartFormDataSection("lastname", UnityWebRequest.EscapeURL(lastname)));
        formData.Add(new MultipartFormDataSection("email", UnityWebRequest.EscapeURL(email)));
        formData.Add(new MultipartFormDataSection("password", UnityWebRequest.EscapeURL(password)));

        int device_type = 0; //unknown
#if UNITY_ANDROID
        device_type = 1;
        formData.Add(new MultipartFormDataSection("device_type", "1"));
#elif UNITY_IOS
        device_type = 2;
        formData.Add(new MultipartFormDataSection("device_type", "2"));
#endif

        formData.Add(new MultipartFormDataSection("push_token", push_token));

        string requestURL = Global.DOMAIN + "/API/Signup.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        StartCoroutine(ResponseEmailSignup(www, firstname, lastname, email, password, device_type));
    }

    IEnumerator ResponseEmailSignup(UnityWebRequest www, string firstname, string lastname, string email, string password, int device_type)
    {
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            isQuit = true;
            ShowAlertPopup("Please confirm internet.");
            yield break;
        }

        string resultData = www.downloadHandler.text;
        if (string.IsNullOrEmpty(resultData))
        {
            ShowAlertPopup("Server api error!");
            yield break;
        }

        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            string resText = json["responseText"].ToString();
            ShowAlertPopup(resText);
            yield break;
        }

        long id = long.Parse(json["id"].ToString());
        int is_use = int.Parse(json["is_use"].ToString());
        string reg_date = json["reg_date"].ToString();

        //move to Question
        Global.m_user = new User();
        Global.m_user.id = id;
        Global.m_user.email = email;
        Global.m_user.password = password;
        Global.m_user.firstname = firstname;
        Global.m_user.lastname = lastname;
        Global.m_user.avatar = "";
        Global.m_user.question_list = "";
        Global.m_user.is_fb_login = 0;
        Global.m_user.fb_user_id = -1;
        Global.m_user.fb_access_token = "";
        Global.m_user.is_use = is_use;
        Global.m_user.last_login = DateTime.Now;
        Global.m_user.device_type = device_type;
        Global.m_user.reg_date = DateTime.Parse(reg_date);
        Global.m_user.children_id = 0;

        gotoQuestion(2);
    }

    public void EmailLogin(string email, string password)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("is_fb_login", "0"));
        formData.Add(new MultipartFormDataSection("email", UnityWebRequest.EscapeURL(email)));
        formData.Add(new MultipartFormDataSection("password", UnityWebRequest.EscapeURL(password)));

        int device_type = 0; //unknown
#if UNITY_ANDROID
        device_type = 1;
        formData.Add(new MultipartFormDataSection("device_type", "1"));
#elif UNITY_IOS
        device_type = 2;
        formData.Add(new MultipartFormDataSection("device_type", "2"));
#endif
        formData.Add(new MultipartFormDataSection("push_token", UnityWebRequest.EscapeURL(push_token)));

        string requestURL = Global.DOMAIN + "/API/Login.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        StartCoroutine(ResponseEmailLogin(www, email, password, device_type));
    }

    IEnumerator ResponseEmailLogin(UnityWebRequest www, string email, string password, int device_type)
    {
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            isQuit = true;
            ShowAlertPopup("Please confirm internet.");
            yield break;
        }

        string resultData = www.downloadHandler.text;
        if (string.IsNullOrEmpty(resultData))
        {
            ShowAlertPopup("Server api error!");
            yield break;
        }

        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            string resText = json["responseText"].ToString();
            ShowAlertPopup(resText);
            yield break;
        }

        long id = long.Parse(json["id"].ToString());
        string firstname = UnityWebRequest.UnEscapeURL(json["firstname"].ToString());
        string lastname = UnityWebRequest.UnEscapeURL(json["lastname"].ToString());
        string question_list = UnityWebRequest.UnEscapeURL(json["question_list"].ToString());
        string category_list = UnityWebRequest.UnEscapeURL(json["category_list"].ToString());
        string last_login = UnityWebRequest.UnEscapeURL(json["last_login"].ToString());
        string reg_date = UnityWebRequest.UnEscapeURL(json["reg_date"].ToString());
        long children_id = long.Parse(json["children_id"].ToString());
        JsonData children_list = json["children_list"];

        Global.m_user = new User();
        Global.m_user.id = id;
        Global.m_user.email = email;
        Global.m_user.password = password;
        Global.m_user.firstname = firstname;
        Global.m_user.lastname = lastname;
        Global.m_user.avatar = "";
        Global.m_user.question_list = "";
        Global.m_user.is_fb_login = 0;
        Global.m_user.fb_user_id = -1;
        Global.m_user.fb_access_token = "";
        Global.m_user.question_list = question_list;
        Global.m_user.is_use = 1;
        Global.m_user.last_login = DateTime.Parse(last_login);
        Global.m_user.device_type = device_type;
        Global.m_user.reg_date = DateTime.Parse(reg_date);
        Global.m_user.children_id = children_id;

        Global.m_childrenList.Clear();
        for (int i = 0; i < children_list.Count; i++)
        {
            Children children = new Children();
            children.id = long.Parse(children_list[i]["id"].ToString());
            children.firstname = UnityWebRequest.UnEscapeURL(children_list[i]["firstname"].ToString());
            children.lastname = UnityWebRequest.UnEscapeURL(children_list[i]["lastname"].ToString());
            children.reg_date = UnityWebRequest.UnEscapeURL(children_list[i]["reg_date"].ToString());

            Global.m_childrenList.Add(children);
        }

        Global.categoryList.Clear();
        JsonData categories = JsonMapper.ToObject(category_list);
        for (int i=0; i< categories.Count; i++)
        {
            Category ct = new Category();
            ct.id = categories[i]["category_id"].ToString();
            ct.name = categories[i]["category_name"].ToString();
            ct.market_id = int.Parse(categories[i]["market_id"].ToString());
            Global.categoryList.Add(ct);
        }

        PlayerPrefs.SetInt("auto_login", 1);
        PlayerPrefs.Save();
        Global.SaveUserInfo(Global.m_user);

        gotoDashboard(2);
    }

    public void showLoading()
    {
        LoadingPopup.SetActive(true);
    }

    public void hideLoading()
    {
        LoadingPopup.SetActive(false);
    }

    public void ShowDelPopup(string strString)
    {
        DelPopup.transform.Find("Text").GetComponent<Text>().text = strString;
        DelPopup.SetActive(true);
    }

    public void HideDelPopup()
    {
        DelPopup.SetActive(false);
    }

    public void onbtnDelOK()
    {
        if(Global.screenID == 7)
        {//MyItem
            MyItems.transform.GetComponent<Items>().deleteItem();
        }
        HideDelPopup();
    }
}
