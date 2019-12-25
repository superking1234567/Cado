using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenSelect : MonoBehaviour
{
    public long children_id = 0;

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
        Global.m_user.children_id = children_id;
        GameObject.Find("MainManager").GetComponent<MainManager>().Dashboard.GetComponent<Dashboard>().onbtnHome();
    }
}
