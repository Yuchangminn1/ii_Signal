using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameText : MonoBehaviour
{
    Text _text;


    string currentText = "";

    bool isTextSet = false;


    void Start()
    {
        _text = GetComponent<Text>();
        currentText = _text.text;
        _text.text = "";
    }


    void OnEnable()
    {
        if (PlayerData.Instance.GetPlayer() != null)
        {
            SetText();
        }
    }

    public Text GetTextComponent()
    {
        return _text;
    }

    public void SetText(string textData = "")
    {
        if (textData != "")
            _text.text = textData.Replace("Name", PlayerData.Instance.GetPlayer().Name);

        else if (_text.text == "")
            _text.text = currentText.Replace("Name", PlayerData.Instance.GetPlayer().Name);
    }


}
