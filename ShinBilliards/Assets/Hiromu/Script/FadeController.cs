using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : CSingletonMonoBehaviour<FadeController>
{
    public enum FadeState
    {
        FadeIn,
        FadeOut,
        Black,
        Normal
    }

    public FadeState state;
    private Image FadeImage;
    private Color color;
    [SerializeField] float FadeSpeed;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(transform.parent.gameObject);

        FadeImage = gameObject.GetComponent<Image>();
        color = FadeImage.color;

        state = FadeState.Normal;
    }

    public void FadeInStart()
    {
        if (state == FadeState.FadeIn|| state == FadeState.FadeOut) return;
        state = FadeState.FadeIn;

        StartCoroutine(FadeIn());
    }

    public void FadeOutStart()
    {
        if (state == FadeState.FadeOut|| state == FadeState.FadeIn) return;
        state = FadeState.FadeOut;

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        color = FadeImage.color;

        while (FadeImage.color.a > 0f)
        {
            color.a -= Time.deltaTime;
            FadeImage.color = color;
            yield return null;
        }
        state = FadeState.Normal;
    }

    IEnumerator FadeOut()
    {
        color = FadeImage.color;

        while (FadeImage.color.a < 1f)
        {
            color.a += Time.deltaTime;
            FadeImage.color = color;
            yield return null;
        }
        state = FadeState.Black;
    }
}
