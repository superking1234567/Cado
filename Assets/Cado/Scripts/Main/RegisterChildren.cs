using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RegisterChildren : MonoBehaviour
{
    public MainManager mm;
    public InputField FirstName;
    public InputField LastName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initUI()
    {
        FirstName.text = "";
        LastName.text = "";
    }

    public void onbtnAddChildren()
    {
        string firstname = FirstName.text;
        string lastname = LastName.text;

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

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", Global.m_user.id.ToString()));
        formData.Add(new MultipartFormDataSection("firstname", UnityWebRequest.EscapeURL(firstname)));
        formData.Add(new MultipartFormDataSection("lastname", UnityWebRequest.EscapeURL(lastname)));

        string requestURL = Global.DOMAIN + "/API/AddChildren.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        StartCoroutine(ResponseAddChildren(www, firstname, lastname));

    }

    IEnumerator ResponseAddChildren(UnityWebRequest www, string firstname, string lastname)
    {
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            mm.isQuit = true;
            mm.ShowAlertPopup("Please confirm internet.");
            yield break;
        }

        string resultData = www.downloadHandler.text;
        if (string.IsNullOrEmpty(resultData))
        {
            mm.ShowAlertPopup("Server api error!");
            yield break;
        }

        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            string resText = json["responseText"].ToString();
            mm.ShowAlertPopup(resText);
            yield break;
        }

        Children children = new Children();
        children.id = long.Parse(json["children_id"].ToString());
        children.firstname = firstname;
        children.lastname = lastname;
        children.reg_date = json["reg_date"].ToString();

        Global.m_childrenList.Add(children);


        mm.Categories.GetComponent<Categories>().btnClose.SetActive(false);
        mm.Categories.GetComponent<Categories>().isChildrenCategory = true;
        mm.Categories.GetComponent<Categories>().children_id = children.id;
        mm.Categories.GetComponent<Categories>().GetCategoryList(1);
        mm.Categories.transform.localPosition = new Vector3(0, 0, 0);
    }
}
