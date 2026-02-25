using System.Collections;
using UnityEngine;


public enum EffectSoundNum
{
    // ...existing code...
    BGM,
    SaveSound,      // 답변 저장 소리
    SoulPieceSound, // 마음 조각 뜨는 소리
    ConfirmSound,   // 사용자 확인 완료음
    PopupSound,     // 팝업 뜨는 소리
    ActiveSound,     // 활성화음
    StepTextSound,
    MorseDotSound_1,
    MorseDashSound_1,
    MorseDotSound_2,
    MorseDashSound_2,
    MorseDashLoopSound,
    SignalReceiveSound, // 마음신호 도착음
    SignalSendSound     // 전송음


}
public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SoundManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("SoundManager");
                    instance = singletonObject.AddComponent<SoundManager>();
                }
            }

            return instance;
        }
    }
    static SoundManager instance;

    AudioSource[] audioSources;

    float _soundVolume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    void Start()
    {
        audioSources = GetComponentsInChildren<AudioSource>();

        PlayBGM();

    }


    public void MuteBGM()
    {
        AudioSource tempAudioSource = audioSources[(int)(EffectSoundNum.BGM)];

        if (tempAudioSource == null) return;
        tempAudioSource.volume = 0f;
        tempAudioSource.Stop();

    }
    public void PlayBGM()
    {
        AudioSource tempAudioSource = audioSources[(int)(EffectSoundNum.BGM)];
        if (tempAudioSource == null) return;
        tempAudioSource.Play();
        tempAudioSource.volume = 0.8f;
    }

    public void PlayingLoopSound()
    {
        if (audioSources != null && audioSources.Length > 0)
        {
            AudioSource tempAudioSource = audioSources[(int)(EffectSoundNum.MorseDashLoopSound)];
            if (tempAudioSource != null)
            {
                if (tempAudioSource.isPlaying == false)
                {
                    tempAudioSource.Play();
                }
            }
        }


    }
    public void StopLoopSound()
    {
        if (audioSources != null && audioSources.Length > 0)
        {
            AudioSource tempAudioSource = audioSources[(int)(EffectSoundNum.MorseDashLoopSound)];
            if (tempAudioSource != null)
            {
                if (tempAudioSource.isPlaying)
                {
                    tempAudioSource.Stop();
                }
            }
        }

    }

    public void PlayEffectSound(EffectSoundNum effectSoundNum, float soundVolume = 1f)
    {
        if (GameManager.Instance.IsStarted == false)
        {
            Debug.Log("Game Not Started Yet");
            return;
        }

        if (soundVolume == 1) soundVolume = _soundVolume;

        AudioSource tempAudioSource = audioSources[(int)effectSoundNum];
        if (tempAudioSource != null)
        {
            tempAudioSource.volume = soundVolume;
            tempAudioSource.Play();
        }
        // Debug.Log("Played sound: " + effectSoundNum.ToString() + " with volume: " + soundVolume);

    }
    public void StopEffectSound(EffectSoundNum effectSoundNum, float soundVolume = 1f)
    {
        if (GameManager.Instance.IsStarted == false)
        {
            Debug.Log("Game Not Started Yet");
            return;
        }

        if (soundVolume == 1) soundVolume = _soundVolume;

        AudioSource tempAudioSource = audioSources[(int)effectSoundNum];
        if (tempAudioSource != null)
        {
            tempAudioSource.volume = soundVolume;
            tempAudioSource.Stop();
        }
        //  Debug.Log("Stopped sound: " + effectSoundNum.ToString() + " with volume: " + soundVolume);

    }
}
