using System;
using System.Collections;
using System.IO.Ports;
using UnityEngine;

public class Arduino : MonoBehaviour
{
    protected Action<Arduino> _onArduinoStart;


    protected bool isStop = false;


    protected bool _isRunning = false;

    public string WaitResponse = "";

    public int PlayerIndex = 0;
    public string[] SerialPortNames =
    {
        "COM101"
    };
    protected SerialPort stream;
    protected virtual void Start()
    {
        stream = FindDevicePort();
    }

    virtual public void StartArduino()
    {
        if (GameManager.Instance.IsStarted == false)
            return;
        if (stream == null)
        {
            Debug.LogError("SerialPort가 초기화되지 않았습니다.");
            return;
        }
        try
        {
            if (!stream.IsOpen)
            {
                stream.Open();
                Debug.Log("시리얼 포트 열림: " + stream.PortName + " / " + PlayerIndex);
            }
            _isRunning = true;
            StartCoroutine(ReadMessage());

        }
        catch (Exception e)
        {
            Debug.LogError(" / " + PlayerIndex + " / " + stream.PortName);
            Debug.LogError("시리얼 포트를 여는 중 오류 발생: " + e.Message + " / " + stream.PortName);
            return;
        }
        _onArduinoStart?.Invoke(this);


    }

    public SerialPort FindDevicePort(int baudRate = 9600, int timeout = 5000)
    {
        string[] ports = SerialPort.GetPortNames();

        foreach (string portName in ports)
        {
            Debug.Log("포트 탐색 중: " + portName + " / " + WaitResponse);
            SerialPort port = new SerialPort(portName, baudRate);
            port.ReadTimeout = timeout;
            port.WriteTimeout = timeout;

            try
            {
                port.Open();

                port.DiscardInBuffer();
                port.DiscardOutBuffer();

                string response = port.ReadLine();
                if (response != null)
                    Debug.Log($"포트 {portName}에서 응답 받음: {response}");

                if (response.Contains(WaitResponse))
                {
                    Debug.Log($"연결 연결이요.. {portName}");
                    _isRunning = true;
                    StartCoroutine(DelayTOStart());
                    return port; // 유지
                }

                port.Close();
            }
            catch (Exception)
            {
                if (port.IsOpen)
                    port.Close();
            }
        }

        Debug.LogWarning("Device not found");
        return null;
    }

    IEnumerator DelayTOStart()
    {
        yield return CoroutineReturnManager.GetWaitForSeconds(1f);
        _isRunning = true;
        StartCoroutine(ReadMessage());
    }



    public void AddOnCheckStartListener(Action<Arduino> listener)
    {
        _onArduinoStart += listener;
    }
    public virtual void StopArduino()
    {
        _isRunning = false;

        stream.Close();
        Debug.Log("시리얼 포트 닫힘: " + stream.PortName);
    }
    protected void OnApplicationQuit()
    {
        if (stream.IsOpen) stream.Close();
    }
    protected IEnumerator ReadMessage()
    {
        string received;

        while (_isRunning)
        {
            if (IsReadingMessage())
            {
                if (stream.IsOpen && stream.BytesToRead > 0)
                {
                    //bool isInput = false;
                    received = "";
                    bool isError = false;

                    try
                    {
                        received = stream.ReadLine();
                        // Debug.Log("Received from Arduino: " + received);
                    }
                    catch (TimeoutException)
                    {
                        Debug.LogError("타임아웃 발생 ");
                        isError = true;
                    }
                    catch (Exception e)
                    {
                        // 타임아웃 외의 다른 에러(연결 끊김 등) 처리
                        Debug.LogError("오류 발생: " + e.Message);
                        isError = true;
                        break;
                    }
                    if (isError)
                    {
                        yield return CoroutineReturnManager.GetWaitForSeconds(1f);
                    }
                    ReadMessageProcess(received);
                }
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(0.15f);
        }
    }

    protected IEnumerator SendMessage()
    {
        while (_isRunning)
        {
            // 숫자 1키를 누르면 긴 문장을 보냄

            if (IsSendingMessage())
            {
                SendMessageProcess();
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(0.15f);
        }
    }



    /// <summary>
    /// 메세지 전송 지속 조건 
    /// </summary>
    /// <returns></returns>
    virtual protected bool IsSendingMessage()
    {
        return true;
    }
    /// <summary>
    /// 메세지 전송 로직
    /// </summary>
    virtual public void SendMessageProcess()
    {
        ;
    }
    /// <summary>
    /// 메세지 리드 지속 조건 
    /// </summary>
    /// <returns></returns>
    virtual protected bool IsReadingMessage()
    {
        return true;
    }
    /// <summary>
    /// 메세지 리드 로직
    /// </summary>
    /// <param name="received"></param>
    virtual public void ReadMessageProcess(string received)
    {
        ;
    }
}
