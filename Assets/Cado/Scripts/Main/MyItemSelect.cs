using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyItemSelect : MonoBehaviour
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

    }

    public void OnItemDeleted()
    {
        Global.selectedItemIndex = Global.myItemList.FindIndex(x => x.product_id == product_id && x.market_id == market_id);
        GameObject.Find("MainManager").GetComponent<MainManager>().ShowDelPopup("Do you really want to delete it?");
    }
}
