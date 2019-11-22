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
        for (int i=0; i< json["category_list"].Count; i+=3)
        {
            temp = Instantiate(categoryPrefab) as GameObject;
            temp.SetActive(false);
            temp.transform.name = i.ToString();

            for(int j=0; j<3; j++)
            {
                if(i+j >= json["category_list"].Count)
                { 
                    temp.transform.Find("item" + j).gameObject.SetActive(false);
                }
                else
                {
                    temp.transform.Find("item" + j + "/Text").GetComponent<Text>().text = json["category_list"][i + j]["name"].ToString();
                    temp.transform.SetParent(categoryList.transform);
                    temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
            temp.SetActive(true);

        }

    }


}
