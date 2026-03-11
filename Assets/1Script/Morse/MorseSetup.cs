using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum InputSymbolGapDuration2
{
    Dot_Dot = 125,
    Dot_Dash = 223,
    Dash_Dash = 257
}
public class MorseSetup : MonoBehaviour
{
    MorseColoringImage[] _morseColoringImage;

    Arduino_MorseKey arduino_MorseKey;

    public SequenceScript sequenceScript;

    public Text RateText;

    public Text ResultText;


    public CanvasGroup rateTextCanvasgroup;

    Coroutine _coloringCheckCoroutine = null;

    Coroutine _hindSoundCoroutine = null;

    string[] rateTextList = new string[3] { "% 일치", "% 일치1", "% 일치2" };
    string[] resultTextList = new string[2] { "합니다. 다시 입력해 주세요.", "합니다." };

    int _currentIndex = 0;
    string _morseData = "0";
    void Start()
    {
        _morseColoringImage = GetComponentsInChildren<MorseColoringImage>();

        arduino_MorseKey = GetComponentInParent<Arduino_MorseKey>();


    }




    public void CheckStart()
    {
        arduino_MorseKey.IsAccuracyRateCheck = true;
        arduino_MorseKey.OnAccuracyCheckAction += AccuracyCheck;
        if (_hindSoundCoroutine != null)
        {
            StopCoroutine(_hindSoundCoroutine);
        }
        _hindSoundCoroutine = StartCoroutine(PlayMorseHintSoundCorotuine());
        _morseData = UserDataManager.Instance.GetPlayer().PartnerPassCode;
        if (_morseData == "")
            return;

        for (int i = 0; i < _morseColoringImage.Length; i++)
        {
            if (_morseData[i] == '0')
                _morseColoringImage[i].SetMorseType(MorseType.Dot);
            else if (_morseData[i] == '1')
                _morseColoringImage[i].SetMorseType(MorseType.Dash);

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

    IEnumerator PlayMorseHintSoundCorotuine()
    {
        if (_morseData == "")
        {
            Debug.Log("_morseData IS Empty");
            yield break;
        }

        while (true)
        {
            for (int i = 0; i < _morseData.Length; i++)
            {
                if (_morseData[i] == '0')
                {
                    SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDotSound_1);
                    yield return CoroutineReturnManager.GetWaitForSeconds(MorseTranslator.DefaultDotTime);
                    SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDotSound_1);


                    yield return CoroutineReturnManager.GetWaitForSeconds(0.2f);


                }
                else if (_morseData[i] == '1')
                {
                    SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDashSound_1);
                    yield return CoroutineReturnManager.GetWaitForSeconds(MorseTranslator.DefaultDashTime);
                    SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDashSound_1);
                    yield return CoroutineReturnManager.GetWaitForSeconds(0.2f);
                }
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(3f);
        }


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
        string q = "";
        if (rate < 10f)
        {
            q += "  ";
        }
        else if (rate < 100f)
        {
            q += "  ";
        }
        RateText.text = q + rate.ToString("F0") + rateTextList[_currentIndex];
        if (rate < 80f)
        {
            ResultText.text = resultTextList[0];
            arduino_MorseKey.Reset();

        }
        else
        {
            StopCoroutine(_hindSoundCoroutine);
            SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDashSound_1);
            SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDotSound_1);

            StartCoroutine(DelayToPlay());
        }
    }

    IEnumerator DelayToPlay()
    {
        yield return CoroutineReturnManager.GetWaitForSeconds(1.0f);

        ResultText.text = resultTextList[1];

        ColoringMorseImage();
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
        for (int i = 0; i < _morseColoringImage.Length; i++)
        {
            if (_morseData[i] == '0')
            {
                arduino_MorseKey.PlayMorseSound(MorseType.Dot);
            }
            else if (_morseData[i] == '1')
            {
                arduino_MorseKey.PlayMorseSound(MorseType.Dash);
            }
            _morseColoringImage[i].StartColoring();
            yield return CoroutineReturnManager.WaitForFixedUpdate;

            while (_morseColoringImage[i].IsCheck == false)
            {
                yield return CoroutineReturnManager.WaitForFixedUpdate;
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(0.25f);

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
