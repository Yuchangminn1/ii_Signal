using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseInputImage : MonoBehaviour
{
    MorseInputTarget _morseImageDot;
    MorseInputTarget _morseImageDash;


    public bool isFilled = false;

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
            else
            {
                Debug.LogError("타입 안정했음 ");
            }
        }

    }

    public void Reset()
    {
        _morseImageDot.Reset();
        _morseImageDash.Reset();
        isFilled = false;
    }

    void OnEnable()
    {

    }
    public void StartColoring(MorseType morseType)
    {
        isFilled = false;
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

    }

    IEnumerator WaitAndCheckFill(MorseInputTarget morseInputTarget)
    {
        while (morseInputTarget.IsCheck == false)
        {
            yield return CoroutineReturnManager.WaitForFixedUpdate;
        }
        WaitCheckCoroutine = null;
        isFilled = true;
    }
}
