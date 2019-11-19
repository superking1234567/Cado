using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCtrl : MonoBehaviour
{
    public Question qt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onValueChanged(Toggle t)
    {
        if (t.isOn)
        {
            qt.addQuestionItem(t.name);
        }
        else
        {
            qt.removeQuestionItem(t.name);
        }
    }
}
