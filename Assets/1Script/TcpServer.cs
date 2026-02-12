using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TcpServer : MonoBehaviour
{
    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;
    private Thread thread;

    public int port = 8000;

    void Start()
    {
        thread = new Thread(ServerLoop);
        thread.IsBackground = true;
        thread.Start();
    }

    void ServerLoop()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Debug.Log("Server Started");

        client = listener.AcceptTcpClient(); // 여기서 연결 대기
        Debug.Log("Client Connected");

        stream = client.GetStream();

        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if (bytes <= 0) continue;

            string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
            Debug.Log("Client: " + msg);

            // 에코 응답
            byte[] send = Encoding.UTF8.GetBytes("Echo: " + msg);
            stream.Write(send, 0, send.Length);
        }
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
        listener?.Stop();
        thread?.Abort();
    }
}
