using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAni : MonoBehaviour
{
    private float sumTime = 0.0f, actionTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ZoomAni(float speed, float zoomMin, float zoomMax)
    {
        StartCoroutine(ZoomAniCoroutine(speed, zoomMin, zoomMax));
    }

    IEnumerator ZoomAniCoroutine(float speed, float zoomMin, float zoomMax)
    {
        float step = zoomMin;
        while (true)
        {
            if (step <= zoomMax)
            {
                this.transform.localScale = new Vector3(step, step, 1.0f);
                step += speed * Time.fixedDeltaTime;
            }
            else
            {
                step = zoomMin;
                this.transform.localScale = new Vector3(zoomMax, zoomMax, 1.0f);
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void MoveAni(Vector3 pos1, Vector3 pos2, bool isDestroy = false)
    {
        StartCoroutine(MoveAniCoroutine(pos1, pos2, isDestroy));
    }

    IEnumerator MoveAniCoroutine(Vector3 pos1, Vector3 pos2, bool isDestroy)
    {
        while (true)
        {
            sumTime += Time.deltaTime;
            float fracJourney = sumTime / actionTime;

            this.transform.localPosition = Vector3.Lerp(pos1, pos2, fracJourney);
            if (fracJourney >= 1.0f)
            {
                sumTime = 0.0f;
                actionTime = 0.2f;
                this.transform.localPosition = pos2;
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        if (isDestroy)
        {
            Destroy(this.gameObject);
        }
    }

}
