using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TcpClientMono : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread thread;

    public string serverIP = "192.168.0.25";
    public int port = 8000;

    void Start()
    {
        thread = new Thread(ClientLoop);
        thread.IsBackground = true;
        thread.Start();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Send("Hello Server");
        }
    }

    void ClientLoop()
    {
        client = new TcpClient(AddressFamily.InterNetwork); // IPv4 강제
        client.Connect(serverIP, port);

        Debug.Log("Connected to Server");

        stream = client.GetStream();

        // 먼저 메시지 보내기
        Send("Hello Server");

        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if (bytes <= 0) continue;

            string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
            Debug.Log("Server: " + msg);
        }
    }

    void Send(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
        thread?.Abort();
    }
}
