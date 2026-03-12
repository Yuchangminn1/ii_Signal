using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseInputImage : MonoBehaviour
{
    MorseInputTarget _morseImageDot;
    MorseInputTarget _morseImageDash;

    MorseInputTarget _morseShadow;


    public bool IsFilled = false;

    public bool IsFilling = false;


    Coroutine WaitCheckCoroutine = null;



    void Start()
    {
        foreach (MorseInputTarget child in GetComponentsInChildren<MorseInputTarget>())
        {
            if (child.CurrentMorseType == MorseType.Dot)
            {
                _morseImageDot = child;
            }
            else if (child.CurrentMorseType == MorseType.Dash)
            {
                _morseImageDash = child;
            }
            else if (child.CurrentMorseType == MorseType.Shadow)
            {
                _morseShadow = child;
            }
            else
            {
                Debug.LogError("타입 안정했음 ");
            }
        }

    }
    public void SetErrorColor(Color32 color)
    {
        _morseImageDot.SetErrorColor(color);
        _morseImageDash.SetErrorColor(color);
        _morseShadow.SetErrorColor(color);
    }



    public void Reset()
    {
        _morseImageDot.Reset();
        _morseImageDash.Reset();
        _morseShadow.Reset();
        IsFilled = false;
    }

    void OnEnable()
    {
        IsFilled = false;
    }

    public void ShadowColoring()
    {
        StartColoring(MorseType.Shadow);
    }
    public void StartColoring(MorseType morseType)
    {
        IsFilling = true;
        if (morseType == MorseType.Dot)
        {
            _morseImageDot.StartColoring();


            if (WaitCheckCoroutine == null)
                WaitCheckCoroutine = StartCoroutine(WaitAndCheckFill(_morseImageDot));
        }
        else if (morseType == MorseType.Dash)
        {
            _morseImageDash.StartColoring();

            if (WaitCheckCoroutine == null)
                WaitCheckCoroutine = StartCoroutine(WaitAndCheckFill(_morseImageDash));
        }
        else if (morseType == MorseType.Shadow)
        {
            _morseShadow.StartColoring();

            if (WaitCheckCoroutine == null)
                WaitCheckCoroutine = StartCoroutine(WaitAndCheckFill(_morseShadow));
        }

    }

    IEnumerator WaitAndCheckFill(MorseInputTarget morseInputTarget)
    {
        while (morseInputTarget.IsCheck == false)
        {
            yield return CoroutineReturnManager.WaitForFixedUpdate;
        }
        WaitCheckCoroutine = null;
        IsFilled = true;
        IsFilling = false;
    }
}
