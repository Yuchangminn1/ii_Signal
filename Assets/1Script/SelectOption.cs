using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOption : MonoBehaviour
{
    public string _morseValue = "";

    RawImage[] _rawImages;

    Text _text;
    Color32 _normalColor = new Color32(66, 66, 66, 255);

    Color32 _selectedColor = new Color32(141, 118, 178, 255);

    QuestionSelectTextContainer _questionSelectTextContainer;

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
    }


    void OnEnable()
    {
        Reset();
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
