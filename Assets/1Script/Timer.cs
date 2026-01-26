using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    public float defultTime = 5f;
    float time = 0f;
    bool isRunning = false;

    Text _timerText;

    void Awake()
    {
        _timerText = GetComponent<Text>();
    }



    public void ResetTimer()
    {
        time = defultTime;
        _timerText.text = $"{time}";
        isRunning = false;
    }

    public void Trigger()
    {
        ;
    }

    public void StartTimer()
    {
        time = defultTime;
        isRunning = true;
    }


    public void FixedUpdate()
    {
        if (isRunning)
        {
            time -= Time.fixedDeltaTime;
            _timerText.text = time.ToString("F0");
            if (time < 0f)
            {
                time = 0f;
                _timerText.text = "0";
                isRunning = false;
                Trigger();
            }
        }
    }

}
