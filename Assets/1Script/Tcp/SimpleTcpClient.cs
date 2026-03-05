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

    string serverIp = "192.168.219.103";
    public string ServerIp { get => serverIp; set => serverIp = value; }


    // Start is called before the first frame update
    void Start()
    {
        // ConnectToTcpServer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToTcpServer("준비완료");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SendMessageToTcpServer("0번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SendMessageToTcpServer("1번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SendMessageToTcpServer("2번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SendMessageToTcpServer("3번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SendMessageToTcpServer("4번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SendMessageToTcpServer("5번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SendMessageToTcpServer("6번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SendMessageToTcpServer("7번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SendMessageToTcpServer("8번");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SendMessageToTcpServer("9번");
        }
    }

    public void Space()
    {
        SendMessageToTcpServer("준비완료");
    }
    public void input1()
    {
        SendMessageToTcpServer("1번");
    }
    public void input2()
    {
        SendMessageToTcpServer("2번");
    }
    public void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    private void ListenForData()
    {
        while (true) // 연결 재시도 루프
        {
            try
            {
                // 사용자의 ifconfig 정보(en0)에 따라 IP 주소를 192.0.0.2로 설정
                socketConnection = new TcpClient(serverIp, 8052);
                Debug.Log("Connected to Server!");

                using (NetworkStream stream = socketConnection.GetStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    while (true)
                    {
                        // 데이터를 한 줄씩 읽어서 처리 (패킷 뭉침/잘림 방지)
                        string serverMessage = reader.ReadLine();

                        // 연결이 끊기면 null이 반환됨
                        if (serverMessage == null)
                        {
                            Debug.Log("Server disconnected.");
                            break;
                        }

                        ReadData(serverMessage);

                        Debug.Log("Server : " + serverMessage);
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Retrying to connect to server... " + socketException.Message);
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e.Message);
            }

            // 연결이 끊어지거나 실패하면 잠시 대기 후 재시도
            Thread.Sleep(1000);
        }
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
                Debug.Log("Client sent: " + clientMessage);
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

    public void ReadData(string data)
    {
        bool isMorseData = false;

        if (data.Length == 4)
        {
            Debug.Log("data.Length == 4 Received Data: " + data);
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
        else if (data == "Reset")
        {
            Debug.Log("data.Length == Reset Received Data: " + data);

            if (PageController.Instance.IsIdle())
            {
                Debug.Log("TCP 리셋 - 이미 Idle 상태");
                return;
            }
            else
            {
                Debug.Log("TCP 리셋");
                PageController.Instance.RequestResetOpenPage(0);
            }
            return;

        }

        if (isMorseData)
        {
            if (PageController.Instance.CurrentPage == 4)
                UserDataManager.Instance.GetPlayer().PartnerAnswerData.Enqueue(data);

            else
            {
                Debug.Log("4번 페이지가 아닌데 모스 데이터 수신: " + data);
            }

        }


        else
        {
            Debug.Log("처리 안하는 입력: " + data);
        }
    }

}
