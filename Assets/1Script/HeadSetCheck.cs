using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSetCheck : MonoBehaviour
{
    SequenceScript sequenceScript;
    void Start()
    {
        sequenceScript = GetComponent<SequenceScript>();
        FindObjectOfType<Arduino_SelectButton>()._onButtonPressed += CheckStart;
    }

    public void CheckStart()
    {
        if (gameObject.activeInHierarchy == false)
            return;
        sequenceScript?.TriggerFroceOn();

    }

    public void SendGo()
    {
        if (NetworkManager.Instance.IsServer == false)
            NetworkManager.Instance.SendData("Go");
    }
}
