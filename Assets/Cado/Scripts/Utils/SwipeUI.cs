using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeUI : MonoBehaviour
{
    private float sumTime = 0.0f, actionTime = 0.2f;

    public Vector3 pos1 = Vector3.zero;
    public Vector3 pos2 = new Vector3(512.0f, 0.0f, 0.0f);
    private int direction = 1;  //-1: right->left, 1: left->right

    private bool isShowUI = false;
    private bool isHideUI = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isShowUI)
        {
            sumTime += Time.deltaTime;
            float fracJourney = sumTime / actionTime;

            this.transform.localPosition = Vector3.Lerp(pos2 * direction, pos1, fracJourney);
            if (fracJourney >= 1.0f)
            {
                sumTime = 0.0f;
                actionTime = 0.2f;
                this.transform.localPosition = pos1;
                isShowUI = false;
            }
        }

        if (isHideUI)
        {
            sumTime += Time.deltaTime;
            float fracJourney = sumTime / actionTime;

            this.transform.localPosition = Vector3.Lerp(pos1, pos2 * direction, fracJourney);
            if (fracJourney >= 1.0f)
            {
                sumTime = 0.0f;
                actionTime = 0.2f;
                this.transform.localPosition = pos2 * direction;
                isHideUI = false;
            }
        }
    }

    public void showUI(int direction)
    {
        this.direction = direction;
        isShowUI = true;
    }

    public void hideUI(int direction)
    {
        this.direction = direction;
        isHideUI = true;
    }
}
