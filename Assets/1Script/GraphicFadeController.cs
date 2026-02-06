using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class GraphicFadeController : MonoBehaviour
{

    public int playerIndex = 0;

    public Graphic[] _graphic;

    protected float customFadeDuration = -1f;


    public bool isMain = false;
    public Arduino_MorseKey arduino_Touch;

    protected virtual void Awake()
    {
        if (customFadeDuration == -1)
        {
            customFadeDuration = FadeManager.Instance.FadeDuration;
        }
    }

    protected virtual void OnEnable()
    {
        if (GameManager.Instance.IsStarted == false)
            return;
        Reset();
    }






    protected virtual void Reset()
    {
        ;

    }


    protected virtual void Start()
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

        _graphic = list.ToArray();


    }


    public void AllFadeOn()
    {
        FadeManager.Instance.TargetFade(_graphic, 1f, customFadeDuration);
    }
    public void AllCutOn()
    {
        FadeManager.Instance.SetAlphaOne(_graphic);
    }
    public void AllFadeOff()
    {
        FadeManager.Instance.TargetFade(_graphic, 0f, customFadeDuration);
    }

    public void AllCutOff()
    {
        FadeManager.Instance.SetAlphaZero(_graphic);
    }



    public void FadeOn()
    {
        //Pair pair = PlayerDatas.Instance.GetPlayerLEDPair(playerIndex);

        // Debug.Log($"{name}FadeOn Called {pair.First}, {pair.Second}");
        // int count = 0;
        // for (int i = 0; i < _graphic.Length; i++)
        // {
        //     if (i == pair.First || i == pair.Second)
        //     {
        //         FadeManager.Instance.TargetFade(_graphic[i], 1f, customFadeDuration);
        //         count++;
        //     }
        //     else
        //     {
        //         FadeManager.Instance.TargetFade(_graphic[i], 0f, customFadeDuration);
        //     }
        // }
    }
    public void FadeOn(int index)
    {
        for (int i = 0; i < _graphic.Length; i++)
        {
            if (i == index)
            {
                FadeManager.Instance.TargetFade(_graphic[i], 1f, customFadeDuration);
            }
            else
            {
                FadeManager.Instance.TargetFade(_graphic[i], 0f, customFadeDuration);
            }
        }
    }
}
