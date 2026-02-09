using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MorseSetup : MonoBehaviour
{
    MorseImage[] morseImages;

    Arduino_MorseKey arduino_MorseKey;

    public SequenceScript sequenceScript;

    public Text rateText;

    public CanvasGroup rateTextCanvasgroup;

    Coroutine _coloringCheckCoroutine = null;

    string[] rateTextList = new string[3] { "% 일치", "% 일치1", "% 일치2" };
    int _currentIndex = 0;
    string _morseData = "0";
    void Start()
    {
        morseImages = GetComponentsInChildren<MorseImage>();

        arduino_MorseKey = GetComponentInParent<Arduino_MorseKey>();

        // foreach (MorseImage morseImage in morseImages)
        // {
        //     morseImage.SetTextures(arduino_MorseKey.DotTexture, arduino_MorseKey.DashTexture);
        // }
    }




    public void CheckStart()
    {
        arduino_MorseKey.IsAccuracyRateCheck = true;
        arduino_MorseKey.OnAccuracyCheckAction += AccuracyCheck;
        _morseData = PlayerDatas.Instance.GetPlayer().PassCode;
        if (_morseData == "")
            return;

        for (int i = 0; i < morseImages.Length; i++)
        {
            if (_morseData[i] == '0')
                morseImages[i].SetMorseType(MorseType.Dot);
            else if (_morseData[i] == '1')
                morseImages[i].SetMorseType(MorseType.Dash);
        }


        if (arduino_MorseKey == null)
        {
            return;
        }
        if (arduino_MorseKey != null)
        {
            //arduino_MorseKey.AddOnMorseInput(ColoringMorseImage);
            arduino_MorseKey.OnReset += Reset;
        }
        _currentIndex = 0;


        arduino_MorseKey.StartMorseCheck();


    }

    void OnDisable()
    {

        if (arduino_MorseKey != null)
        {
            arduino_MorseKey.StopMorseCheck();
        }

    }

    public void AccuracyCheck(float rate)
    {
        rateTextCanvasgroup.alpha = 1f;
        rateText.text = rate.ToString("F0") + rateTextList[_currentIndex];
        if (rate < 50f)
            arduino_MorseKey.Reset();
        else
        {
            ColoringMorseImage();
        }
    }

    public void ColoringMorseImage()
    {
        if (_coloringCheckCoroutine != null)
        {
            StopCoroutine(_coloringCheckCoroutine);
            _coloringCheckCoroutine = null;
        }
        _coloringCheckCoroutine = StartCoroutine(ColoringMorseImageCorotuine());
    }
    public IEnumerator ColoringMorseImageCorotuine()
    {
        for (int i = 0; i < morseImages.Length; i++)
        {
            if (_morseData[i] == '0')
            {
                arduino_MorseKey.PlayMorseSound(MorseType.Dot);
            }
            else if (_morseData[i] == '1')
            {
                arduino_MorseKey.PlayMorseSound(MorseType.Dash);
            }
            morseImages[i].StartColoring();
            while (morseImages[i].IsCheck == false)
            {
                yield return CoroutineReturnManager.WaitForFixedUpdate;
            }
        }
        yield return CoroutineReturnManager.GetWaitForSeconds(1.5f);
        sequenceScript.TriggerOn();
        _coloringCheckCoroutine = null;
    }


    public void Reset()
    {
        Debug.Log("Reset");

    }

}
