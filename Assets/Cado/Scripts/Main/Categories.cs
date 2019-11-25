using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Categories : MonoBehaviour
{
    public GameObject categoryList;
    public GameObject categoryPrefab;

    public MainManager mm;
    public Dashboard dashboard;

    // Start is called before the first frame update
    void Start()
    {
        GetCategoryList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCategoryList()
    {
        foreach (Transform child in categoryList.transform)
        {
            Destroy(child.gameObject);
        }
        Resources.UnloadUnusedAssets();

        string requestURL = Global.DOMAIN + "/API/GetCategoryList.aspx";
        UnityWebRequest www = UnityWebRequest.Get(requestURL);
        StartCoroutine(ResponseGetCategoryList(www));
    }

    IEnumerator ResponseGetCategoryList(UnityWebRequest www)
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

        GameObject temp;
        for (int i=0; i< json["etsy_categories"].Count; i+=3)
        {
            temp = Instantiate(categoryPrefab) as GameObject;
            temp.SetActive(false);
            temp.transform.name = i.ToString();

            for(int j=0; j<3; j++)
            {
                if(i+j >= json["etsy_categories"].Count)
                { 
                    temp.transform.Find("item" + j).gameObject.SetActive(false);
                }
                else
                {
                    string category_id = json["etsy_categories"][i + j]["id"].ToString();
                    string category_name = json["etsy_categories"][i + j]["name"].ToString();

                    temp.transform.Find("item" + j).GetComponent<CategorySelect>().category_id = long.Parse(category_id);
                    temp.transform.Find("item" + j).GetComponent<CategorySelect>().category_name = category_name;
                    temp.transform.Find("item" + j).GetComponent<CategorySelect>().market_id = 1;
                    temp.transform.Find("item" + j + "/Text").GetComponent<Text>().text = category_name;

                    temp.transform.SetParent(categoryList.transform);
                    temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
            temp.SetActive(true);

        }

    }

    public void onbtnCategorySave()
    {
        if(Global.categoryList.Count == 0)
        {
            mm.ShowAlertPopup("Please select a category.");
            return;
        }

        SetCategoryList();
    }

    public void SetCategoryList()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        string jsonData = JsonMapper.ToJson(Global.categoryList.ToArray());
        formData.Add(new MultipartFormDataSection("user_id", Global.m_user.id.ToString()));
        formData.Add(new MultipartFormDataSection("data", UnityWebRequest.EscapeURL(jsonData)));

        string requestURL = Global.DOMAIN + "/API/SetCategoryList.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        StartCoroutine(ResponseSetCategoryList(www));
    }

    IEnumerator ResponseSetCategoryList(UnityWebRequest www)
    {
        mm.showLoading();
        yield return www.SendWebRequest();
        mm.hideLoading();

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

        dashboard.gotoProductPanel();
    }
}
