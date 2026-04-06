using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SimpleTcpServer : MonoBehaviour, ITCP
{
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    private bool _isConnected = false;
    private volatile bool _isRunning = false;

    public bool IsConnected => _isConnected && connectedTcpClient != null && connectedTcpClient.Connected;





    // Start is called before the first frame update
    void Start()
    {
        // MainThreadDispatcher 초기화
        MainThreadDispatcher.EnsureCreated();

        // Start TcpServer background thread
        _isRunning = true;
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ListenForIncommingRequests()
    {
        try
        {
            // Create listener on all network interfaces (0.0.0.0), port 8052.
            // 사용자의 요청에 따라 특정 IP("192.0.0.2")뿐만 아니라 
            // localhost 등 모든 주소로 들어오는 연결을 허용하도록 IPAddress.Any로 변경합니다.
            tcpListener = new TcpListener(IPAddress.Any, 8052);
            tcpListener.Start();
            Debug.Log("Server is listening on all network interfaces");

            while (_isRunning)
            {
                connectedTcpClient = tcpListener.AcceptTcpClient();
                if (!_isRunning)
                {
                    break;
                }

                _isConnected = true;

                // NetworkManager에 연결 성공 알림
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    NetworkManager.Instance.OnConnectionEstablished();
                });

                Debug.Log("Client connected");

                // Get a stream object for reading and writing
                NetworkStream stream = connectedTcpClient.GetStream();

                try
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                    {
                        while (_isRunning)
                        {
                            string clientMessage = reader.ReadLine();
                            if (clientMessage == null)
                            {
                                _isConnected = false;
                                if (_isRunning)
                                {
                                    MainThreadDispatcher.RunOnMainThread(() =>
                                    {
                                        NetworkManager.Instance.SetConnectionLost();
                                    });
                                }
                                break;
                            }
                            string receivedMessage = clientMessage;
                            MainThreadDispatcher.RunOnMainThread(() =>
                            {
                                ReadData(receivedMessage);
                            });
                        }
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
                        Debug.Log("Client disconnected or error: " + e.Message);
                    }
                }

                CloseClientConnection();
            }

        }
        catch (SocketException socketException)
        {
            if (_isRunning)
            {
                Debug.Log("SocketException " + socketException.ToString());
            }
        }
        finally
        {
            CloseClientConnection();
            CloseListener();
            tcpListenerThread = null;
        }
    }

    private void SendMessageToClient(string message)
    {
        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing.
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                byte[] serverMessageAsByteArray = Encoding.UTF8.GetBytes(message + "\n");
                // Write byte array to socketConnection stream.
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server : " + message);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SendData(string data)
    {
        SendMessageToClient(data);
    }

    public void StopServer()
    {
        _isRunning = false;
        _isConnected = false;
        CloseClientConnection();
        CloseListener();
        if (tcpListenerThread != null && tcpListenerThread.IsAlive)
        {
            tcpListenerThread.Join(1000); // Wait up to 1 second
        }
    }

    private void CloseClientConnection()
    {
        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            connectedTcpClient.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Close client exception: " + e.Message);
        }
        finally
        {
            connectedTcpClient = null;
        }
    }

    private void CloseListener()
    {
        if (tcpListener == null)
        {
            return;
        }

        try
        {
            tcpListener.Stop();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Close listener exception: " + e.Message);
        }
        finally
        {
            tcpListener = null;
        }
    }

    private void OnDisable()
    {
        StopServer();
    }

    private void OnDestroy()
    {
        StopServer();
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }

    public void ReadData(string data)
    {
        try
        {
            bool isMorseData = false;

            if (data.Equals("Go", StringComparison.OrdinalIgnoreCase))
            {
                try { NetworkManager.Instance.IsTutorialRead = true; }
                catch (Exception e) { Debug.LogError($"[TCP ReadData] IsTutorialRead 설정 에러: {e.Message}"); }
                return;
            }

            try { GameManager.Instance.GoToIdleCheck(); }
            catch (Exception e) { Debug.LogError($"[TCP ReadData] GameManager.GoToIdleCheck 에러: {e.Message}"); }

            if (NetworkManager.Instance.EndWait && data.Equals("End", StringComparison.OrdinalIgnoreCase))
            {
                try { NetworkManager.Instance.EndNReset(); }
                catch (Exception e) { Debug.LogError($"[TCP ReadData] EndNReset 에러: {e.Message}"); }
                return;
            }
            else if (data.Equals("EReset", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    UserDataManager.Instance.IsContentEnd = true;
                    if (PageController.Instance.IsIdle())
                    {
                        Debug.Log("TCP 리셋 - 이미 Idle 상태");
                    }
                    else
                    {
                        Debug.Log("TCP 리셋");
                    }
                }
                catch (Exception e) { Debug.LogError($"[TCP ReadData] EReset 처리 에러: {e.Message}"); }
                return;
            }
            else if (data.Equals("Reset", StringComparison.OrdinalIgnoreCase))
            {
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
                    isMorseData = true;

                    foreach (char c in data)
                    {
                        if (c != '0' && c != '1')
                        {
                            isMorseData = false;
                            break;
                        }
                    }

                    if (isMorseData)
                    {
                        UserDataManager.Instance.GetPlayer().PartnerAnswerData.Enqueue(data);
                        UserDataManager.Instance.GetPlayer().MorsePartnerTotalData += data;
                    }
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

                        isMorseData = true;

                        foreach (char c in data)
                        {
                            if (c != '0' && c != '1')
                            {
                                isMorseData = false;
                                break;
                            }
                        }
                        if (isMorseData)
                        {
                            UserDataManager.Instance.GetPlayer().PartnerAnswerData.Enqueue(data);
                        }
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
