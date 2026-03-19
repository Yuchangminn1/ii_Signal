using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITCP
{
    void SendData(string data);
    bool IsConnected { get; }
}


public class NetworkManager : Singleton<NetworkManager>, IJsonGenericTarget, ITCP
{

    [Header("Select Network Role")]
    JsonGenericUpData _genericData = new JsonGenericUpData();

    public bool EndWait = false;

    Coroutine _requestCoroutine = null;
    public bool IsTutorialRead = false;

    ITCP tcpComponent;

    public bool ResetRequested { get; set; } = false;

    bool _isServer = false;

    public bool IsServer => _isServer;

    // 네트워크 연결 상태 추적
    private bool _isConnected = false;
    public bool IsConnected
    {
        get => _isConnected;
        private set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                if (!value)
                {
                    OnConnectionLost();
                }
            }
        }
    }

    string ipAddress = "";

    Coroutine toggleCoroutine = null;

    public void ChangeIsTutorialRead()
    {
        if (toggleCoroutine != null)
            StopCoroutine(toggleCoroutine);
        toggleCoroutine = StartCoroutine(ToggleCoroutine());
    }


    IEnumerator ToggleCoroutine()
    {
        IsTutorialRead = true;

        yield return new WaitForSeconds(0.5f);
        IsTutorialRead = false;

        toggleCoroutine = null;


    }




    public void SelectPosition(bool isServer)
    {
        if (isServer)
        {
            if (GetComponent<SimpleTcpServer>() == null)
            {
                tcpComponent = gameObject.AddComponent<SimpleTcpServer>();
            }
            Debug.Log("TCP - Server ");
        }
        else
        {
            if (GetComponent<SimpleTcpClient>() == null)
            {
                tcpComponent = gameObject.AddComponent<SimpleTcpClient>();
                SimpleTcpClient tmp = GetComponent<SimpleTcpClient>();
                tmp.ServerIp = ipAddress;
                tmp.ConnectToTcpServer();
            }
            Debug.Log($"TCP - Client : {ipAddress}");
        }
    }

    public void Initialize(JsonGenericUpData data)
    {
        _genericData = data;
        data.stringParams.TryGetValue("ipAddress", out ipAddress);
        if (string.IsNullOrEmpty(ipAddress))
        {
            Debug.LogError("제이슨 IP 주소가 비어있음");
            return;
        }
        _genericData.stringParams["ipAddress"] = ipAddress;
        _genericData.boolParams.TryGetValue("isServer", out _isServer);

        SelectPosition(_isServer);
    }
    public JsonGenericUpData Data()
    {
        _genericData.intParams = new Dictionary<string, int>();
        _genericData.floatParams = new Dictionary<string, float>();
        _genericData.boolParams = new Dictionary<string, bool>();
        _genericData.stringParams = new Dictionary<string, string>();

        _genericData.boolParams["isServer"] = _isServer;
        _genericData.stringParams["ipAddress"] = _isServer ? "Server IP" : "Client IP";
        return _genericData;
    }

    // public void SetEndWait()
    // {
    //     if (IsServer)
    //         EndWait = true;
    // }

    public void SendData(string data)
    {
        if (!IsConnected)
        {
            Debug.LogWarning($"[TCP] 연결 상태가 아닙니다. 데이터 전송 불가: {data}");
            return;
        }

        if (tcpComponent != null)
        {
            tcpComponent.SendData(data);
        }
        else
        {
            Debug.LogError("TCP 컴포넌트가 없습니다.");
        }
    }

    /// <summary>
    /// TCP 연결이 성공했을 때 호출 (TCP 컴포넌트에서 호출)
    /// </summary>
    public void OnConnectionEstablished()
    {
        IsConnected = true;
        Debug.Log("[TCP] 연결 성공");
    }

    /// <summary>
    /// TCP 연결이 끊어졌을 때 처리
    /// </summary>
    private void OnConnectionLost()
    {
        Debug.LogError("[TCP] 네트워크 연결이 끊어졌습니다!");

        // 진행 중인 Coroutine 중단
        if (_requestCoroutine != null)
        {
            StopCoroutine(_requestCoroutine);
            _requestCoroutine = null;
        }

        if (toggleCoroutine != null)
        {
            StopCoroutine(toggleCoroutine);
            toggleCoroutine = null;
        }

        // 게임 상태 초기화
        EndWait = false;
        ResetRequested = false;
    }

    /// <summary>
    /// TCP 연결이 끊어졌을 때 외부에서 명시적으로 호출
    /// </summary>
    public void SetConnectionLost()
    {
        IsConnected = false;
    }



    public void StopEndResetRequest()
    {
        if (_requestCoroutine != null)
        {
            StopCoroutine(_requestCoroutine);
            _requestCoroutine = null;
        }
    }

    public void EndNReset()
    {
        SendData("EReset");
        EndWait = false;
    }

    // public void EndResetRequest()
    // {
    //     if (IsServer == false)
    //     {
    //         if (_requestCoroutine == null)
    //             _requestCoroutine = StartCoroutine(EndResetRequestCoroutine());
    //     }

    // }
    // IEnumerator EndResetRequestCoroutine()
    // {
    //     yield return CoroutineReturnManager.GetWaitForSeconds(5f);

    //     while (true)
    //     {
    //         SendData("End");
    //         yield return CoroutineReturnManager.GetWaitForSeconds(1f);
    //     }

    // }



}
