using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Morse_Guide : MonoBehaviour
{


    MorseImage[] morseImages;


    Arduino_MorseKey arduino_MorseKey;

    public SequenceScript SequenceScript;

    public string MorseData = "0";

    float _graphicDelay = 1.2f;



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
        for (int i = 0; i < morseImages.Length; i++)
        {
            if (MorseData[i] == '0')
                morseImages[i].SetMorseType(MorseType.Dot);
            else if (MorseData[i] == '1')
                morseImages[i].SetMorseType(MorseType.Dash);
        }


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
            arduino_MorseKey.StopMorseCheck();

            while (morseImages[_currentIndex - 1].IsCheck == false)
            {
                yield return CoroutineReturnManager.WaitForFixedUpdate;
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(_graphicDelay);

            Debug.Log("트리거");
            SequenceScript.TriggerFroceOn();
        }

        _morseIndexCheckCoroutine = null;


    }



    void Start()
    {
        morseImages = GetComponentsInChildren<MorseImage>();

        arduino_MorseKey = GetComponentInParent<Arduino_MorseKey>();

        // foreach (MorseImage morseImage in morseImages)
        // {
        //     morseImage.SetTextures(arduino_MorseKey.DotTexture, arduino_MorseKey.DashTexture);
        // }
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
