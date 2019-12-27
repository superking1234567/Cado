using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChildrenSelect : MonoBehaviour
{
    public long children_id = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnItemSelected()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", Global.m_user.id.ToString()));
        formData.Add(new MultipartFormDataSection("children_id", children_id.ToString()));

        string requestURL = Global.DOMAIN + "/API/UpdateUser.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        StartCoroutine(ResponseUpdateUser(www, children_id));
    }

    IEnumerator ResponseUpdateUser(UnityWebRequest www, long children_id)
    {
        yield return www.SendWebRequest();

        MainManager mm = GameObject.Find("MainManager").GetComponent<MainManager>();
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

        Global.m_user.children_id = children_id;
        mm.Dashboard.GetComponent<Dashboard>().onbtnHome();
    }
}
