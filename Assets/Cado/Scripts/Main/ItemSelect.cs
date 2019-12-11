using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviour
{
    public string product_id = "";
    public string product_name = "";
    public int market_id = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnItemSelected()
    {
        Global.selectedItemIndex = Global.myItemList.FindIndex(x => x.product_id == product_id && x.market_id == market_id);
        GameObject.Find("Canvas/Dashboard").GetComponent<Dashboard>().ShowDetailPopup(Global.myItemList[Global.selectedItemIndex], this.transform.Find("RawImage").GetComponent<RawImage>());
    }

    public void OnItemDeleted()
    {
        Global.selectedItemIndex = Global.myItemList.FindIndex(x => x.product_id == product_id && x.market_id == market_id);
        GameObject.Find("MainManager").GetComponent<MainManager>().ShowDelPopup("Do you really want to delete it?");
    }
}
