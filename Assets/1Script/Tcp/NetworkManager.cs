using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITCP
{
    void SendData(string data);

}


public class NetworkManager : Singleton<NetworkManager>, IJsonGenericTarget, ITCP
{

    [Header("Select Network Role")]
    JsonGenericUpData _genericData = new JsonGenericUpData();

    ITCP tcpComponent;

    bool _isServer = false;
    string ipAddress = "";




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

    public void SendData(string data)
    {
        if (tcpComponent != null)
        {
            tcpComponent.SendData(data);
        }
        else
        {
            Debug.LogError("TCP 컴포넌트가 없습니다.");
        }
    }


}
