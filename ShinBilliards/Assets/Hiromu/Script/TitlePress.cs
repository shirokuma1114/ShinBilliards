using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePress : MonoBehaviour
{
    private Text Press;
    // Start is called before the first frame update
    void Awake()
    {
        Press = gameObject.GetComponent<Text>();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeOut()
    {
        float a = 0f;

        while (Press.color.a < 1f)
        {
            a += Time.deltaTime;
            Press.color = new Color(1, 1, 1, a);
            yield return null;
        }
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float a = 1f;

        while (Press.color.a > 0f)
        {
            a -= Time.deltaTime;
            Press.color = new Color(1, 1, 1, a);
            yield return null;
        }
        StartCoroutine(FadeOut());
    }
}
