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

    public GameObject btnEtsy;
    public GameObject btnBestBuy;
    public GameObject btnWish;

    public GameObject LoadingBar;

    public bool isbtnEtsySelected = false;
    public bool isbtnBestBuySelected = false;
    public bool isbtnWishSelected = false;

    public MainManager mm;
    public Dashboard dashboard;

    // Start is called before the first frame update
    void Start()
    {
        initCategoryUI();
        GetCategoryList(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initCategoryUI()
    {
        Color colorV;
        ColorUtility.TryParseHtmlString("#6FCAF3", out colorV);

        ColorBlock theColor = btnEtsy.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnEtsy.GetComponent<Button>().colors = theColor;
        btnEtsy.transform.Find("Text").GetComponent<Text>().color = Color.white;
        isbtnEtsySelected = true;

        GetCategoryList(1);
    }

    public void GetCategoryList(int market_id)
    {
        foreach (Transform child in categoryList.transform)
        {
            Destroy(child.gameObject);
        }
        Resources.UnloadUnusedAssets();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("market_id", market_id.ToString()));

        string requestURL = Global.DOMAIN + "/API/GetCategoryList.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        StartCoroutine(ResponseGetCategoryList(www, market_id));
    }

    IEnumerator ResponseGetCategoryList(UnityWebRequest www, int market_id)
    {
        LoadingBar.SetActive(true);
        yield return www.SendWebRequest();
        LoadingBar.SetActive(false);
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
        for (int i=0; i< json["categories"].Count; i+=3)
        {
            temp = Instantiate(categoryPrefab) as GameObject;
            temp.SetActive(false);
            temp.transform.name = i.ToString();

            for(int j=0; j<3; j++)
            {
                if(i+j >= json["categories"].Count)
                { 
                    temp.transform.Find("item" + j).gameObject.SetActive(false);
                }
                else
                {
                    string category_id = json["categories"][i + j]["id"].ToString();
                    string category_name = json["categories"][i + j]["name"].ToString();

                    temp.transform.Find("item" + j).GetComponent<CategorySelect>().category_id = category_id;
                    temp.transform.Find("item" + j).GetComponent<CategorySelect>().category_name = category_name;
                    temp.transform.Find("item" + j).GetComponent<CategorySelect>().market_id = market_id;
                    temp.transform.Find("item" + j + "/Text").GetComponent<Text>().text = category_name;

                    int index = Global.categoryList.FindIndex(x => x.id == category_id && x.market_id == market_id);
                    if (index > -1)
                    {
                        temp.transform.Find("item" + j).GetComponent<CategorySelect>().isSelected = true;
                    }
                }
            }

            temp.transform.SetParent(categoryList.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
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

    public void onbtnEtsy()
    {
        if (isbtnEtsySelected)
        {
            return;
        }

        Color colorV;

        ColorUtility.TryParseHtmlString("#6FCAF3", out colorV);
        ColorBlock theColor = btnEtsy.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnEtsy.GetComponent<Button>().colors = theColor;
        btnEtsy.transform.Find("Text").GetComponent<Text>().color = Color.white;
        isbtnEtsySelected = true;

        ColorUtility.TryParseHtmlString("#FFFFFF", out colorV);
        theColor = btnBestBuy.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnBestBuy.GetComponent<Button>().colors = theColor;
        btnBestBuy.transform.Find("Text").GetComponent<Text>().color = Color.black;
        isbtnBestBuySelected = false;

        ColorUtility.TryParseHtmlString("#FFFFFF", out colorV);
        theColor = btnWish.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnWish.GetComponent<Button>().colors = theColor;
        btnWish.transform.Find("Text").GetComponent<Text>().color = Color.black;
        isbtnWishSelected = false;

        GetCategoryList(1);
    }

    public void onbtnBestBuy()
    {
        if (isbtnBestBuySelected)
        {
            return;
        }

        Color colorV;

        ColorUtility.TryParseHtmlString("#FFFFFF", out colorV);
        ColorBlock theColor = btnEtsy.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnEtsy.GetComponent<Button>().colors = theColor;
        btnEtsy.transform.Find("Text").GetComponent<Text>().color = Color.black;
        isbtnEtsySelected = false;

        ColorUtility.TryParseHtmlString("#6FCAF3", out colorV);
        theColor = btnBestBuy.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnBestBuy.GetComponent<Button>().colors = theColor;
        btnBestBuy.transform.Find("Text").GetComponent<Text>().color = Color.white;
        isbtnBestBuySelected = true;

        ColorUtility.TryParseHtmlString("#FFFFFF", out colorV);
        theColor = btnWish.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnWish.GetComponent<Button>().colors = theColor;
        btnWish.transform.Find("Text").GetComponent<Text>().color = Color.black;
        isbtnWishSelected = false;

        GetCategoryList(2);
    }

    public void onbtnWish()
    {

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
