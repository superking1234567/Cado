using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Products : MonoBehaviour
{
    public MainManager mm;
    public GameObject CardPanel;
    public GameObject cardPrefab;
    public GameObject Loading;

    private bool isUpdating = false;
    private bool isFirstLoading = true;

    private List<GameObject> cardList = new List<GameObject>();
    private List<Product> productList = new List<Product>();

    private Vector3 mouseDownPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Global.m_user != null && !isUpdating && Global.productList != null)
        {
            if(Global.productList.Count < 5)
            {
                GetProductList();
            }
        }

        if(cardList.Count > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                float deltaValue = (Input.mousePosition.x - mouseDownPos.x);
                //Debug.Log("--deltaValue--" + deltaValue + "--screenWidth--" + Screen.width);
                if (Input.mousePosition.x > mouseDownPos.x)
                {
                    if (deltaValue / Screen.width > 0.2f)
                    {
                        Debug.Log("You dragged right!");
                        onbtnPresent();
                    }
                }
                else if (Input.mousePosition.x < mouseDownPos.x)
                {
                    if (deltaValue / Screen.width < -0.2f)
                    {
                        Debug.Log("You dragged right!");
                        onbtnBin();
                    }
                }
            }
        }
    }

    public void GetProductList()
    {
        isUpdating = true;

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", Global.m_user.id.ToString()));

        string requestURL = Global.DOMAIN + "/API/GetProductList.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        StartCoroutine(ResponseGetProductList(www));
    }

    IEnumerator ResponseGetProductList(UnityWebRequest www)
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

        int market_id = int.Parse(json["market_id"].ToString());
        if(market_id == 1)
        {//etsy
            for (int i = 0; i < json["product_list"].Count; i++)
            {
                Product pt = new Product();
                pt.product_id = long.Parse(json["product_list"][i]["listing_id"].ToString());
                pt.title = UnityWebRequest.UnEscapeURL(json["product_list"][i]["title"].ToString());
                pt.description = UnityWebRequest.UnEscapeURL(json["product_list"][i]["description"].ToString());
                pt.image = UnityWebRequest.UnEscapeURL(json["product_list"][i]["image"].ToString());
                pt.market_id = 1;

                Global.productList.Add(pt);
            }
        }

        Global.Shuffle<Product>(Global.productList);

        if (isFirstLoading)
        {
            StartCoroutine(FirstLoad());
        }

        isUpdating = false;
    }

    IEnumerator FirstLoad()
    {
        yield return new WaitForEndOfFrame();
        foreach (Transform child in CardPanel.transform)
        {
            Destroy(child.gameObject);
        }
        Resources.UnloadUnusedAssets();

        int count = Global.productList.Count;
        if (count > 3)
        {
            count = 3;
        }
        
        float height = -20.0f;
        for (int i = 0; i < 3; i++)
        {
            Product product = Global.productList[0];
            Global.productList.RemoveAt(0);

            GameObject temp = Instantiate(cardPrefab) as GameObject;
            temp.transform.SetParent(CardPanel.transform);

            temp.SetActive(false);
            temp.transform.name = product.product_id.ToString();
            temp.transform.Find("txtProductName").GetComponent<Text>().text = product.title;
            temp.transform.Find("txtProductDetail").GetComponent<Text>().text = product.description;
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(product.image);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield break;
            }
            temp.transform.Find("imgProduct").GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            temp.transform.localPosition = new Vector3(0.0f, height, 0.0f);
            temp.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            temp.SetActive(true);

            cardList.Add(temp);
            productList.Add(product);

            height += 20.0f;
        }

        Loading.SetActive(false);

        float zoomMax = 0.9f;
        for (int i = 0; i < 3; i++)
        {
            cardList[i].transform.SetAsLastSibling();
            cardList[i].transform.GetComponent<CardAni>().ZoomAni(5, 0.0f, zoomMax);
            zoomMax += 0.05f;
        }

        isFirstLoading = false;
    }

    IEnumerator AddCard()
    {
        yield return new WaitForEndOfFrame();

        Product product = Global.productList[0];
        Global.productList.RemoveAt(0);

        GameObject temp = Instantiate(cardPrefab) as GameObject;
        temp.transform.SetParent(CardPanel.transform);

        temp.SetActive(false);
        temp.transform.name = product.product_id.ToString();
        temp.transform.Find("txtProductName").GetComponent<Text>().text = product.title;
        temp.transform.Find("txtProductDetail").GetComponent<Text>().text = product.description;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(product.image);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield break;
        }
        temp.transform.Find("imgProduct").GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        temp.transform.localPosition = new Vector3(0.0f, -20.0f, 0.0f);
        temp.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        temp.SetActive(true);

        cardList.Insert(0, temp);
        productList.Insert(0, product);

        cardList[0].transform.SetAsFirstSibling();
        cardList[0].transform.GetComponent<CardAni>().ZoomAni(5, 0.0f, 0.9f);
    }

    public void onbtnBin()
    {
        if(cardList.Count < 3)
        {
            return;
        }

        GameObject topCard = cardList[cardList.Count - 1];
        cardList.RemoveAt(cardList.Count - 1);
        Product product = productList[productList.Count - 1];
        productList.RemoveAt(productList.Count - 1);

        Vector3 pos11 = topCard.transform.localPosition;
        Vector3 pos12 = new Vector3(-512.0f, pos11.y, pos11.z);
        topCard.transform.GetComponent<CardAni>().MoveAni(pos11, pos12, true);

        GameObject secondCard = cardList[cardList.Count - 1];
        Vector3 pos21 = secondCard.transform.localPosition;
        Vector3 pos22 = new Vector3(pos21.x, pos21.y + 20.0f, pos21.z);
        secondCard.transform.GetComponent<CardAni>().MoveAni(pos21, pos22);
        secondCard.transform.GetComponent<CardAni>().ZoomAni(5, 0.95f, 1f);

        GameObject thirdCard = cardList[cardList.Count - 2];
        Vector3 pos31 = thirdCard.transform.localPosition;
        Vector3 pos32 = new Vector3(pos31.x, pos31.y + 20.0f, pos31.z);
        thirdCard.transform.GetComponent<CardAni>().MoveAni(pos31, pos32);
        thirdCard.transform.GetComponent<CardAni>().ZoomAni(5, 0.9f, 0.95f);

        StartCoroutine(AddCard());
        StartCoroutine(SetRating(product, 2));
    }

    public void onbtnPresent()
    {
        if (cardList.Count < 3)
        {
            return;
        }

        GameObject topCard = cardList[cardList.Count - 1];
        cardList.RemoveAt(cardList.Count - 1);
        Product product = productList[productList.Count - 1];
        productList.RemoveAt(productList.Count - 1);

        Vector3 pos11 = topCard.transform.localPosition;
        Vector3 pos12 = new Vector3(512.0f, pos11.y, pos11.z);
        topCard.transform.GetComponent<CardAni>().MoveAni(pos11, pos12, true);

        GameObject secondCard = cardList[cardList.Count - 1];
        Vector3 pos21 = secondCard.transform.localPosition;
        Vector3 pos22 = new Vector3(pos21.x, pos21.y + 20.0f, pos21.z);
        secondCard.transform.GetComponent<CardAni>().MoveAni(pos21, pos22);
        secondCard.transform.GetComponent<CardAni>().ZoomAni(5, 0.95f, 1f);

        GameObject thirdCard = cardList[cardList.Count - 2];
        Vector3 pos31 = thirdCard.transform.localPosition;
        Vector3 pos32 = new Vector3(pos31.x, pos31.y + 20.0f, pos31.z);
        thirdCard.transform.GetComponent<CardAni>().MoveAni(pos31, pos32);
        thirdCard.transform.GetComponent<CardAni>().ZoomAni(5, 0.9f, 0.95f);

        StartCoroutine(AddCard());
        StartCoroutine(SetRating(product, 1));
    }

    IEnumerator SetRating(Product product, int value)
    {
        yield return new WaitForEndOfFrame();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", Global.m_user.id.ToString()));
        formData.Add(new MultipartFormDataSection("product_id", product.product_id.ToString()));
        formData.Add(new MultipartFormDataSection("title", UnityWebRequest.EscapeURL(product.title)));
        formData.Add(new MultipartFormDataSection("description", UnityWebRequest.EscapeURL(product.description)));
        formData.Add(new MultipartFormDataSection("image", UnityWebRequest.EscapeURL(product.image)));
        formData.Add(new MultipartFormDataSection("market_id", product.market_id.ToString()));
        formData.Add(new MultipartFormDataSection("value", value.ToString()));

        string requestURL = Global.DOMAIN + "/API/SetRating.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

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
    }

    public void onbtnFavorite()
    {

    }

}
