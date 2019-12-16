using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject Categories;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onbtnEditCategories()
    {
        Categories.GetComponent<Categories>().btnClose.SetActive(true);
        Categories.GetComponent<Categories>().GetCategoryList(1);
        Categories.transform.localPosition = new Vector3(0, 0, 0);
    }
}
