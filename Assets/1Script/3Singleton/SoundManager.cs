using System.Collections;
using UnityEngine;


public enum EffectSoundNum
{
    // ...existing code...
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
    private AudioSource Bgm;

    private AudioSource SaveSound;

    private AudioSource SoulPieceSound;

    private AudioSource ConfirmSound;

    private AudioSource PopupSound;

    private AudioSource ActiveSound;

    private AudioSource StepTextSound;

    private AudioSource MorseDotSound_1;
    private AudioSource MorseDashSound_1;
    private AudioSource MorseDotSound_2;
    private AudioSource MorseDashSound_2;
    private AudioSource MorseDashLoopSound;
    private AudioSource SignalReceiveSound;
    private AudioSource SignalSendSound;

    float _soundVolume = 1f;

    float _delayTime = 0.5f;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    void Start()
    {


        StartCoroutine(DelayToPlay(Bgm));
    }




    IEnumerator DelayToPlay(AudioSource _tempSource)
    {
        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();

        foreach (AudioSource source in audioSources)
        {
            switch (source.gameObject.name)
            {
                case "BGMSound":
                    Bgm = source;
                    break;
                case "SaveSound":
                    SaveSound = source;
                    break;
                case "SoulPieceSound":
                    SoulPieceSound = source;
                    break;
                case "ConfirmSound":
                    ConfirmSound = source;
                    break;
                case "PopupSound":
                    PopupSound = source;
                    break;
                case "ActiveSound":
                    ActiveSound = source;
                    break;

                case "StepTextSound":
                    StepTextSound = source;
                    break;
                case "MorseDotSound_1":
                    MorseDotSound_1 = source;
                    break;
                case "MorseDashSound_1":
                    MorseDashSound_1 = source;
                    break;
                case "MorseDotSound_2":
                    MorseDotSound_2 = source;
                    break;
                case "MorseDashSound_2":
                    MorseDashSound_2 = source;
                    break;
                case "MorseDashLoopSound":
                    MorseDashLoopSound = source;
                    MorseDashLoopSound.loop = true;
                    break;
                default:
                    Debug.LogWarning("Unrecognized AudioSource: " + source.gameObject.name);
                    break;
                case "SignalReceiveSound":
                    SignalReceiveSound = source;
                    break;
                case "SignalSendSound":
                    SignalSendSound = source;
                    break;
            }
        }

        yield return CoroutineReturnManager.GetWaitForSeconds(1f);
        if (_tempSource == null) yield break;
        _tempSource.Play();
        //MuteBGM();
    }


    public void MuteBGM()
    {
        if (Bgm == null) return;
        Bgm.volume = 0f;
        Bgm.Stop();

    }
    public void PlayBGM()
    {
        Bgm.Play();
        Bgm.volume = 0.8f;
    }

    public void MuteSound()
    {

        MorseDashSound_1.Stop();
        MorseDashSound_2.Stop();

    }

