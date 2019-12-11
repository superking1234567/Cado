using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendSelect : MonoBehaviour
{
    public long user_id = -1;

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
        GameObject.Find("MainManager").GetComponent<MainManager>().FriendItems.SetActive(true);
        GameObject.Find("MainManager").GetComponent<MainManager>().FriendItems.GetComponent<Items>().initUI(user_id);
    }
}
