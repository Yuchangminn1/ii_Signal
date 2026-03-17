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


    public Text RetryGuideText;


    Color32[] acTextColor = new Color32[2] { new Color32(49, 251, 0, 255), new Color32(255, 20, 20, 255) };



    public CanvasGroup rateTextCanvasgroup;

    CanvasGroup _canvasgroup;

    Coroutine _coloringCheckCoroutine = null;

    Coroutine _hindSoundCoroutine = null;


    int _currentIndex = 0;
    string _morseData = "0";
    void Start()
    {
        _morseColoringImage = GetComponentsInChildren<MorseColoringImage>();

        arduino_MorseKey = GetComponentInParent<Arduino_MorseKey>();



        _canvasgroup = GetComponent<CanvasGroup>();

    }

    public void PlayMorseHintSound()
    {
        if (_hindSoundCoroutine != null)
        {
            StopCoroutine(_hindSoundCoroutine);
            _hindSoundCoroutine = null;
        }
        _hindSoundCoroutine = StartCoroutine(PlayMorseHintSoundCorotuine());
    }




    public void CheckStart()
    {
        arduino_MorseKey.IsAccuracyRateCheck = true;
        arduino_MorseKey.OnAccuracyCheckAction += AccuracyCheck;

        _morseData = UserDataManager.Instance.GetPlayer().PartnerPassCode;

        for (int i = 0; i < _morseColoringImage.Length; i++)
        {
            if (_morseData[i] == '0')
                _morseColoringImage[i].SetMorseType(MorseType.Dot);
            else if (_morseData[i] == '1')
                _morseColoringImage[i].SetMorseType(MorseType.Dash);
        }

        PlayMorseHintSound();
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

    public void StopCheck()
    {
        arduino_MorseKey.IsAccuracyRateCheck = false;
        arduino_MorseKey.OnAccuracyCheckAction -= AccuracyCheck;
        if (arduino_MorseKey != null)
        {
            //arduino_MorseKey.RemoveOnMorseInput(ColoringMorseImage);
            arduino_MorseKey.OnReset -= Reset;
            arduino_MorseKey.StopMorseCheck();
        }


    }

    IEnumerator PlayMorseHintSoundCorotuine()
    {
        if (_morseData == "")
        {
            Debug.Log("_morseData IS Empty");
            yield break;
        }
        SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDotSound_1);
        SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDashSound_1);
        yield return CoroutineReturnManager.GetWaitForSeconds(0.5f);

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


        string q = "";
        if (rate < 10f)
        {
            q += "  ";
        }
        else if (rate < 100f)
        {
            q += " ";
        }
        if (rate < 80f)
        {
            StartCoroutine(rateFailedCoroutine(q, rate));
        }
        else
        {

            if (_hindSoundCoroutine != null)
            {
                StopCoroutine(_hindSoundCoroutine);
                _hindSoundCoroutine = null;
            }
            FadeManager.Instance.SetAlphaZero(RetryGuideText);

            rateTextCanvasgroup.alpha = 1f;
            RateText.color = acTextColor[0];
            RateText.text = q + rate.ToString("F0") + "%";

            SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDashSound_1);
            SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDotSound_1);

            StartCoroutine(DelayToPlay());
        }
    }

    public IEnumerator rateFailedCoroutine(string q, float rate)
    {


        StopCheck();
        arduino_MorseKey.Reset();
        _canvasgroup.alpha = 1f;
        rateTextCanvasgroup.alpha = 1f;
        RateText.color = acTextColor[1];
        RateText.text = q + rate.ToString("F0") + "%";
        yield return CoroutineReturnManager.GetWaitForSeconds(2.0f);
        rateTextCanvasgroup.alpha = 0f;
        FadeManager.Instance.SetAlphaOne(RetryGuideText);
        yield return CoroutineReturnManager.GetWaitForSeconds(1.0f);
        FadeManager.Instance.SetAlphaZero(RetryGuideText);



        CheckStart();


    }

    IEnumerator DelayToPlay()
    {
        _canvasgroup.alpha = 1f;

        yield return CoroutineReturnManager.GetWaitForSeconds(1.0f);

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
            // if (_morseData[i] == '0')
            // {
            //     arduino_MorseKey.PlayMorseSound(MorseType.Dot);
            // }
            // else if (_morseData[i] == '1')
            // {
            //     arduino_MorseKey.PlayMorseSound(MorseType.Dash);
            // }
            _morseColoringImage[i].StartColoring();
            yield return CoroutineReturnManager.WaitForFixedUpdate;

            while (_morseColoringImage[i].IsCheck == false)
            {
                yield return CoroutineReturnManager.WaitForFixedUpdate;
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(0.25f);

        }
        yield return CoroutineReturnManager.GetWaitForSeconds(2.5f);
        sequenceScript.TriggerOn();
        _coloringCheckCoroutine = null;
    }


    public void Reset()
    {
        Debug.Log("Reset");

    }

}
