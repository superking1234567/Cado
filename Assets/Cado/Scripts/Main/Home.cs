using Facebook.Unity;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Home : MonoBehaviour
{
    public MainManager mm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onbtnFBLogin()
    {
        if (FB.IsInitialized)
        {
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.HandleResult);
        }
    }

    public void onbtnLogin()
    {
        Global.screenID = 2;
        mm.btnHome.SetActive(true);
        mm.Home.GetComponent<SwipeUI>().hideUI(-1);
        mm.Login.GetComponent<SwipeUI>().showUI(1);
    }

    public void onbtnHome()
    {
        Global.screenID = 1;
        mm.btnHome.SetActive(false);
        mm.Login.GetComponent<SwipeUI>().hideUI(1);
        mm.Home.GetComponent<SwipeUI>().showUI(-1);
    }

    protected void HandleResult(IResult result)
    {
        if (result == null)
        {
            Debug.Log("Null Response\n");
            return;
        }

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error - Check log for details");
            Debug.Log("Error Response:\n" + result.Error);
        }
        else if (result.Cancelled)
        {
            Debug.Log("Cancelled - Check log for details");
            Debug.Log("Cancelled Response:\n" + result.RawResult);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("Success - Check log for details");
            Debug.Log("Success Response:\n" + result.RawResult);

            JsonData json = JsonMapper.ToObject(result.RawResult);
            string fb_user_id = json["user_id"].ToString();
            string fb_access_token = json["access_token"].ToString();

            Debug.Log("----" + fb_user_id + "----");
            Debug.Log("----" + fb_access_token + "----");

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("fb_user_id", UnityWebRequest.EscapeURL(fb_user_id)));

            string requestURL = Global.DOMAIN + "/API/IsFBUser.aspx";
            UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

            Debug.Log(requestURL + "?fb_user_id=" + UnityWebRequest.EscapeURL(fb_user_id));

            StartCoroutine(ResponseIsFBUser(www, fb_user_id, fb_access_token));
        }
        else
        {
            Debug.Log("Empty Response\n");
        }

        Debug.Log(result.ToString());
    }

    IEnumerator ResponseIsFBUser(UnityWebRequest www, string fb_user_id, string fb_access_token)
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
            mm.ShowAlertPopup("Server api error!_");
            yield break;
        }

        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response == "1")
        {
            mm.FBSignup(fb_user_id, fb_access_token);
        }
        else if(response == "2")
        {
            mm.FBLogin(fb_user_id, fb_access_token);
        }
        else
        {
            string resText = json["responseText"].ToString();
            mm.ShowAlertPopup(resText);
            yield break;
        }
    }
}
