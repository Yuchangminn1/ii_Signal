using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Arduino_SelectButton : Arduino
{

    public Direction ButtonDirection;
    public string _onMessage = "Btn_On";
    public string _offMessage = "Off";
    public Action _onButtonPressed;

    override protected bool IsReadingMessage()
    {
        return true;
    }

    override public void ReadMessageProcess(string received)
    {
        if (!string.IsNullOrEmpty(received) && received.Contains("Btn"))
        {
            _onButtonPressed?.Invoke();
            if (received.Contains("On"))
                SoundManager.Instance.PlayEffectSound(EffectSoundNum.ArduinoButtonSound);
            Debug.Log($"버튼 눌림 : {ButtonDirection}");
            GameManager.Instance.GoToIdleCheck();
            NetworkManager.Instance.SendData($"B");
        }

        else
        {
            Debug.Log($"없음 {received} ");
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            _onButtonPressed?.Invoke();
        }
    }

    protected override void Start()
    {
        WaitResponse = "Arduino_Btn";

        base.Start();
    }


    override protected bool IsSendingMessage()
    {
        return false;
    }
    override public void SendMessageProcess()
    {
        ;
    }
}
