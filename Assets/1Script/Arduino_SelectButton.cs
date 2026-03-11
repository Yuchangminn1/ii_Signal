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
        if (received == _onMessage)
        {
            _onButtonPressed?.Invoke();
            SoundManager.Instance.PlayEffectSound(EffectSoundNum.ArduinoButtonSound);
            Debug.Log($"버튼 눌림 : {ButtonDirection}");
            GameManager.Instance.GoToIdleCheck();
            NetworkManager.Instance.SendData($"B");
        }

        else
        {
            Debug.Log($"{received} / 이건 무슨 입력이여 ?");
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
