using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSelector : MonoBehaviour
{
    protected RawImage[] _rawImage;

    protected virtual void Awake()
    {
        _rawImage = GetComponentsInChildren<RawImage>();
    }


    public virtual void Reset()
    {
        if (_rawImage == null || _rawImage.Length == 0)
            return;
        FadeManager.Instance.SetAlphaZero(_rawImage);
    }

    public virtual void SelectAnswer(int selectIndex)
    {
        FadeManager.Instance.SetAlphaOne(_rawImage[selectIndex]);
    }
}
