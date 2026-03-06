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





    // Start is called before the first frame update
    void Start()
    {
        // Start TcpServer background thread
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

            while (true)
            {
                connectedTcpClient = tcpListener.AcceptTcpClient();
                Debug.Log("Client connected");

                // Get a stream object for reading and writing
                NetworkStream stream = connectedTcpClient.GetStream();

                try
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                    {
                        while (true)
                        {
                            string clientMessage = reader.ReadLine();
                            if (clientMessage == null) break;
                            ReadData(clientMessage);
                            Debug.Log("Client : " + clientMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Client disconnected or error: " + e.Message);
                }
            }

        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
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
                Debug.Log("받은 메세지 : " + message);
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

    public void ReadData(string data)
    {
        bool isMorseData = false;

        if (NetworkManager.Instance.EndWait && data == "End")
        {
            NetworkManager.Instance.EndNReset();
            return;

        }
        else if (data == "Go")
        {
            NetworkManager.Instance.IsTutorialRead = true;

            return;
        }

        else if (data == "Reset")
        {
            if (PageController.Instance.IsIdle())
            {
                Debug.Log("TCP 리셋 - 이미 Idle 상태");
            }
            else
            {
                Debug.Log("TCP 리셋");
                NetworkManager.Instance.SendData($"Reset");

                UserDataManager.Instance.ResetUserData();

                NetworkManager.Instance.ResetRequested = true;

            }
            return;

        }
        else if (data.Length == 4)
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
        }
        else if (data.Length == 5)
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
                return;
            }

        }
        else
        {
            Debug.Log("처리 안하는 입력: " + data);
        }

        if (isMorseData)
            UserDataManager.Instance.GetPlayer().PartnerAnswerData.Enqueue(data);

    }

}
