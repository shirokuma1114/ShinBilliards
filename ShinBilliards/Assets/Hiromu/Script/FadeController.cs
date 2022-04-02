using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public enum FadeState
    {
        FadeIn,
        FadeOut,
        Black,
        Normal
    }

    public FadeState _state;
    private Image FadeImage;
    private Color color;
    [SerializeField] float FadeSpeed;

    void Awake()
    {
        FadeImage = gameObject.GetComponent<Image>();
        color = FadeImage.color;

        _state = FadeState.Normal;
        FadeInStart();
    }

    public void FadeInStart()
    {
        if (_state == FadeState.FadeIn) return;
        _state = FadeState.FadeIn;

        StartCoroutine(FadeIn());
    }

    public void FadeOutStart()
    {
        if (_state == FadeState.FadeOut) return;
        _state = FadeState.FadeOut;

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
        _state = FadeState.Normal;
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
        _state = FadeState.Black;
    }
}
