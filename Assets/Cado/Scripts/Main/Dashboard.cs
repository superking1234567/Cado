using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{
    public MainManager mm;

    public GameObject Categories;
    public GameObject Products;
    public GameObject MyItems;
    public GameObject FriendFinder;
    public GameObject Settings;

    public GameObject Menu;
    public GameObject[] menuItems;

    public GameObject DetailPopup;
    public ProductDetail pd;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gotoProductPanel()
    {
        Global.screenID = 6;
        Global.productList = new List<Product>();
        Categories.GetComponent<SwipeUI>().hideUI(-1);
        Products.GetComponent<SwipeUI>().showUI(1);
    }

    public void onbtnMenu()
    {
        Color colorV;
        ColorUtility.TryParseHtmlString("#64CAFF", out colorV);
        Color colorWhite;
        ColorUtility.TryParseHtmlString("#FFFFFF", out colorWhite);

        ColorBlock theColor;

        //6: Products(Home), 7: My Items, 8: Friend Finder, 9: Calendar, 10: Notifications, 11: Settings
        string selectedItemName = "btnHome";
        if (Global.screenID == 6)
        {
            selectedItemName = "btnHome";
        }
        else if (Global.screenID == 7)
        {
            selectedItemName = "btnMyItems";
        }
        else if (Global.screenID == 8)
        {
            selectedItemName = "btnFriendFinder";
        }
        else if (Global.screenID == 9)
        {
            selectedItemName = "btnCalendar";
        }
        else if (Global.screenID == 10)
        {
            selectedItemName = "btnNotifications";
        }
        else if (Global.screenID == 11)
        {
            selectedItemName = "btnSettings";
        }

        for (int i=0; i < menuItems.Length; i++)
        {
            if(menuItems[i].name == selectedItemName)
            {//Products(Home)
                theColor = menuItems[i].GetComponent<Button>().colors;
                theColor.normalColor = colorV;
                menuItems[i].GetComponent<Button>().colors = theColor;
            }
            else
            {
                theColor = menuItems[i].GetComponent<Button>().colors;
                theColor.normalColor = colorWhite;
                menuItems[i].GetComponent<Button>().colors = theColor;
            }
        }

        showMenuPopup();
    }

    public void onbtnHome()
    {
        int oldUI = Global.screenID;
        int newUI = 6;
        if (oldUI == newUI)
            return;

        gotoScreenUI(oldUI, newUI);
        hideMenuPopup();
    }

    public void onbtnMyItems()
    {
        int oldUI = Global.screenID;
        int newUI = 7;
        if (oldUI == newUI)
            return;

        gotoScreenUI(oldUI, newUI);
        hideMenuPopup();
    }

    public void onbtnFriendFinder()
    {
        int oldUI = Global.screenID;
        int newUI = 8;
        if (oldUI == newUI)
            return;

        gotoScreenUI(oldUI, newUI);
        hideMenuPopup();
    }

    public void onbtnSettings()
    {
        int oldUI = Global.screenID;
        int newUI = 11;
        if (oldUI == newUI)
            return;

        gotoScreenUI(oldUI, newUI);
        hideMenuPopup();
    }

    public void gotoScreenUI(int oldUI, int newUI)
    {
        if (DetailPopup.activeInHierarchy) DetailPopup.SetActive(false);

        //6: Products(Home), 7: My Items, 8: Friend Finder, 9: Calendar, 10: Notifications, 11: Settings
        if (oldUI == 6)
        {
            Products.GetComponent<SwipeUI>().hideUI(-1);
        }
        else if(oldUI == 7)
        {
            MyItems.GetComponent<SwipeUI>().hideUI(-1);
        }
        else if (oldUI == 8)
        {
            FriendFinder.GetComponent<SwipeUI>().hideUI(-1);
        }
        else if (oldUI == 11)
        {
            Settings.GetComponent<SwipeUI>().hideUI(-1);
        }


        if (newUI == 6)
        {
            Products.GetComponent<SwipeUI>().showUI(1);
        }
        else if(newUI == 7)
        {
            MyItems.GetComponent<SwipeUI>().showUI(1);
            MyItems.GetComponent<Items>().initUI(Global.m_user.id);
        }
        else if (newUI == 8)
        {
            if (mm.FriendItems.activeInHierarchy)
            {
                mm.FriendItems.SetActive(false);
            }
            FriendFinder.GetComponent<SwipeUI>().showUI(1);
            FriendFinder.GetComponent<FriendFinder>().initUI();
        }
        else if (newUI == 11)
        {
            Settings.GetComponent<SwipeUI>().showUI(1);
        }


        Global.screenID = newUI;
    }

    public void showMenuPopup()
    {
        Menu.GetComponent<SwipeUI>().showUI(-1);
        Global.isMenuShowed = true;
    }

    public void hideMenuPopup()
    {
        Menu.GetComponent<SwipeUI>().hideUI(-1);
    }

    public void ShowDetailPopup(Product product, RawImage rawImage)
    {
        if (product.market_id == 1)
        {//Etsy
            pd.dpLogo.sprite = pd.logos[0];
        }
        else if (product.market_id == 2)
        {//BestBuy
            pd.dpLogo.sprite = pd.logos[1];
        }
        else if (product.market_id == 3)
        {//Wish
            pd.dpLogo.sprite = pd.logos[2];
        }

        pd.Title.GetComponent<Text>().text = product.title;
        pd.Image.GetComponent<RawImage>().texture = rawImage.texture;
        pd.Price.transform.Find("value").GetComponent<Text>().text = product.price;
        pd.Description.transform.GetComponent<Text>().text = product.description;
        float height = pd.Description.transform.GetComponent<Text>().preferredHeight;
        pd.Description.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(412, height);
        pd.url = UnityWebRequest.UnEscapeURL(product.url);

        DetailPopup.SetActive(true);
    }

    public void ShowDetailPopup(string product_id, int market_id)
    {
        if (market_id == 1)
        {//Etsy
            pd.dpLogo.sprite = pd.logos[0];
        }
        else if (market_id == 2)
        {//BestBuy
            pd.dpLogo.sprite = pd.logos[1];
        }
        else if (market_id == 3)
        {//Wish
            pd.dpLogo.sprite = pd.logos[2];
        }

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("product_id", product_id));
        formData.Add(new MultipartFormDataSection("market_id", market_id.ToString()));

        string requestURL = Global.DOMAIN + "/API/GetProduct.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        StartCoroutine(ResponseGetProduct(www));
    }

    IEnumerator ResponseGetProduct(UnityWebRequest www)
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

        string title = UnityWebRequest.UnEscapeURL(json["title"].ToString());
        string description = UnityWebRequest.UnEscapeURL(json["description"].ToString());
        string price = UnityWebRequest.UnEscapeURL(json["price"].ToString());
        string image = UnityWebRequest.UnEscapeURL(json["image"].ToString());
        string url = UnityWebRequest.UnEscapeURL(json["url"].ToString());

        pd.Title.GetComponent<Text>().text = title;
        pd.Title.transform.SetParent(pd.Content.transform);

        www = UnityWebRequestTexture.GetTexture(image);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield break;
        }
        pd.Image.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

        pd.Image.transform.SetParent(pd.Content.transform);
        pd.Price.transform.Find("value").GetComponent<Text>().text = price;
        pd.Price.transform.SetParent(pd.Content.transform);
        pd.Description.GetComponent<Text>().text = description;
        pd.Description.transform.SetParent(pd.Content.transform);

        pd.url = UnityWebRequest.UnEscapeURL(url);

        DetailPopup.SetActive(true);
    }
}
