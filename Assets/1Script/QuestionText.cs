using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionText : NameText
{
    protected override void OnEnable()
    {
        if (_text == null)
            return;
        if (originText == "")
        {
            _text.text = originText;
        }
    }

    public override void SetText(string textData = "")
    {

        if (GameManager.Instance.IsStarted == false || _text == null || UserDataManager.Instance.IsUser() == false)
        {
            return;
        }


        if (textData == "")
        {
            textData = originText;
        }

        textData = textData.Replace("\n", " ");

        if (textData.Contains("PartnerFullName"))
        {
            _text.text = textData.Replace("PartnerFullName", UserDataManager.Instance.GetPartnerPlayer().LastName + UserDataManager.Instance.GetPartnerPlayer().FirstName);

        }

        else if (textData.Contains("FullName"))
        {
            _text.text = textData.Replace("FullName", UserDataManager.Instance.GetPlayer().LastName + UserDataManager.Instance.GetPlayer().FirstName);
        }

        else if (textData.Contains("PartnerName"))
        {
            _text.text = textData.Replace("PartnerName", UserDataManager.Instance.GetPartnerPlayer().FirstName);
        }
        else if (textData.Contains("Name"))
        {
            _text.text = textData.Replace("Name", UserDataManager.Instance.GetPlayer().FirstName);
        }
        else
        {
            _text.text = textData;
        }

    }
}
