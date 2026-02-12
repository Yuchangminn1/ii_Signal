using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerStamp : MonoBehaviour
{
    RawImage _emptyStampImage;
    RawImage _correctStampImage;

    public void SetTextures(Texture emptyStampTexture, Texture correctStampTexture)
    {
        _emptyStampImage.texture = emptyStampTexture;
        _correctStampImage.texture = correctStampTexture;
    }

    public void SetEmptyStamp()
    {
        FadeManager.Instance.SetAlphaZero(_correctStampImage);

    }

    public void SetCorrectStamp()
    {

        FadeManager.Instance.SetAlphaOne(_correctStampImage);
    }


    void Start()
    {
        RawImage[] tmpRawimages = GetComponentsInChildren<RawImage>();
        _emptyStampImage = tmpRawimages[0];
        _correctStampImage = tmpRawimages[1];
    }

}
