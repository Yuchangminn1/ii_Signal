using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSelectorText : ResultSelector
{
    protected Text[] _text;

    public Text AdditionalText;

    protected override void Awake()
    {
        base.Awake();
        _text = GetComponentsInChildren<Text>();
    }

    public void SetText(string[] text)
    {
        for (int i = 0; i < _text.Length; i++)
        {
            _text[i].text = text[i];
        }
    }

    public override void Reset()
    {
        base.Reset();
        FadeManager.Instance.SetAlphaZero(_text);
    }

    public override void SelectAnswer(int selectIndex)
    {
        base.SelectAnswer(selectIndex);
        AdditionalText.text = _text[selectIndex].text;
        FadeManager.Instance.SetAlphaOne(_text[selectIndex]);

    }
}
