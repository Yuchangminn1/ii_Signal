using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Morse_Guide : MonoBehaviour
{


    MorseColoringImage[] _morseColoringImages;

    Queue<MorseType> _morseInput = new Queue<MorseType>();
    Arduino_MorseKey arduino_MorseKey;

    public SequenceScript SequenceScript;

    public string MorseData = "0";

    float _graphicDelay = 1.2f;



    Coroutine _morseIndexCheckCoroutine = null;


    int _currentIndex = 0;


    void OnEnable()
    {
        if (GameManager.Instance.IsStarted == false)
            return;
        _morseIndexCheckCoroutine = null;
        if (arduino_MorseKey != null)
            arduino_MorseKey.StopMorseCheck();

        StartCoroutine(BackUp());

    }

    IEnumerator BackUp()
    {
        if (NetworkManager.Instance.IsServer)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.5f);
                NetworkManager.Instance.SendData("Go");
            }
        }

    }

    public void CheckStart()
    {
        for (int i = 0; i < _morseColoringImages.Length; i++)
        {
            if (MorseData[i] == '0')
                _morseColoringImages[i].SetMorseType(MorseType.Dot);
            else if (MorseData[i] == '1')
                _morseColoringImages[i].SetMorseType(MorseType.Dash);

        }


        if (arduino_MorseKey == null)
        {
            return;
        }
        if (arduino_MorseKey != null)
        {
            arduino_MorseKey.AddOnMorseInput(ColoringMorseImage);
            arduino_MorseKey.OnReset += Reset;
            arduino_MorseKey.IsGuide = true;

        }
        ResetValue();

        arduino_MorseKey.StartMorseCheck();


    }

    private void ResetValue()
    {
        _currentIndex = 0;
        _morseInput.Clear();
    }

    public void Reset()
    {
        //ResetValue();
        ;
    }


    IEnumerator MorseIndexCheckCoroutine(MorseType morseType)
    {
        // if (morseType == MorseType.Dot)
        //     arduino_MorseKey.PlayMorseSound(MorseType.Dot);
        // else if (morseType == MorseType.Dash)
        //     arduino_MorseKey.PlayMorseSound(MorseType.Dash);




        while (_morseColoringImages[_currentIndex].IsCheck == false)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }


        _currentIndex++;

        if (_currentIndex == _morseColoringImages.Length)
        {
            arduino_MorseKey.StopMorseCheck();

            while (_morseColoringImages[_currentIndex - 1].IsCheck == false)
            {
                yield return CoroutineReturnManager.WaitForFixedUpdate;
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(_graphicDelay);

            Debug.Log("MorseIndexCheckCoroutine / 트리거");
            SequenceScript.TriggerForceOn();
        }
        if (_currentIndex < _morseColoringImages.Length && _morseInput.Count > 0)
        {
            StartCoroutine(InputDequeue());
        }

        _morseIndexCheckCoroutine = null;


    }
    IEnumerator InputDequeue()
    {
        if (_morseInput.Count > 0)
        {
            yield return CoroutineReturnManager.WaitForFixedUpdate;

            ColoringMorseImage(_morseInput.Dequeue());
        }

    }


    void Start()
    {
        _morseColoringImages = GetComponentsInChildren<MorseColoringImage>();

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

        if (_currentIndex >= _morseColoringImages.Length)
        {
            return;
        }
        if (_morseColoringImages[_currentIndex].CurrentMorseType == morseType)
        {
            _morseColoringImages[_currentIndex].StartColoring();
            if (_morseIndexCheckCoroutine == null)
                _morseIndexCheckCoroutine = StartCoroutine(MorseIndexCheckCoroutine(morseType));
            else
            {
                //Debug.Log($"코루틴 돌리는중 추가입력 {morseType} 큐에 추가");
                _morseInput.Enqueue(morseType);
            }
        }

    }

}
