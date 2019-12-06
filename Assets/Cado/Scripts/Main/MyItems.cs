using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyItems : MonoBehaviour
{
    public MainManager mm;
    public GameObject ProductList;
    public GameObject myItemPrefab;
    public GameObject btnPresent;
    public GameObject btnBin;
    public GameObject Loading;

    private bool isbtnPresentSelected = false;
    private bool isbtnBinSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        //initUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initUI()
    {
        Color colorV;
        ColorUtility.TryParseHtmlString("#6FCAF3", out colorV);

        ColorBlock theColor = btnPresent.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnPresent.GetComponent<Button>().colors = theColor;
        isbtnPresentSelected = true;
    }

    public void onbtnPresent()
    {
        if (isbtnPresentSelected)
        {
            return;
        }

        Color colorV;

        ColorUtility.TryParseHtmlString("#6FCAF3", out colorV);
        ColorBlock theColor = btnPresent.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnPresent.GetComponent<Button>().colors = theColor;
        isbtnPresentSelected = true;

        ColorUtility.TryParseHtmlString("#FFFFFF", out colorV);
        theColor = btnBin.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnBin.GetComponent<Button>().colors = theColor;
        isbtnBinSelected = false;

        GetProductList(Global.m_user.id, 1);
    }

    public void onbtnBin()
    {
        if (isbtnBinSelected)
        {
            return;
        }

        Color colorV;

        ColorUtility.TryParseHtmlString("#FFFFFF", out colorV);
        ColorBlock theColor = btnPresent.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnPresent.GetComponent<Button>().colors = theColor;
        isbtnPresentSelected = false;

        ColorUtility.TryParseHtmlString("#6FCAF3", out colorV);
        theColor = btnBin.GetComponent<Button>().colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnBin.GetComponent<Button>().colors = theColor;
        isbtnBinSelected = true;

        GetProductList(Global.m_user.id, 2);
    }

    public void GetProductList(long user_id, int type)
    {
        foreach (Transform child in ProductList.transform)
        {
            Destroy(child.gameObject);
        }
        Resources.UnloadUnusedAssets();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", user_id.ToString()));
        formData.Add(new MultipartFormDataSection("type", type.ToString()));

        string requestURL = Global.DOMAIN + "/API/GetMyItemList.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        StartCoroutine(ResponseGetProductList(www));
    }

    IEnumerator ResponseGetProductList(UnityWebRequest www)
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

        Global.myItemList.Clear();
        for (int i = 0; i < json["products"].Count; i++)
        {
            Product pt = new Product();
            pt.product_id = json["products"][i]["product_id"].ToString();
            pt.title = UnityWebRequest.UnEscapeURL(json["products"][i]["title"].ToString());
            pt.image = UnityWebRequest.UnEscapeURL(json["products"][i]["image"].ToString());
            pt.market_id = int.Parse(json["products"][i]["market_id"].ToString());
            pt.url = json["products"][i]["url"].ToString();

            Global.myItemList.Add(pt);
        }

        StartCoroutine(LoadProducts());
    }

    IEnumerator LoadProducts()
    {
        yield return null;

        GameObject temp;
        for (int i = 0; i < Global.myItemList.Count; i += 4)
        {
            temp = Instantiate(myItemPrefab) as GameObject;
            temp.SetActive(false);
            temp.transform.name = i.ToString();

            for (int j = 0; j < 4; j++)
            {
                if (i + j >= Global.myItemList.Count)
                {
                    temp.transform.Find("item" + j).gameObject.SetActive(false);
                }
                else
                {
                    string product_id = Global.myItemList[i + j].product_id;
                    string product_name = Global.myItemList[i + j].title;
                    string image = Global.myItemList[i + j].image;
                    int market_id = Global.myItemList[i + j].market_id;

                    temp.transform.Find("item" + j).GetComponent<MyItemSelect>().product_id = product_id;
                    temp.transform.Find("item" + j).GetComponent<MyItemSelect>().product_name = product_name;
                    temp.transform.Find("item" + j).GetComponent<MyItemSelect>().market_id = market_id;
                    temp.transform.Find("item" + j + "/Text").GetComponent<Text>().text = product_name;

                    StartCoroutine(LoadImage(temp.transform.Find("item" + j + "/RawImage").GetComponent<RawImage>(), image));
                }
            }

            temp.transform.SetParent(ProductList.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.SetActive(true);
        }
    }

    IEnumerator LoadImage(RawImage rawImage, string image)
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
