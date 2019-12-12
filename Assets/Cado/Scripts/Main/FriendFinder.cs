using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendFinder : MonoBehaviour
{
    public MainManager mm;
    public GameObject FriendList;
    public GameObject friendPrefab;
    public InputField tbxSearch;
    public GameObject Loading;

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
        tbxSearch.text = "";
        foreach (Transform child in FriendList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void onSearch()
    {
        string search_key = tbxSearch.text;
        if (string.IsNullOrEmpty(search_key))
        {
            return;
        }

        foreach (Transform child in FriendList.transform)
        {
            Destroy(child.gameObject);
        }
        Resources.UnloadUnusedAssets();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", Global.m_user.id.ToString()));
        formData.Add(new MultipartFormDataSection("search_key", search_key));

        string requestURL = Global.DOMAIN + "/API/GetUserList.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        StartCoroutine(ResponseGetUserList(www));
    }

    IEnumerator ResponseGetUserList(UnityWebRequest www)
    {
        Loading.SetActive(true);
        yield return www.SendWebRequest();
        Loading.SetActive(false);
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

        Global.friendList.Clear();
        for (int i = 0; i < json["users"].Count; i++)
        {
            User user = new User();
            user.id = long.Parse(json["users"][i]["id"].ToString());
            user.email = UnityWebRequest.UnEscapeURL(json["users"][i]["email"].ToString());
            user.firstname = UnityWebRequest.UnEscapeURL(json["users"][i]["firstname"].ToString());
            user.lastname = UnityWebRequest.UnEscapeURL(json["users"][i]["lastname"].ToString());
            user.avatar = UnityWebRequest.UnEscapeURL(json["users"][i]["avatar"].ToString());

            Global.friendList.Add(user);
        }

        StartCoroutine(LoadFriends());
    }

    IEnumerator LoadFriends()
    {
        yield return null;

        GameObject temp;
        for (int i = 0; i < Global.friendList.Count; i ++)
        {
            temp = Instantiate(friendPrefab) as GameObject;
            temp.SetActive(false);
            temp.transform.name = i.ToString();

            if (!string.IsNullOrEmpty(Global.friendList[i].avatar))
            {
                StartCoroutine(LoadAvatar(temp.transform.Find("RawImage").GetComponent<RawImage>(), Global.friendList[i].avatar));
            }
            temp.transform.GetComponent<FriendSelect>().user_id = Global.friendList[i].id;
            temp.transform.Find("Text").GetComponent<Text>().text = Global.friendList[i].firstname + " " + Global.friendList[i].lastname;

            temp.transform.SetParent(FriendList.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.SetActive(true);
        }
    }

    IEnumerator LoadAvatar(RawImage rawImage, string image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(image);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield break;
        }
        rawImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    }
}
