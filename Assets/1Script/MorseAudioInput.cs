using UnityEngine;

public class MorseAudioInput : MonoBehaviour
{
    public float spikeThreshold = 0.02f;
    public float stableThreshold = 0.005f;

    private AudioClip micClip;
    private string micName;
    private float[] samples = new float[128];

    private float previousVolume = 0f;

    private bool isPressed = false;
    private float pressStartTime = 0f;

    void Start()
    {
        micName = Microphone.devices[0];
        micClip = Microphone.Start(micName, true, 1, 44100);
    }

    void Update()
    {
        int micPosition = Microphone.GetPosition(micName) - samples.Length;
        if (micPosition < 0) return;

        micClip.GetData(samples, micPosition);

        float volume = 0f;
        for (int i = 0; i < samples.Length; i++)
            volume += Mathf.Abs(samples[i]);

        volume /= samples.Length;

        float delta = Mathf.Abs(volume - previousVolume);

        // 1️⃣ 눌림 시작
        if (!isPressed && delta > spikeThreshold)
        {
            isPressed = true;
            pressStartTime = Time.time;

            Debug.Log("PRESS START");
        }

        // 2️⃣ 눌림 종료 (안정 구간 진입)
        if (isPressed && delta < stableThreshold)
        {
            float pressDuration = Time.time - pressStartTime;

            Debug.Log($"PRESS END / Duration: {pressDuration:F3} sec");

            isPressed = false;
        }

        previousVolume = volume;
    }
}