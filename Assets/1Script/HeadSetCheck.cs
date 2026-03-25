using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSetCheck : MonoBehaviour
{
    SequenceScript sequenceScript;
    public Arduino_SelectButton _selectButton;

    bool Ison = false;

    void Start()
    {

    }

    void OnEnable()

    {
        sequenceScript = GetComponent<SequenceScript>();

        if (_selectButton != null)
        {
            _selectButton._onButtonPressed += CheckStart;
        }
        else
        {
            Debug.LogWarning("HeadSetCheck: Arduino_SelectButton을 찾지 못했습니다.");
        }
        Ison = false;
    }

    void OnDestroy()
    {
        if (_selectButton != null)
            _selectButton._onButtonPressed -= CheckStart;
    }

    public void IsOn()
    {
        Ison = true;

    }

    public void CheckStart()
    {
        Debug.Log("CheckStart 호출됨");
        if (Ison == false)
        {
            Debug.Log("Ison == false");
            return;

        }

        if (gameObject.activeInHierarchy == false)
        {
            Debug.Log("gameObject.activeInHierarchy == false");
            return;


        }
        sequenceScript?.TriggerForceOn();

    }

    public void SendGo()
    {
        if (NetworkManager.Instance.IsServer == false)
            NetworkManager.Instance.SendData("Go");
    }
}
