using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StaticCoroutine : MonoBehaviour
{
    static public StaticCoroutine instance; //the instance of our class that will do the work

    void Awake()
    { //called when an instance awakes in the game
        instance = this; //set our static reference to our newly initialized instance
    }

    public IEnumerator Fadeout(GameObject[] objs, Text[] texts, Image[] imgs, float fadeDuration)
    {
        Debug.Log("Fadeout");
        foreach (GameObject o in objs)
        {
            if (o != null) o.SetActive(true);
            Debug.Log(o.ToString());
        }
        foreach (Text t in texts)
            if (t != null)
            {
                {
                    t.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
                    t.CrossFadeAlpha(0f, fadeDuration, true);
                    Debug.Log(t.ToString());
                }
            }
        foreach (Image img in imgs)
        {
            if (img != null)
            {
                img.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
                img.CrossFadeAlpha(0f, fadeDuration, true);
                Debug.Log(img.ToString());
            }
        }

        yield return new WaitForSeconds(fadeDuration);
        foreach (GameObject o in objs)
        {
            if (o != null) o.SetActive(false);
        }
    }

    static public void DoCoroutine(GameObject[] objs, Text[] texts, Image[] imgs, float fadeDuration)
    {
        instance.StartCoroutine(instance.Fadeout(objs, texts, imgs, fadeDuration)); //this will launch the coroutine on our instance
    }
}
