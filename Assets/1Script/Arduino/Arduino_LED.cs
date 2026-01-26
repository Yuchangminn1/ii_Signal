using UnityEngine;
using System.IO.Ports;
using System.Collections;

public class Arduino_LED : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM101", 9600);

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    Coroutine readMessageCoroutine = null;

    const float receivedTime = 2f;

    bool _isReceived = false;

    bool[] isButtonPressed = new bool[12];

    Coroutine[] buttonPressCoroutine = new Coroutine[12];


    WaitForSeconds actionDelay = new WaitForSeconds(0.5f);

    string prevMessage = "";

    private KeyCode[] targetKeys = {
    KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
    KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.A, KeyCode.S
};

    void Start()
    {
        stream.Open();
        stream.ReadTimeout = 500; // 응답 대기 시간

        StartCoroutine(SendMessage());

        for (int i = 0; i < buttonPressCoroutine.Length; i++)
        {
            buttonPressCoroutine[i] = null;
        }


    }

    void Update()
    {

        // 2. 반복문을 통해 어떤 키가 눌렸는지 검사합니다.
        for (int i = 0; i < targetKeys.Length; i++)
        {
            if (Input.GetKey(targetKeys[i]))
            {
                HandleButtonPress(i);
            }
        }


    }


    // 3. 공통 로직을 별도의 함수로 분리합니다.
    void HandleButtonPress(int index)
    {
        if (buttonPressCoroutine[index] != null)
        {
            StopCoroutine(buttonPressCoroutine[index]);
        }
        buttonPressCoroutine[index] = StartCoroutine(PressButton(index));
    }

    IEnumerator PressButton(int index)
    {
        isButtonPressed[index] = true;
        yield return actionDelay;
        isButtonPressed[index] = false;
    }
    IEnumerator SendMessage()
    {
        while (true)
        {

            // 숫자 1키를 누르면 긴 문장을 보냄
            string message = "IN";

            for (int i = 0; i < buttonPressCoroutine.Length; i++)
            {

                if (isButtonPressed[i])
                {
                    message += $"{i},";
                }

            }
            if (prevMessage != "INOUT" || message != "IN")
            {
                message += "OUT";

                Debug.Log("아두이노로 보내는 메시지: " + message);
                prevMessage = message;

                stream.WriteLine(message); // 끝에 \n을 붙여서 전송

                if (readMessageCoroutine != null)
                    StopCoroutine(readMessageCoroutine);

                readMessageCoroutine = StartCoroutine(ReadMessage());

                yield return StartCoroutine(IsReceived(message));
            }

            yield return actionDelay;

        }
    }

    IEnumerator ReadMessage()
    {
        _isReceived = false;

        bool isInput = false;

        while (_isReceived == false)
        {
            string received = stream.ReadLine();
            if (received != null && received.Length > 0)
            {
                Debug.Log("아두이노로부터 받은 메시지: " + received);

                if (isInput == false)
                    isInput = received?.StartsWith("IN") ?? false;

                if (_isReceived == false)
                    _isReceived = received?.EndsWith("OUT") ?? false;
                if (_isReceived)
                    break;

            }
            else
            {
                yield return waitForFixedUpdate;
            }
        }

        readMessageCoroutine = null;

    }

    IEnumerator IsReceived(string message)
    {
        float timer = receivedTime;
        while (_isReceived == false)
        {
            timer -= Time.deltaTime;
            yield return waitForFixedUpdate;


            // if (timer < 0f && _isReceived == false)
            // {
            //     timer = receivedTime;
            //     Debug.Log("재전송: " + message);
            //     if (readMessageCoroutine != null)
            //         StopCoroutine(readMessageCoroutine);
            //     readMessageCoroutine = StartCoroutine(ReadMessage());

            //     stream.WriteLine(message);
            // }
        }

    }

    void OnApplicationQuit()
    {
        if (stream.IsOpen) stream.Close();
    }
}