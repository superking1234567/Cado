﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductDetail : MonoBehaviour
{
    public string url;

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
