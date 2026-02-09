using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MorsePassCheck : MonoBehaviour
{
    MorseImage[] morseImages;

    Queue<MorseType> _morseInput = new Queue<MorseType>();

    Arduino_MorseKey arduino_MorseKey;

    public SequenceScript SequenceScript;

    Coroutine _morseIndexCheckCoroutine = null;


    int _currentIndex = 0;


    void OnEnable()
    {
        _morseIndexCheckCoroutine = null;
        if (arduino_MorseKey != null)
            arduino_MorseKey.StopMorseCheck();
    }

    public void CheckStart()
    {

        if (arduino_MorseKey == null)
        {
            return;
        }
        if (arduino_MorseKey != null)
        {
            arduino_MorseKey.AddOnMorseInput(ColoringMorseImage);
            arduino_MorseKey.OnReset += Reset;

        }
        _currentIndex = 0;

        _morseInput.Clear();

        arduino_MorseKey.StartMorseCheck();


    }

    public void Reset()
    {
        Debug.Log("Reset");
    }


    IEnumerator MorseIndexCheckCoroutine(MorseType morseType)
    {


        while (morseImages[_currentIndex].IsCheck == false)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }


        _currentIndex++;

        if (_currentIndex == morseImages.Length)
        {
            Debug.Log("트리거");
            SequenceScript.TriggerFroceOn();
            arduino_MorseKey.StopMorseCheck();
        }

        _morseIndexCheckCoroutine = null;


    }



    void Start()
    {
        morseImages = GetComponentsInChildren<MorseImage>();

        arduino_MorseKey = GetComponentInParent<Arduino_MorseKey>();
    }
    void OnDisable()
    {

        if (arduino_MorseKey != null)
        {
            arduino_MorseKey.StopMorseCheck();
        }

    }



    public void ColoringMorseImage(MorseType morseType)
    {

        if (_currentIndex >= morseImages.Length)
        {
            return;
        }
        if (morseImages[_currentIndex].CurrentMorseType == morseType)
        {
            morseImages[_currentIndex].StartColoring();
            if (_morseIndexCheckCoroutine == null)
                _morseIndexCheckCoroutine = StartCoroutine(MorseIndexCheckCoroutine(morseType));
        }
    }

}
