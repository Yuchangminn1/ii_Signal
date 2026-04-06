using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SimpleTcpClient : MonoBehaviour, ITCP
{
    // System.Net.Sockets.TcpClient와 이름 충돌을 피하기 위해 클래스 이름을 변경했습니다.
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private bool _isConnected = false;
    private volatile bool _isRunning = false;

    string serverIp = "192.168.219.103";
    public string ServerIp { get => serverIp; set => serverIp = value; }

    public bool IsConnected => _isConnected && socketConnection != null && socketConnection.Connected;


    // Start is called before the first frame update
    void Start()
    {
        // ConnectToTcpServer();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ConnectToTcpServer()
    {
        if (_isRunning && clientReceiveThread != null && clientReceiveThread.IsAlive)
        {
            return;
        }

        try
        {
            // MainThreadDispatcher 초기화
            MainThreadDispatcher.EnsureCreated();

            _isRunning = true;
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            _isRunning = false;
            Debug.Log("On client connect exception " + e);
        }
    }

    private void ListenForData()
    {
        while (_isRunning) // 연결 재시도 루프
        {
            try
            {
                CloseSocketConnection();

                // 사용자의 ifconfig 정보(en0)에 따라 IP 주소를 192.0.0.2로 설정
                socketConnection = new TcpClient(serverIp, 8052);
                _isConnected = true;

                // NetworkManager에 연결 성공 알림
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    NetworkManager.Instance.OnConnectionEstablished();
                });

                Debug.Log("Connected to Server!");

                using (NetworkStream stream = socketConnection.GetStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    while (_isRunning)
                    {
                        // 데이터를 한 줄씩 읽어서 처리 (패킷 뭉침/잘림 방지)
                        string serverMessage = reader.ReadLine();

                        // 연결이 끊기면 null이 반환됨
                        if (serverMessage == null)
                        {
                            Debug.Log("Server disconnected.");
                            _isConnected = false;
                            MainThreadDispatcher.RunOnMainThread(() =>
                            {
                                NetworkManager.Instance.SetConnectionLost();
                            });
                            break;
                        }

                        string receivedMessage = serverMessage;
                        MainThreadDispatcher.RunOnMainThread(() =>
                        {
                            ReadData(receivedMessage);
                        });

                    }
                }
            }
            catch (SocketException socketException)
            {
                _isConnected = false;
                if (_isRunning)
                {
                    MainThreadDispatcher.RunOnMainThread(() =>
                    {
                        NetworkManager.Instance.SetConnectionLost();
                    });
                    Debug.Log("Socket Exception - Retrying to connect to server... " + socketException.Message);
                }
            }
            catch (Exception e)
            {
                _isConnected = false;
                if (_isRunning)
                {
                    MainThreadDispatcher.RunOnMainThread(() =>
                    {
                        NetworkManager.Instance.SetConnectionLost();
                    });
                    Debug.Log("Exception: " + e.Message);
                }
            }

            // 연결이 끊어지거나 실패하면 잠시 대기 후 재시도
            if (_isRunning)
            {
                Thread.Sleep(1000);
            }
        }

        CloseSocketConnection();
        clientReceiveThread = null;
    }

    private void SendMessageToTcpServer(string clientMessage)
    {
        if (socketConnection == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing.
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                // 메시지 끝에 줄바꿈(\n)을 추가하여 전송 (데이터 경계 처리)
                // ASCII 대신 UTF8을 사용하여 한글 깨짐 방지
                byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage + "\n");

                // Write byte array to socketConnection stream.
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SendData(string data)
    {
        SendMessageToTcpServer(data);
    }

    public void StopClient()
    {
        _isRunning = false;
        _isConnected = false;
        CloseSocketConnection();

        if (clientReceiveThread != null && clientReceiveThread.IsAlive)
        {
            clientReceiveThread.Join(1000); // Wait up to 1 second
        }
    }

    private void CloseSocketConnection()
    {
        if (socketConnection == null)
        {
            return;
        }

        try
        {
            socketConnection.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Close socket exception: " + e.Message);
        }
        finally
        {
            socketConnection = null;
        }
    }

    private void OnDisable()
    {
        StopClient();
    }

    private void OnDestroy()
    {
        StopClient();
    }

    private void OnApplicationQuit()
    {
        StopClient();
    }

    public void ReadData(string data)
    {
        try
        {
            if (NetworkManager.Instance.TryHandleSyncMessage(data))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(data))
            {
                Debug.LogWarning("[TCP ReadData] 빈 데이터 수신");
                return;
            }

            if (data.Equals("Go", StringComparison.OrdinalIgnoreCase))
            {
                NetworkManager.Instance.IsTutorialRead = true;
                return;
            }

            try { GameManager.Instance.GoToIdleCheck(); }
            catch (Exception e) { Debug.LogError($"[TCP ReadData] GameManager.GoToIdleCheck 에러: {e.Message}"); }

            if (data.Equals("EReset", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Received data: " + data);

                try
                {
                    if (UserDataManager.Instance.IsContentEnd)
                    {
                        NetworkManager.Instance.ResetRequested = true;
                        NetworkManager.Instance.SendData("EReset");
                    }
                }
                catch (Exception e) { Debug.LogError($"[TCP ReadData] EReset 처리 에러: {e.Message}"); }
                return;
            }
            else if (data.Equals("Reset", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Received data: " + data);

                try { NetworkManager.Instance.ResetRequested = true; }
                catch (Exception e) { Debug.LogError($"[TCP ReadData] Reset 처리 에러: {e.Message}"); }
                return;
            }
            else if (data.Length == 2 && data[0] == 'S')
            {
                try
                {
                    Debug.Log("Stamp Count: " + data);

                    if (!int.TryParse(data.Substring(1, 1), out int addPiece))
                    {
                        Debug.LogWarning($"[TCP ReadData] Stamp 데이터 파싱 실패: {data}");
                        return;
                    }

                    UserDataManager userDataManager = UserDataManager.Instance;
                    if (userDataManager == null)
                    {
                        Debug.LogWarning("[TCP ReadData] UserDataManager가 없어 Stamp 반영을 건너뜁니다.");
                        return;
                    }

                    Player leftPlayer = userDataManager.GetPlayer(Direction.Left);
                    Player rightPlayer = userDataManager.GetPlayer(Direction.Right);
                    if (leftPlayer == null || rightPlayer == null)
                    {
                        Debug.LogWarning("[TCP ReadData] Player가 초기화되지 않아 Stamp 반영을 건너뜁니다.");
                        return;
                    }

                    leftPlayer.AddPiece = addPiece;
                    rightPlayer.AddPiece = addPiece;
                    Debug.Log("AddPiece Set: " + addPiece);
                }
                catch (Exception e) { Debug.LogError($"[TCP ReadData] Stamp 처리 에러: {e.Message}"); }
                return;
            }
            else if (data.Length == 4)
            {
                try
                {
                    Debug.Log("data.Length == 4 Received Data: " + data);
                    UserDataManager.Instance.GetPlayer().PartnerAnswerData.Enqueue(data);
                }
                catch (Exception e) { Debug.LogError($"[TCP ReadData] 4자리 데이터 처리 에러: {e.Message}"); }
            }
            else if (data.Length == 5)
            {
                try
                {
                    Debug.Log("data.Length == 5 Received Data: " + data);

                    if (data.StartsWith("P"))
                    {
                        data = data.Substring(1, 4);
                        Debug.Log("PassCode Data Received: " + data);
                        UserDataManager.Instance.GetPlayer().PartnerPassCode = data;
                        Debug.Log("PassCode Set: " + UserDataManager.Instance.GetPlayer().PartnerPassCode);
                    }
                }
                catch (Exception e) { Debug.LogError($"[TCP ReadData] 5자리 데이터 처리 에러: {e.Message}"); }
                return;
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"[TCP ReadData] 예상치 못한 에러: {e.Message}\n{e.StackTrace}");
        }
    }

}
