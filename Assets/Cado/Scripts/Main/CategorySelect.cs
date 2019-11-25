using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategorySelect : MonoBehaviour
{
    public long category_id = 0;
    public string category_name = "";
    public int market_id = 0;

    private Button btnCategory;
    private bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        btnCategory = this.transform.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnItemSelected()
    {
        Color colorV;
        if (!isSelected)
        {
            isSelected = true;
            ColorUtility.TryParseHtmlString("#6FCAF3", out colorV);

            Category category = new Category(category_id, category_name, market_id);
            Global.categoryList.Add(category);
        }
        else
        {
            isSelected = false;
            ColorUtility.TryParseHtmlString("#FFFFFF", out colorV);

            int index = Global.categoryList.FindLastIndex(x => x.id == category_id);
            Global.categoryList.RemoveAt(index);
        }

        ColorBlock theColor = btnCategory.colors;
        theColor.normalColor = colorV;
        theColor.highlightedColor = colorV;
        btnCategory.colors = theColor;
    }
}
