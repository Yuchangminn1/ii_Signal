using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Arduino : MonoBehaviour, IJsonGenericTarget
{
    protected Action<Arduino> _onArduinoStart;


    protected bool isStop = false;
    JsonGenericUpData _genericData = new JsonGenericUpData();


    protected bool _isRunning = false;

    public string WaitResponse = "";

    public int PlayerIndex = 0;
    public string SerialPortName = "COM101";

    protected SerialPort stream;
    protected Coroutine _readMessageCoroutine;
    protected virtual void Start()
    {
        // = FindDevicePort();
    }

    virtual public void StartArduino()
    {

        if (stream == null)
        {
            Debug.LogError("SerialPort가 초기화되지 않았습니다.");

            return;
        }
        try
        {
            if (stream.ReadTimeout == SerialPort.InfiniteTimeout)
                stream.ReadTimeout = 200;

            if (stream.WriteTimeout == SerialPort.InfiniteTimeout)
                stream.WriteTimeout = 200;

            if (!stream.IsOpen)
            {
                stream.Open();
                Debug.Log("시리얼 포트 열림: " + stream.PortName + " / " + PlayerIndex);
            }

            if (_isRunning)
                return;

            _isRunning = true;
            _readMessageCoroutine = StartCoroutine(ReadMessage());

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

        if (_readMessageCoroutine != null)
        {
            StopCoroutine(_readMessageCoroutine);
            _readMessageCoroutine = null;
        }

        if (stream == null)
            return;

        try
        {
            if (stream.IsOpen)
                stream.Close();
            Debug.Log("시리얼 포트 닫힘: " + stream.PortName);
        }
        catch (Exception e)
        {
            Debug.LogError("시리얼 포트 닫는 중 오류 발생: " + e.Message);
        }
    }
    protected void OnApplicationQuit()
    {
        StopArduino();
    }

    protected void OnDisable()
    {
        StopArduino();
    }
    protected IEnumerator ReadMessage()
    {
        string received;

        while (_isRunning)
        {
            if (stream == null)
                yield break;

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
                    else
                    {
                        try
                        {
                            ReadMessageProcess(received);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("ReadMessageProcess 처리 중 오류 발생: " + e.Message);
                        }
                    }
                }
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(0.15f);
        }

        _readMessageCoroutine = null;
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

    public void Initialize(JsonGenericUpData data)
    {
        if (data != null && data.stringParams != null
            && data.stringParams.TryGetValue("ButtonPortName", out var savedPortName)
            && string.IsNullOrWhiteSpace(savedPortName) == false)
        {
            SerialPortName = savedPortName;
        }

        if (string.IsNullOrWhiteSpace(SerialPortName))
        {
            Debug.LogError("ButtonPortName이 비어있어 Arduino 초기화를 중단합니다.");
            return;
        }

        stream = new SerialPort(SerialPortName, 9600)
        {
            ReadTimeout = 200,
            WriteTimeout = 200,
            NewLine = "\n"
        };

        StartArduino();

    }

    public JsonGenericUpData Data()
    {
        _genericData.intParams = new Dictionary<string, int>();
        _genericData.floatParams = new Dictionary<string, float>();
        _genericData.boolParams = new Dictionary<string, bool>();
        _genericData.stringParams = new Dictionary<string, string>();


        _genericData.stringParams["ButtonPortName"] = SerialPortName;
        return _genericData;
    }
}