    public void PlayingLoopSound()
    {
        if (MorseDashSound_1 != null)
        {
            if (MorseDashSound_1.isPlaying == false)
            {
                MorseDashSound_1.Play();
            }
        }

    }
    public void StopLoopSound()
    {
        if (MorseDashLoopSound != null)
        {
            if (MorseDashLoopSound.isPlaying)
            {
                MorseDashLoopSound.Stop();
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

        switch (effectSoundNum)
        {
            case EffectSoundNum.SaveSound:
                if (SaveSound == null) return;
                SaveSound.PlayOneShot(SaveSound.clip, soundVolume);
                break;

            case EffectSoundNum.SoulPieceSound:
                if (SoulPieceSound == null) return;
                SoulPieceSound.PlayOneShot(SoulPieceSound.clip, soundVolume);
                break;
            case EffectSoundNum.ConfirmSound:
                if (ConfirmSound == null) return;
                ConfirmSound.PlayOneShot(ConfirmSound.clip, soundVolume);
                break;
            case EffectSoundNum.PopupSound:
                if (PopupSound == null) return;
                PopupSound.PlayOneShot(PopupSound.clip, soundVolume);
                break;
            case EffectSoundNum.ActiveSound:
                if (ActiveSound == null) return;
                ActiveSound.PlayOneShot(ActiveSound.clip, soundVolume);
                break;

            case EffectSoundNum.StepTextSound:
                if (StepTextSound == null) return;
                StepTextSound.PlayOneShot(StepTextSound.clip, soundVolume);
                break;

            case EffectSoundNum.MorseDotSound_1:
                if (MorseDotSound_1 == null) return;
                MuteSound();
                MorseDotSound_1.PlayOneShot(MorseDotSound_1.clip, soundVolume);
                break;

            case EffectSoundNum.MorseDashSound_1:
                if (MorseDashSound_1 == null) return;
                MuteSound();
                MorseDashSound_1.PlayOneShot(MorseDashSound_1.clip, soundVolume);
                break;

            case EffectSoundNum.MorseDotSound_2:
                if (MorseDotSound_2 == null) return;
                MuteSound();
                MorseDotSound_2.PlayOneShot(MorseDotSound_2.clip, soundVolume);
                break;

            case EffectSoundNum.MorseDashSound_2:
                if (MorseDashSound_2 == null) return;
                MuteSound();
                MorseDashSound_2.PlayOneShot(MorseDashSound_2.clip, soundVolume);
                break;

            case EffectSoundNum.SignalReceiveSound:
                if (SignalReceiveSound == null) return;
                SignalSendSound.Stop();
                SignalReceiveSound.PlayOneShot(SignalReceiveSound.clip, soundVolume);
                break;

            case EffectSoundNum.SignalSendSound:
                if (SignalSendSound == null) return;
                MuteSound();
                SignalSendSound.PlayOneShot(SignalSendSound.clip, soundVolume);
                break;
        }
        Debug.Log("Played sound: " + effectSoundNum.ToString() + " with volume: " + soundVolume);

    }
    // public void PlayEffectSound(int soundIndex)
    // {
    //     if (soundIndex < 0 || soundIndex > 7) return;
    //     EffectSoundNum effectSoundNum = (EffectSoundNum)soundIndex;

    //     switch (effectSoundNum)
    //     {


    //         case EffectSoundNum.SaveSound:
    //             if (SaveSound == null) return;
    //             SaveSound.PlayOneShot(SaveSound.clip, _soundVolume);
    //             break;

    //         case EffectSoundNum.SoulPieceSound:
    //             if (SoulPieceSound == null) return;
    //             SoulPieceSound.PlayOneShot(SoulPieceSound.clip, _soundVolume);
    //             break;
    //         case EffectSoundNum.ConfirmSound:
    //             if (ConfirmSound == null) return;
    //             ConfirmSound.PlayOneShot(ConfirmSound.clip, _soundVolume);
    //             break;
    //         case EffectSoundNum.PopupSound:
    //             if (PopupSound == null) return;
    //             PopupSound.PlayOneShot(PopupSound.clip, _soundVolume);
    //             break;
    //         case EffectSoundNum.ActiveSound:
    //             if (ActiveSound == null) return;
    //             ActiveSound.PlayOneShot(ActiveSound.clip, _soundVolume);
    //             break;


    //         case EffectSoundNum.StepTextSound:
    //             if (StepTextSound == null) return;
    //             StepTextSound.PlayOneShot(StepTextSound.clip, _soundVolume);
    //             break;


    //         case EffectSoundNum.MorseDotSound_1:
    //             if (MorseDotSound_1 == null) return;
    //             MorseDotSound_1.PlayOneShot(MorseDotSound_1.clip, _soundVolume);
    //             break;

    //         case EffectSoundNum.MorseDashSound_1:
    //             if (MorseDashSound_1 == null) return;
    //             MorseDashSound_1.PlayOneShot(MorseDashSound_1.clip, _soundVolume);
    //             break;
    //     }
    //     Debug.Log("Played sound: " + effectSoundNum.ToString() + " with volume: " + _soundVolume);

    // }
}
