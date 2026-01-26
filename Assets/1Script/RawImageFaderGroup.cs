using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RawImageFaderGroup : MonoBehaviour
{
    public RawImage[] _rawImages;


    void Start()
    {
        var rawImages = GetComponentsInChildren<RawImage>();

        var list = new List<RawImage>(rawImages.Length);
        for (int i = 0; i < rawImages.Length; i++)
        {
            var r = rawImages[i];
            if (r.gameObject != gameObject)
            {
                list.Add(r);
            }
        }
        _rawImages = list.ToArray();

    }


    public void AllLEDOn()
    {
        FadeManager.Instance.TargetFade(_rawImages, 1f, 0.5f);
    }

    public void AllLEDOff()
    {
        FadeManager.Instance.TargetFade(_rawImages, 0f, 0.5f);
    }

    public void LEDOn(int index)
    {
        FadeManager.Instance.TargetFade(_rawImages[index], 1f, 0.5f);
    }

    public void LEDOff(int index)
    {
        FadeManager.Instance.TargetFade(_rawImages[index], 0f, 0.5f);
    }

    public void LEDOnOnly(int index)
    {
        int count = 0;
        foreach (var img in _rawImages)
        {
            if (count != index)
                FadeManager.Instance.TargetFade(img, 0f, 0.5f);
            count++;

        }
        FadeManager.Instance.TargetFade(_rawImages[index], 1f, 0.5f);
    }




}
