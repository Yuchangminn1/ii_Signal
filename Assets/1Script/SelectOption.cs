using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOption : MonoBehaviour
{
    public string _morseValue = "";

    public string MorseValue
    {
        get { return _morseValue; }
        set { _morseValue = value; }
    }


    RawImage[] _rawImages;
    RectTransform[] _rawImagesRects;
    const int GAP_COUNT = 3;

    const float DEFAULT_X_GAP = 52f;
    const float DEFAULT_DASH_GAP = 66f;
    Text _text;
    Color32 _normalColor = new Color32(66, 66, 66, 255);

    Color32 _selectedColor = new Color32(141, 118, 178, 255);

    QuestionSelectTextContainer _questionSelectTextContainer;
    bool[] _dotDashArray = new bool[GAP_COUNT];

    public void Initialize(string text)
    {
        if (_text != null)
            _text.text = text;
    }


    void Start()
    {
        _rawImages = GetComponentsInChildren<RawImage>();
        _questionSelectTextContainer = GetComponentInParent<QuestionSelectTextContainer>();
        _text = GetComponentInChildren<Text>();
        _rawImagesRects = new RectTransform[_rawImages.Length];
        for (int i = 0; i < _rawImages.Length; i++)
        {
            _rawImagesRects[i] = _rawImages[i].GetComponent<RectTransform>();
        }
    }

    void OnEnable()
    {
        Reset();
    }

    public void SetPattern(string morseValue)
    {
        _morseValue = morseValue;
        for (int i = 0; i < _morseValue.Length; i++)
        {
            if (_morseValue[i] == '0')
            {
                _rawImages[i].texture = _questionSelectTextContainer.NormalDotTexture;

            }
            else if (_morseValue[i] == '1')
            {
                _rawImages[i].texture = _questionSelectTextContainer.NormalDashTexture;
            }
            _rawImages[i].SetNativeSize();
        }
        if (_text != null)
            _text.color = _normalColor;




        for (int i = 0; i < GAP_COUNT; i++)
        {
            _dotDashArray[i] = false;
        }

        int count = 0;

        for (int i = 0; i < _morseValue.Length; i++)
        {
            if (_morseValue[i] == '1')
            {
                if (i - 1 >= 0)
                    _dotDashArray[i - 1] = true;
                if (i < _dotDashArray.Length)
                    _dotDashArray[i] = true;

            }
        }


        foreach (bool dotDash in _dotDashArray)
        {
            if (dotDash)
            {
                count++;
            }
        }

        float totalX = (GAP_COUNT - count) * DEFAULT_X_GAP + count * DEFAULT_DASH_GAP;
        Debug.Log($"{name} /  totalX : " + totalX + " count : " + count);

        Vector3 startPos = Vector3.right * totalX / -2f;
        _rawImagesRects[0].localPosition = startPos;
        _rawImagesRects[_rawImagesRects.Length - 1].localPosition = startPos * -1f;

        for (int i = 1; i < _rawImagesRects.Length - 1; i++)
        {
            float gapX = DEFAULT_X_GAP;
            if (_dotDashArray[i - 1])
            {
                gapX = DEFAULT_DASH_GAP;
            }
            startPos += Vector3.right * gapX;
            _rawImagesRects[i].localPosition = startPos;
            Debug.Log($"_rawImagesRects[{i}] pos : " + startPos);
        }

    }


    public void Reset()
    {
        if (_rawImages != null)
        {
            for (int i = 0; i < _morseValue.Length; i++)
            {
                if (_morseValue[i] == '0')
                {
                    _rawImages[i].texture = _questionSelectTextContainer.NormalDotTexture;

                }
                else if (_morseValue[i] == '1')
                {
                    _rawImages[i].texture = _questionSelectTextContainer.NormalDashTexture;
                }
            }
        }
        if (_text != null)
            _text.color = _normalColor;

    }

    public void Select()
    {
        SoundManager.Instance.PlayEffectSound(EffectSoundNum.ActiveSound);
        if (_rawImages != null)
        {
            for (int i = 0; i < _rawImages.Length; i++)
            {
                if (_morseValue[i] == '0')
                {
                    _rawImages[i].texture = _questionSelectTextContainer.SelectedDotTexture;
                }
                else if (_morseValue[i] == '1')
                {
                    _rawImages[i].texture = _questionSelectTextContainer.SelectedDashTexture;
                }
            }
        }
        if (_text != null)
            _text.color = _selectedColor;
    }


}
