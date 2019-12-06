using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{
    public GameObject Categories;
    public GameObject Products;
    public GameObject MyItems;
    public GameObject FriendFinder;

    public GameObject Menu;
    public GameObject[] menuItems;

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

    public void onbtnSetting()
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

    public void gotoScreenUI(int oldUI, int newUI)
    {
        //6: Products(Home), 7: My Items, 8: Friend Finder, 9: Calendar, 10: Notifications, 11: Settings
        if (oldUI == 6){
            Products.GetComponent<SwipeUI>().hideUI(-1);
        }else if(oldUI == 7){
            MyItems.GetComponent<SwipeUI>().hideUI(-1);
        }else if (oldUI == 8){
            FriendFinder.GetComponent<SwipeUI>().hideUI(-1);
        }

        if (newUI == 6){
            Products.GetComponent<SwipeUI>().showUI(1);
        }else if(newUI == 7){
            MyItems.GetComponent<SwipeUI>().showUI(1);
            MyItems.GetComponent<MyItems>().initUI();
        }else if (newUI == 8)
        {
            FriendFinder.GetComponent<SwipeUI>().showUI(1);
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
}
