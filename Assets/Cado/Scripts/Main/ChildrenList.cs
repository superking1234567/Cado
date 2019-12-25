using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildrenList : MonoBehaviour
{
    public MainManager mm;
    public GameObject ChildrenContent;
    public GameObject childrenPrefab;
    public GameObject Loading;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initUI()
    {
        foreach (Transform child in ChildrenContent.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject temp;
        if(Global.m_user.children_id > 0)
        {
            temp = Instantiate(childrenPrefab) as GameObject;
            temp.SetActive(false);
            temp.transform.name = "0";

            temp.transform.GetComponent<ChildrenSelect>().children_id = 0;
            temp.transform.Find("Text").GetComponent<Text>().text = Global.m_user.firstname + " " + Global.m_user.lastname;

            temp.transform.SetParent(ChildrenContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.SetActive(true);
        }

        for (int i=0; i< Global.m_childrenList.Count; i++)
        {
            if(Global.m_user.children_id == Global.m_childrenList[i].id)
            {
                continue;
            }

            temp = Instantiate(childrenPrefab) as GameObject;
            temp.SetActive(false);
            temp.transform.name = Global.m_childrenList[i].id.ToString();

            temp.transform.GetComponent<ChildrenSelect>().children_id = Global.m_childrenList[i].id;
            temp.transform.Find("Text").GetComponent<Text>().text = Global.m_childrenList[i].firstname + " " + Global.m_childrenList[i].lastname;

            temp.transform.SetParent(ChildrenContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.SetActive(true);
        }
    }
}
