using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UnifiedTcp : MonoBehaviour
{
    [Header("Mode")]
    public bool isServer = true;

    [Header("Network")]
    public string ip = "127.0.0.1";
    public int port = 7777;

    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    private bool running = false;

    void Start()
    {
        if (isServer)
            StartServer();
        else
            StartClient();
    }

    #region Server
    void StartServer()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        listener.BeginAcceptTcpClient(OnClientConnected, null);

        Debug.Log("Server started, waiting...");
    }

    void OnClientConnected(IAsyncResult ar)
    {
        client = listener.EndAcceptTcpClient(ar);
        stream = client.GetStream();

        running = true;
        StartReceiveThread();

        Debug.Log("Client connected!");
    }
    #endregion

    #region Client
    void StartClient()
    {
        try
        {
            client = new TcpClient();
            client.Connect(ip, port);

            stream = client.GetStream();
            running = true;

            StartReceiveThread();

            Debug.Log("Connected to server!");
        }
        catch (Exception e)
        {
            Debug.LogError("Connection failed: " + e.Message);
        }
    }
    #endregion

    void StartReceiveThread()
    {
        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveLoop()
    {
        byte[] buffer = new byte[1024];

        while (running)
        {
            try
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes <= 0) continue;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
                Debug.Log((isServer ? "Client: " : "Server: ") + msg);
            }
            catch
            {
                running = false;
            }
        }
    }

    public void Send(string message)
    {
        if (stream == null || !stream.CanWrite) return;

        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Send(isServer ? "Hello from Server" : "Hello from Client");
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        stream?.Close();
        client?.Close();
        listener?.Stop();
    }
}
