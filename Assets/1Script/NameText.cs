using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NameText : MonoBehaviour
{

    Text _text;

    string originText = "";



    string currentText = "";



    void Start()
    {
        _text = GetComponent<Text>();
        currentText = _text.text;
        _text.text = "";
    }


    void OnEnable()
    {
        if (originText == "")
            originText = currentText;


        if (UserDataManager.Instance.GetPlayer() != null)
        {
            SetText(originText);
        }
    }

    public Text GetTextComponent()
    {
        return _text;
    }

    public void SetText(string textData = "")
    {
        if (textData != "")
            _text.text = textData.Replace("Name", UserDataManager.Instance.GetPlayer().FirstName);

        else if (_text.text == "")
            _text.text = currentText.Replace("Name", UserDataManager.Instance.GetPlayer().FirstName);
    }


}
