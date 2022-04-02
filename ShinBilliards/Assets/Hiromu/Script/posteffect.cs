using UnityEngine;
using System.Collections;

public class posteffect : MonoBehaviour
{

    [SerializeField] Material monoTone;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, monoTone);
    }
}