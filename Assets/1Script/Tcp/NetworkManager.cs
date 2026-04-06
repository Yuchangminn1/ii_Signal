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

    public enum ConnectionSyncState
    {
        Connected,
        Reconnecting,
        Resyncing
    }

    [Header("Select Network Role")]
    JsonGenericUpData _genericData = new JsonGenericUpData();

    public bool EndWait = false;

    Coroutine _requestCoroutine = null;
    public bool IsTutorialRead = false;

    ITCP tcpComponent;

    private readonly Queue<string> deferredControlMessages = new Queue<string>();
    const int maxDeferredControlMessages = 20;

    ConnectionSyncState _syncState = ConnectionSyncState.Reconnecting;
    public ConnectionSyncState SyncState => _syncState;

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
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        bool isProtocol = data == "STATE_REQ"
            || data == "RESUME_OK"
            || data.StartsWith("HELLO|", System.StringComparison.Ordinal)
            || data.StartsWith("STATE|", System.StringComparison.Ordinal);

        bool isDeferredControl = data == "Go"
            || data == "EReset"
            || data == "Reset"
            || data == "End";

        if (!IsConnected)
        {
            if (isDeferredControl)
            {
                if (deferredControlMessages.Count >= maxDeferredControlMessages)
                {
                    deferredControlMessages.Dequeue();
                }
                deferredControlMessages.Enqueue(data);
            }
            Debug.LogWarning($"[TCP] 연결 상태가 아닙니다. 데이터 전송 불가: {data}");
            return;
        }

        if (!isProtocol && _syncState != ConnectionSyncState.Connected)
        {
            if (isDeferredControl)
            {
                if (deferredControlMessages.Count >= maxDeferredControlMessages)
                {
                    deferredControlMessages.Dequeue();
                }
                deferredControlMessages.Enqueue(data);
                Debug.Log($"[TCP] 재동기화 중 제어 메시지 보류: {data}");
            }
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
        _syncState = ConnectionSyncState.Resyncing;
        Debug.Log("[TCP] 연결 성공 - 재동기화 시작");

        string role = IsServer ? "S" : "C";
        SendData($"HELLO|{role}");

        if (!IsServer)
        {
            SendData("STATE_REQ");
        }
    }

    /// <summary>
    /// TCP 연결이 끊어졌을 때 처리
    /// </summary>
    private void OnConnectionLost()
    {
        _syncState = ConnectionSyncState.Reconnecting;
        Debug.LogError("[TCP] 네트워크 연결이 끊어졌습니다! 재연결 대기 중");

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

        // 상태는 유지하고 재연결 후 스냅샷으로 다시 맞춘다.
    }

    /// <summary>
    /// TCP 연결이 끊어졌을 때 외부에서 명시적으로 호출
    /// </summary>
    public void SetConnectionLost()
    {
        IsConnected = false;
    }

    public bool TryHandleSyncMessage(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return false;
        }

        if (data.StartsWith("HELLO|", System.StringComparison.Ordinal))
        {
            if (IsServer)
            {
                SendStateSnapshot();
            }
            return true;
        }

        if (data.Equals("STATE_REQ", System.StringComparison.OrdinalIgnoreCase))
        {
            SendStateSnapshot();
            return true;
        }

        if (data.StartsWith("STATE|", System.StringComparison.Ordinal))
        {
            ApplyStateSnapshot(data);
            SendData("RESUME_OK");
            MarkResyncComplete();
            return true;
        }

        if (data.Equals("RESUME_OK", System.StringComparison.OrdinalIgnoreCase))
        {
            MarkResyncComplete();
            return true;
        }

        return false;
    }

    public void RequestStateSync()
    {
        if (!IsConnected)
        {
            return;
        }

        _syncState = ConnectionSyncState.Resyncing;
        SendData("STATE_REQ");
    }

    void SendStateSnapshot()
    {
        int page = 0;
        if (PageController.Instance != null)
        {
            page = PageController.Instance.CurrentPage;
        }

        bool contentEnd = false;
        if (UserDataManager.Instance != null)
        {
            contentEnd = UserDataManager.Instance.IsContentEnd;
        }

        string state = $"STATE|{page}|{(IsTutorialRead ? 1 : 0)}|{(ResetRequested ? 1 : 0)}|{(EndWait ? 1 : 0)}|{(contentEnd ? 1 : 0)}";
        SendData(state);
    }

    void ApplyStateSnapshot(string data)
    {
        string[] parts = data.Split('|');
        if (parts.Length < 6)
        {
            Debug.LogWarning($"[TCP] 잘못된 STATE 스냅샷: {data}");
            return;
        }

        if (!int.TryParse(parts[1], out int page))
        {
            page = 0;
        }

        IsTutorialRead = parts[2] == "1";
        ResetRequested = parts[3] == "1";
        EndWait = parts[4] == "1";

        bool contentEnd = parts[5] == "1";
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.IsContentEnd = contentEnd;
        }

        if (PageController.Instance != null)
        {
            PageController.Instance.RequestResetOpenPage(page);
        }

        Debug.Log($"[TCP] 상태 복원 완료: page={page}, tutorial={IsTutorialRead}, reset={ResetRequested}, endWait={EndWait}, contentEnd={contentEnd}");
    }

    void MarkResyncComplete()
    {
        if (!IsConnected)
        {
            return;
        }

        if (_syncState == ConnectionSyncState.Connected)
        {
            return;
        }

        _syncState = ConnectionSyncState.Connected;
        Debug.Log("[TCP] 재동기화 완료");

        while (deferredControlMessages.Count > 0)
        {
            string deferred = deferredControlMessages.Dequeue();
            if (tcpComponent != null)
            {
                tcpComponent.SendData(deferred);
            }
        }
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
