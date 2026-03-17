using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SoundUpload : MonoBehaviour
{
    [Header("Input Clips (0 = short, 1 = long)")]
    public AudioClip clip0;
    public AudioClip clip1;

    [Header("Pattern")]
    public string pattern = "";
    public bool playOnStart = true;
    public bool saveWavWhenPlayed = false;
    public string outputFileName = "morse_output.wav";

    [Header("Server Upload (PhotoCompositor style)")]
    bool uploadCombinedAudioWhenPlayed = true;
    [Min(1)] public int uploadCount = 1;
    private int maxRetries = 10;
    private float retryDelay = 1.0f;
    private string uploadUrl = "http://192.168.0.252:8500/api/uploadFile.cfm";
    private string uploadType = "wav";
    private string uploadContentType = "audio/wav";
    private bool logUploadDebug = true;

    [Header("Playback")]
    public AudioSource audioSource;
    bool silentMode = true;

    private AudioClip _lastCombinedClip;
    private Coroutine _uploadCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (NetworkManager.Instance.IsServer == false)
                return;
            LeftPlayerUpload();
            RightPlayerUpload();
        }
    }

    public void LeftPlayerUpload()
    {
        if (NetworkManager.Instance.IsServer == false)
            return;
        if (UserDataManager.Instance.GetPlayer().MorseTotalData.Length < 1)
        {
            Debug.LogWarning("Left player Morse data is empty. Upload skipped.");
            return;
        }
        Debug.Log($"Upload Morse Wav Left Player Morse Data  : {UserDataManager.Instance.GetPlayer().MorseTotalData}");
        Startss(UserDataManager.Instance.GetPlayer().MorseTotalData);
    }
    public void RightPlayerUpload()
    {
        if (NetworkManager.Instance.IsServer == false)
            return;
        if (UserDataManager.Instance.GetPlayer().MorsePartnerTotalData.Length < 1)
        {
            Debug.LogWarning("Right player Morse data is empty. Upload skipped.");
            return;
        }
        Debug.Log($"Upload Morse Wav Right Player Morse Data  : {UserDataManager.Instance.GetPlayer().MorsePartnerTotalData}");
        Startss(UserDataManager.Instance.GetPlayer().MorsePartnerTotalData);
    }

    public void Startss(string patterns)
    {
        pattern = patterns;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (playOnStart)
        {
            Debug.Log($"[SoundTest] Start -> PlayPattern called. pattern={pattern}, uploadEnabled={uploadCombinedAudioWhenPlayed}");
            PlayPattern(pattern);
        }
        else
        {
            Debug.Log("[SoundTest] Start skipped playback because playOnStart is false.");
        }
    }

    [ContextMenu("Play Current Pattern")]
    public void PlayCurrentPattern()
    {
        PlayPattern(pattern);
    }

    public void PlayPattern(string binaryPattern)
    {
        if (!TryBuildCombinedClip(binaryPattern, out AudioClip combinedClip))
        {
            return;
        }

        _lastCombinedClip = combinedClip;
        if (!silentMode)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = combinedClip;
            audioSource.Play();
        }

        if (saveWavWhenPlayed)
        {
            string outputPath = Path.Combine(Application.persistentDataPath, outputFileName);
            SaveAudioClipAsWav(combinedClip, outputPath);
            Debug.Log($"WAV saved: {outputPath}");
        }

        if (uploadCombinedAudioWhenPlayed)
        {
            if (_uploadCoroutine != null)
            {
                StopCoroutine(_uploadCoroutine);
            }

            Debug.Log("[SoundTest] Starting upload coroutine for combined audio.");
            _uploadCoroutine = StartCoroutine(UploadCombinedAudioRoutine(combinedClip));
        }
        else
        {
            Debug.LogWarning("[SoundTest] Upload skipped because uploadCombinedAudioWhenPlayed is false. In Unity, the Inspector/prefab serialized value overrides the code default, so this object is currently saved with uploadCombinedAudioWhenPlayed unchecked.");
        }
    }

    [ContextMenu("Save Last Combined Clip As WAV")]
    public void SaveLastCombinedClipAsWav()
    {
        if (_lastCombinedClip == null)
        {
            Debug.LogWarning("No combined clip exists yet. Play a pattern first.");
            return;
        }

        string outputPath = Path.Combine(Application.persistentDataPath, outputFileName);
        SaveAudioClipAsWav(_lastCombinedClip, outputPath);
        Debug.Log($"WAV saved: {outputPath}");
    }

    private bool TryBuildCombinedClip(string binaryPattern, out AudioClip combinedClip)
    {
        combinedClip = null;

        if (string.IsNullOrEmpty(binaryPattern))
        {
            Debug.LogWarning("Pattern is empty.");
            return false;
        }

        if (clip0 == null || clip1 == null)
        {
            Debug.LogWarning("Assign both clip0 and clip1.");
            return false;
        }

        if (clip0.channels != clip1.channels || clip0.frequency != clip1.frequency)
        {
            Debug.LogError("clip0 and clip1 must have the same channel count and sample rate.");
            return false;
        }

        int channels = clip0.channels;
        int frequency = clip0.frequency;
        int clip0DataLength = clip0.samples * channels;
        int clip1DataLength = clip1.samples * channels;

        if (clip0DataLength <= 0 || clip1DataLength <= 0)
        {
            Debug.LogError("clip0 or clip1 has no audio data.");
            return false;
        }

        float[] clip0Data = new float[clip0.samples * channels];
        float[] clip1Data = new float[clip1.samples * channels];
        clip0.GetData(clip0Data, 0);
        clip1.GetData(clip1Data, 0);

        int totalDataLength = 0;

        for (int i = 0; i < binaryPattern.Length; i++)
        {
            char symbol = binaryPattern[i];
            if (symbol == '0')
            {
                totalDataLength += clip0DataLength;
            }
            else if (symbol == '1')
            {
                totalDataLength += clip1DataLength;
            }
            else
            {
                Debug.LogError($"Invalid symbol '{symbol}' at index {i}. Only 0 and 1 are allowed.");
                return false;
            }
        }

        float[] combinedData = new float[totalDataLength];
        int writeIndex = 0;

        for (int i = 0; i < binaryPattern.Length; i++)
        {
            int sourceLength = binaryPattern[i] == '0' ? clip0DataLength : clip1DataLength;
            float[] sourceData = binaryPattern[i] == '0' ? clip0Data : clip1Data;
            Array.Copy(sourceData, 0, combinedData, writeIndex, sourceLength);
            writeIndex += sourceLength;
        }

        int combinedSamplesPerChannel = totalDataLength / channels;
        combinedClip = AudioClip.Create($"morse_{binaryPattern}", combinedSamplesPerChannel, channels, frequency, false);
        combinedClip.SetData(combinedData, 0);
        return true;
    }

    private void SaveAudioClipAsWav(AudioClip clip, string path)
    {
        byte[] wavBytes = BuildWavBytes(clip);
        if (wavBytes == null || wavBytes.Length == 0)
        {
            Debug.LogError("Failed to build WAV bytes.");
            return;
        }

        string directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllBytes(path, wavBytes);
    }

    private byte[] BuildWavBytes(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * sizeof(short)];

        const float rescaleFactor = 32767f;
        for (int i = 0; i < samples.Length; i++)
        {
            float clamped = Mathf.Clamp(samples[i], -1f, 1f);
            intData[i] = (short)(clamped * rescaleFactor);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            bytesData[i * 2] = byteArr[0];
            bytesData[i * 2 + 1] = byteArr[1];
        }

        int sampleRate = clip.frequency;
        short channels = (short)clip.channels;
        short bitsPerSample = 16;
        int byteRate = sampleRate * channels * bitsPerSample / 8;
        short blockAlign = (short)(channels * bitsPerSample / 8);
        int subChunk2Size = bytesData.Length;
        int chunkSize = 36 + subChunk2Size;

        using (MemoryStream stream = new MemoryStream(44 + bytesData.Length))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write(chunkSize);
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write(channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write(blockAlign);
            writer.Write(bitsPerSample);
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write(subChunk2Size);
            writer.Write(bytesData);
            writer.Flush();
            return stream.ToArray();
        }
    }

    private IEnumerator UploadCombinedAudioRoutine(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[SoundTest] Upload skipped: clip is null.");
            _uploadCoroutine = null;
            yield break;
        }

        if (UserDataManager.Instance == null || !UserDataManager.Instance.IsUser())
        {
            Debug.LogWarning("[SoundTest] Upload skipped: user data is not ready. UserDataManager.Instance is null or IsUser() returned false.");
            _uploadCoroutine = null;
            yield break;
        }

        byte[] audioBytes = BuildWavBytes(clip);
        if (audioBytes == null || audioBytes.Length == 0)
        {
            Debug.LogError("[SoundTest] Upload failed: encoded audio is empty.");
            _uploadCoroutine = null;
            yield break;
        }

        int safeUploadCount = Mathf.Max(1, uploadCount);

        string idxUser = UserDataManager.Instance.FindValue("IDX_USER");
        string uid = UserDataManager.Instance.FindValue("UID_LEFT");
        string code = ServerData.Instance.Code;
        string requestUrl = $"{uploadUrl}?idx_user={idxUser}&uid={uid}&code={code}&type={uploadType}&count={safeUploadCount}";

        if (logUploadDebug)
        {
            Debug.Log($"[SoundTest] Upload request prepared. url={requestUrl}, bytes={audioBytes.Length}, contentType={uploadContentType}, type={uploadType}, count={safeUploadCount}");
        }

        bool uploadSuccess = false;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            Task requestTask;

            using (UnityWebRequest webRequest = new UnityWebRequest(requestUrl, UnityWebRequest.kHttpVerbPOST))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(audioBytes);
                webRequest.uploadHandler.contentType = uploadContentType;
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.timeout = 15;

                if (logUploadDebug)
                {
                    Debug.Log($"[SoundTest] Sending upload attempt {attempt + 1}/{maxRetries} to {requestUrl}");
                }

                requestTask = SendWebRequestAsync(webRequest);
                yield return new WaitUntil(() => requestTask.IsCompleted);

                string responseText = webRequest.downloadHandler != null ? webRequest.downloadHandler.text : string.Empty;
                if (logUploadDebug)
                {
                    Debug.Log($"[SoundTest] Upload response: result={webRequest.result}, status={webRequest.responseCode}, error={webRequest.error}, body={responseText}");
                }

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[SoundTest] Audio upload success: {webRequest.responseCode}, body={responseText}");
                    uploadSuccess = true;
                    break;
                }

                if (attempt < maxRetries - 1)
                {
                    Debug.LogWarning($"[SoundTest] Audio upload failed ({attempt + 1}/{maxRetries}): {webRequest.error}. Retrying in {retryDelay} sec...");
                    yield return new WaitForSeconds(retryDelay);
                }
                else
                {
                    Debug.LogError($"[SoundTest] Audio upload final failure: {webRequest.error}");
                }
            }
        }

        if (uploadSuccess)
        {
            uploadCount = safeUploadCount + 1;
        }

        _uploadCoroutine = null;
    }

    private Task SendWebRequestAsync(UnityWebRequest request)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        request.SendWebRequest().completed += _ => tcs.SetResult(true);
        return tcs.Task;
    }
}
