using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupAni : MonoBehaviour
{
    private float step = 0.0f;
    private int direction = 0;  //0:+ 1:-

    private float speed = 10.0f;
    public bool isAutoClose = false;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
        StartCoroutine("StartAni");
    }

    IEnumerator StartAni()
    {
        while (true)
        {
            if (step < 1.2f && direction == 0)
            {
                this.transform.localScale = new Vector3(step, step, 1.0f);
                //step += speed * Time.deltaTime;
                step += speed * Time.fixedDeltaTime;
            }
            else
            {
                direction = 1;
                this.transform.localScale = new Vector3(step, step, 1.0f);
                //step -= (speed * Time.deltaTime / 2);
                step -= (speed * Time.fixedDeltaTime / 2);
                if (step < 1.0f)
                {
                    this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    step = 0.0f;
                    direction = 0;

                    if (isAutoClose)
                    {
                        yield return new WaitForSeconds(2.0f);
                        StopAni();
                    }
                    break;
                }
            }
            //yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
        }
    }

    public void StopAni()
    {
        this.gameObject.SetActive(false);
    }
}
