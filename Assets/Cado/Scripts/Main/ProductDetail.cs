using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductDetail : MonoBehaviour
{
    public GameObject Content;
    public GameObject Title;
    public GameObject Image;
    public GameObject Price;
    public GameObject Description;
    public string url;

    public Sprite[] logos = new Sprite[3];
    public Image dpLogo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onbtnClose()
    {
        this.transform.gameObject.SetActive(false);
    }

    public void onbtnLink()
    {
        Application.OpenURL(url);
    }
}
