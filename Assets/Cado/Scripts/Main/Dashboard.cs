using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashboard : MonoBehaviour
{
    public GameObject Categories;
    public GameObject ProductPanel;

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
        Global.productList = new List<Product>();
        Categories.GetComponent<SwipeUI>().hideUI(-1);
        ProductPanel.GetComponent<SwipeUI>().showUI(1);
    }
}
