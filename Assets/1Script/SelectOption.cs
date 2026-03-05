using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum SelectSymbolGapDuration
{
    Dot_Dot = 49,
    Dot_Dash = 66,
    Dash_Dash = 78
}

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

    Text _text;
    Color32 _normalColor = new Color32(66, 66, 66, 255);

    Color32 _selectedColor = new Color32(141, 118, 178, 255);

    QuestionSelectTextContainer _questionSelectTextContainer;
    SelectSymbolGapDuration[] _dotDashArray = new SelectSymbolGapDuration[GAP_COUNT];

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
                _rawImages[i].texture = _questionSelectTextContainer.DotTexture;

            }
            else if (_morseValue[i] == '1')
            {
                _rawImages[i].texture = _questionSelectTextContainer.DashTexture;
            }
            if (_dotDashArray.Length > i)
            {
                if ((_morseValue[i] == '0' && _morseValue[i + 1] == '1') || (_morseValue[i] == '1' && _morseValue[i + 1] == '0'))
                {
                    _dotDashArray[i] = SelectSymbolGapDuration.Dot_Dash;
                }
                else if (_morseValue[i] == '1' && _morseValue[i + 1] == '1')
                {
                    _dotDashArray[i] = SelectSymbolGapDuration.Dash_Dash;
                }
                else if (_morseValue[i] == '0' && _morseValue[i + 1] == '0')
                {
                    _dotDashArray[i] = SelectSymbolGapDuration.Dot_Dot;
                }

            }

            _rawImages[i].SetNativeSize();
        }
        if (_text != null)
            _text.color = _normalColor;

        float totalX = 0;
        foreach (SelectSymbolGapDuration gap in _dotDashArray)
        {
            totalX += (float)gap;
        }


        Vector3 startPos = Vector3.right * totalX / -2f;

        _rawImagesRects[0].localPosition = startPos;
        _rawImagesRects[_rawImagesRects.Length - 1].localPosition = startPos * -1f;
        //        Debug.Log($"{name} /  totalX : " + totalX + " start : " + startPos.x + " end : " + startPos * -1f);

        for (int i = 1; i < _rawImagesRects.Length - 1; i++)
        {
            float gapX = (float)_dotDashArray[i - 1];
            startPos += Vector3.right * gapX;
            _rawImagesRects[i].localPosition = startPos;
            // Debug.Log($"_rawImagesRects[{i}] pos : " + startPos);
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
                    _rawImages[i].texture = _questionSelectTextContainer.DotTexture;
                }
                else if (_morseValue[i] == '1')
                {
                    _rawImages[i].texture = _questionSelectTextContainer.DashTexture;
                }
                _rawImages[i].color = _normalColor;

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
                _rawImages[i].color = _selectedColor;
            }
        }
        if (_text != null)
            _text.color = _selectedColor;
    }


}
