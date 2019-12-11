using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    public MainManager mm;
    public GameObject ProductList;
    public GameObject myItemPrefab;
    public GameObject pagination;
    public GameObject btnPresent;
    public GameObject btnBin;
    public GameObject Loading;

    private bool isbtnPresentSelected = false;
    private bool isbtnBinSelected = false;

    private int curPage = 1;
    private int totalPages = 1;
    private long user_id = -1;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initUI(long user_id)
    {
        this.user_id = user_id;
        if (!isbtnPresentSelected && !isbtnBinSelected)
        {
            onbtnPresent();
        }
        else
        {
            if (isbtnPresentSelected)
            {
                present();
            }
            else if (isbtnBinSelected)
            {
                bin();
            }
        }
    }

    public void onbtnPresent()
    {
        if (isbtnPresentSelected)
        {
            return;
        }

        curPage = 1;
        present();
    }

    public void present()
    {
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

        GetProductList(user_id, 1);
    }

    public void onbtnBin()
    {
        if (isbtnBinSelected)
        {
            return;
        }

        curPage = 1;
        bin();
    }

    public void bin()
    {
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

    public void onbtnNext()
    {
        curPage++;
        if (isbtnPresentSelected)
        {
            present();
        }
        else if (isbtnBinSelected)
        {
            bin();
        }
    }

    public void onbtnPrev()
    {
        curPage--;
        if (isbtnPresentSelected)
        {
            present();
        }
        else if (isbtnBinSelected)
        {
            bin();
        }
    }

    public void GetProductList(long user_id, int rate)
    {
        foreach (Transform child in ProductList.transform)
        {
            if (child.gameObject == pagination) {
                pagination.transform.SetParent(this.transform);
                continue;
            } 
            Destroy(child.gameObject);
        }
        Resources.UnloadUnusedAssets();
        pagination.SetActive(false);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", user_id.ToString()));
        formData.Add(new MultipartFormDataSection("rate", rate.ToString()));
        formData.Add(new MultipartFormDataSection("page", curPage.ToString()));

        string requestURL = Global.DOMAIN + "/API/GetMyItemList.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        StartCoroutine(ResponseGetProductList(www, rate));
    }

    IEnumerator ResponseGetProductList(UnityWebRequest www, int rate)
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

        totalPages = int.Parse(json["totalPages"].ToString());

        Global.myItemList.Clear();
        for (int i = 0; i < json["products"].Count; i++)
        {
            Product pt = new Product();
            pt.product_id = json["products"][i]["product_id"].ToString();
            pt.title = UnityWebRequest.UnEscapeURL(json["products"][i]["title"].ToString());
            pt.description = UnityWebRequest.UnEscapeURL(json["products"][i]["description"].ToString());
            pt.price = json["products"][i]["price"].ToString();
            pt.image = UnityWebRequest.UnEscapeURL(json["products"][i]["image"].ToString());
            pt.url = UnityWebRequest.UnEscapeURL(json["products"][i]["url"].ToString());
            pt.market_id = int.Parse(json["products"][i]["market_id"].ToString());

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

                    temp.transform.Find("item" + j).GetComponent<ItemSelect>().product_id = product_id;
                    temp.transform.Find("item" + j).GetComponent<ItemSelect>().product_name = product_name;
                    temp.transform.Find("item" + j).GetComponent<ItemSelect>().market_id = market_id;
                    temp.transform.Find("item" + j + "/Text").GetComponent<Text>().text = product_name;

                    if(Global.m_user.id != user_id)
                    {
                        temp.transform.Find("item" + j + "/btnClose").gameObject.SetActive(false);
                    }

                    StartCoroutine(LoadImage(temp.transform.Find("item" + j + "/RawImage").GetComponent<RawImage>(), image));
                }
            }

            temp.transform.SetParent(ProductList.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.SetActive(true);
        }

        if(totalPages > 1)
        {
            pagination.transform.Find("Text").GetComponent<Text>().text = curPage + " of " + totalPages;

            if(curPage > 1)
            {
                pagination.transform.Find("prev").GetComponent<Button>().enabled = true;
            }
            else
            {
                pagination.transform.Find("prev").GetComponent<Button>().enabled = false;
            }

            if(curPage < totalPages)
            {
                pagination.transform.Find("next").GetComponent<Button>().enabled = true;
            }
            else
            {
                pagination.transform.Find("next").GetComponent<Button>().enabled = false;
            }

            pagination.transform.SetParent(ProductList.transform);
            pagination.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            pagination.SetActive(true);
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

    public void deleteItem()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", Global.m_user.id.ToString()));
        formData.Add(new MultipartFormDataSection("product_id", Global.myItemList[Global.selectedItemIndex].product_id.ToString()));
        formData.Add(new MultipartFormDataSection("market_id", Global.myItemList[Global.selectedItemIndex].market_id.ToString()));

        string requestURL = Global.DOMAIN + "/API/DelMyItem.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        StartCoroutine(ResponseDelMyItem(www));
    }

    IEnumerator ResponseDelMyItem(UnityWebRequest www)
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

        if(Global.myItemList.Count == 1)
        {
            curPage--;
        }

        if (isbtnPresentSelected)
        {
            present();
        }
        else if (isbtnBinSelected)
        {
            bin();
        }
    }

    public void onbtnClose()
    {
        this.gameObject.SetActive(false);
    }
}
