using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NameText : MonoBehaviour
{

    protected Text _text;

    protected string originText = "";



    void Start()
    {
        _text = GetComponent<Text>();
        originText = _text.text;

    }


    protected virtual void OnEnable()
    {
        if (_text == null)
            return;
        if (originText == "")
        {
            _text.text = originText;
        }

        if (UserDataManager.Instance.GetPlayer() != null)
        {
            SetText(originText);
        }
    }
    public Text GetTextComponent()
    {
        return _text;
    }

    public virtual void SetText(string textData = "")
    {

        if (GameManager.Instance.IsStarted == false || _text == null || UserDataManager.Instance.IsUser() == false)
        {
            return;
        }

        if (textData == "")
        {
            textData = originText;
        }
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

    }


}
