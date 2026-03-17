using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace My.Scripts.Utils
{
    [Serializable]
    public class CompositeSlot
    {
        public string fileSuffix;

        [Header("Position (Top-Left Pivot)")]
        [Tooltip("배경의 좌상단(0,0)을 기준으로, 사진의 좌상단이 위치할 좌표입니다.")]
        public Vector2 position;

        [Header("Scale")]
        public Vector2 scale = Vector2.one;
    }

    /// <summary> 
    /// 저장된 개별 플레이어 사진들을 지정된 프레임(틀) 이미지 위에 합성한 뒤,
    /// 로컬 디스크에 PNG로 저장하고 서버로 업로드하는 시퀀스를 관리합니다.
    /// </summary>
    public class PhotoCompositor : MonoBehaviour
    {
        [Header("Assets")]
        public Texture2D baseFrame;

        [Tooltip("배경 이미지(출력 캔버스)의 스케일입니다. 화질을 높이려면 값을 키워 해상도를 증가시킬 수 있습니다.")]
        public Vector2 baseFrameScale = Vector2.one;

        [Header("Config")]
        public string saveFolderName = "Pictures";
        public string outputFileName = "Composite";

        [Tooltip("서버 업로드 시 구분용 카운트 번호")]
        [Min(1)]
        public int uploadCount = 1;

        [Header("Canvas Capture")]
        [Tooltip("메인 화면과 다르게 별도로 저장/업로드할 캔버스 목록입니다.")]
        public List<Canvas> uploadCanvases = new List<Canvas>();

        [Tooltip("지정한 캔버스를 렌더링할 PNG 해상도입니다.")]
        public Vector2Int canvasCaptureResolution = new Vector2Int(2250, 4000);

        [Header("API Retry Settings")]
        [SerializeField] private int maxRetries = 10;
        [SerializeField] private float retryDelay = 1.0f;

        [Header("API Endpoints")]
        [SerializeField] private string uploadUrl = "http://192.168.0.252:8500/api/uploadFile.cfm";
        [SerializeField] private string uploadUrl2 = "http://192.168.0.252:8500/api/uploadFile.cfm";

        [Header("Layout")]
        public List<CompositeSlot> slots;

        [Header("Debug")]
        public string debugBaseName = "PlayerAPlayerB";

        public bool IsProcessing { get; private set; }

        [ContextMenu("Execute Composite Now")]
        public void DebugProcessAndSave()
        {
            // 컨텍스트 메뉴로 실행 시 isDebug를 true로 전달하여 로컬 PNG 저장만 수행
            ProcessAndSave(debugBaseName, true);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && !IsProcessing)
            {
                ProcessAndSave(debugBaseName, false);
            }
        }

        public void UpdatePlayerImage()
        {
            ProcessAndSave(debugBaseName, false);
        }

        /// <summary> 
        /// 합성 로직을 실행합니다. 무거운 인코딩과 업로드는 비동기로 처리하여 프리징을 방지합니다.
        /// </summary>
        public void ProcessAndSave(string baseName, bool isDebug = false)
        {
            IsProcessing = true;

            string safeBaseName = string.IsNullOrEmpty(baseName) ? "" : baseName;
            string clean = safeBaseName.Replace("\n", "").Replace("\r", "").Trim();
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            string sanitizedName = Regex.Replace(clean, invalidRegStr, "");

            if (string.IsNullOrWhiteSpace(sanitizedName))
            {
                sanitizedName = "UnknownPlayers";
            }

            StartCoroutine(CaptureCanvasAndUploadRoutine(sanitizedName, isDebug));
        }

        /// <summary> 
        /// 지정된 캔버스를 캡처하거나, 별도 지정이 없으면 현재 화면을 캡처해 PNG로 저장/업로드합니다.
        /// </summary>
        private IEnumerator CaptureCanvasAndUploadRoutine(string sanitizedName, bool isDebug)
        {
            // UI(Canvas)까지 모두 반영된 최종 프레임을 캡처하기 위해 프레임 종료를 대기
            yield return new WaitForEndOfFrame();

            string rootPath = GetRootPath();
            int nextUploadCount = Mathf.Max(1, uploadCount);
            bool hasCanvasTargets = uploadCanvases != null && uploadCanvases.Exists(canvas => canvas != null);

            try
            {
                if (hasCanvasTargets)
                {
                    int savedCanvasCount = 0;

                    foreach (Canvas targetCanvas in uploadCanvases)
                    {
                        if (targetCanvas == null)
                        {
                            continue;
                        }

                        Texture2D canvasTexture = null;

                        try
                        {
                            canvasTexture = CaptureCanvasToTexture(targetCanvas);
                            if (!canvasTexture)
                            {
                                Debug.LogWarning($"[PhotoCompositor] 캔버스 캡처 실패: {targetCanvas.name}");
                                continue;
                            }

                            Task<byte[]> encodeTask = EncodeTextureToPngAsync(canvasTexture);
                            yield return new WaitUntil(() => encodeTask.IsCompleted);

                            if (encodeTask.IsFaulted)
                            {
                                Debug.LogError($"[PhotoCompositor] PNG 인코딩 예외: {encodeTask.Exception?.GetBaseException().Message}");
                                continue;
                            }

                            byte[] pngBytes = encodeTask.Result;
                            if (pngBytes == null || pngBytes.Length == 0)
                            {
                                Debug.LogError($"[PhotoCompositor] PNG 인코딩 실패: {targetCanvas.name}");
                                continue;
                            }

                            savedCanvasCount++;
                            string suffix = $"{outputFileName}_{savedCanvasCount}";
                            string finalFileName = $"{sanitizedName}_{suffix}.png";

                            Task writeTask = File.WriteAllBytesAsync(Path.Combine(rootPath, finalFileName), pngBytes);
                            yield return new WaitUntil(() => writeTask.IsCompleted);

                            if (writeTask.IsFaulted)
                            {
                                Debug.LogError($"[PhotoCompositor] 파일 저장 예외: {writeTask.Exception?.GetBaseException().Message}");
                                continue;
                            }

                            if (!isDebug)
                            {
                                bool useSecondaryUrl = savedCanvasCount > 1;
                                Task uploadTask = UploadImageAsync(pngBytes, finalFileName, nextUploadCount, useSecondaryUrl);
                                yield return new WaitUntil(() => uploadTask.IsCompleted);

                                if (uploadTask.IsFaulted)
                                {
                                    Debug.LogError($"[PhotoCompositor] 업로드 예외: {uploadTask.Exception?.GetBaseException().Message}");
                                }
                            }

                            nextUploadCount++;
                        }
                        finally
                        {
                            if (canvasTexture) Destroy(canvasTexture);
                        }
                    }

                    if (savedCanvasCount == 0)
                    {
                        Debug.LogError("[PhotoCompositor] 저장/업로드할 캔버스 이미지가 생성되지 않았습니다.");
                        yield break;
                    }

                    if (isDebug)
                    {
                        Debug.Log($"<color=cyan>[PhotoCompositor] 디버그 모드 완료: 캔버스 이미지 {savedCanvasCount}개 생성됨. 서버 업로드는 생략되었습니다.</color>");
                    }
                }
                else
                {
                    Texture2D screenshotTex = ScreenCapture.CaptureScreenshotAsTexture();
                    if (!screenshotTex)
                    {
                        Debug.LogError("[PhotoCompositor] 화면 캡처 실패");
                        yield break;
                    }

                    try
                    {
                        Task<byte[]> encodeTask = EncodeTextureToPngAsync(screenshotTex);
                        yield return new WaitUntil(() => encodeTask.IsCompleted);

                        if (encodeTask.IsFaulted)
                        {
                            Debug.LogError($"[PhotoCompositor] PNG 인코딩 예외: {encodeTask.Exception?.GetBaseException().Message}");
                            yield break;
                        }

                        byte[] pngBytes = encodeTask.Result;

                        if (pngBytes == null || pngBytes.Length == 0)
                        {
                            Debug.LogError("[PhotoCompositor] PNG 인코딩 실패");
                            yield break;
                        }

                        string finalFileName = $"{sanitizedName}_{outputFileName}.png";

                        Task writeTask = File.WriteAllBytesAsync(Path.Combine(rootPath, finalFileName), pngBytes);
                        yield return new WaitUntil(() => writeTask.IsCompleted);

                        if (writeTask.IsFaulted)
                        {
                            Debug.LogError($"[PhotoCompositor] 파일 저장 예외: {writeTask.Exception?.GetBaseException().Message}");
                            yield break;
                        }

                        if (!isDebug)
                        {
                            Task uploadTask = UploadImageAsync(pngBytes, finalFileName, nextUploadCount, false);
                            yield return new WaitUntil(() => uploadTask.IsCompleted);

                            if (uploadTask.IsFaulted)
                            {
                                Debug.LogError($"[PhotoCompositor] 업로드 예외: {uploadTask.Exception?.GetBaseException().Message}");
                            }
                        }
                        else
                        {
                            Debug.Log($"<color=cyan>[PhotoCompositor] 디버그 모드 완료: {finalFileName} (PNG) 생성됨. 서버 업로드는 생략되었습니다.</color>");
                        }
                    }
                    finally
                    {
                        Destroy(screenshotTex);
                    }
                }
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private Task<byte[]> EncodeTextureToPngAsync(Texture2D texture)
        {
            byte[] rawData = texture.GetRawTextureData();
            int texWidth = texture.width;
            int texHeight = texture.height;
            UnityEngine.Experimental.Rendering.GraphicsFormat format = texture.graphicsFormat;

            return Task.Run(() =>
                ImageConversion.EncodeArrayToPNG(rawData, format, (uint)texWidth, (uint)texHeight));
        }

        private Texture2D CaptureCanvasToTexture(Canvas targetCanvas)
        {
            Canvas rootCanvas = targetCanvas.rootCanvas != null ? targetCanvas.rootCanvas : targetCanvas;
            if (!rootCanvas.isActiveAndEnabled)
            {
                Debug.LogWarning($"[PhotoCompositor] 비활성 캔버스는 캡처할 수 없습니다: {rootCanvas.name}");
                return null;
            }

            if (rootCanvas.renderMode == RenderMode.WorldSpace)
            {
                Debug.LogError($"[PhotoCompositor] World Space Canvas는 현재 캡처 대상에서 지원하지 않습니다: {rootCanvas.name}");
                return null;
            }

            int captureWidth = Mathf.Max(1, canvasCaptureResolution.x);
            int captureHeight = Mathf.Max(1, canvasCaptureResolution.y);
            RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24, RenderTextureFormat.ARGB32);
            GameObject cameraObject = new GameObject("PhotoCompositorCanvasCaptureCamera");
            cameraObject.hideFlags = HideFlags.HideAndDontSave;

            Camera captureCamera = cameraObject.AddComponent<Camera>();
            captureCamera.enabled = false;
            captureCamera.clearFlags = CameraClearFlags.SolidColor;
            captureCamera.backgroundColor = Color.clear;
            captureCamera.cullingMask = ~0;
            captureCamera.nearClipPlane = 0.01f;
            captureCamera.farClipPlane = 10f;
            captureCamera.transform.position = new Vector3(0f, 0f, -5f);
            captureCamera.targetTexture = renderTexture;

            RenderMode originalRenderMode = rootCanvas.renderMode;
            Camera originalWorldCamera = rootCanvas.worldCamera;
            float originalPlaneDistance = rootCanvas.planeDistance;
            int originalTargetDisplay = rootCanvas.targetDisplay;
            RenderTexture previousActive = RenderTexture.active;

            try
            {
                renderTexture.Create();
                rootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                rootCanvas.worldCamera = captureCamera;
                rootCanvas.planeDistance = 1f;
                rootCanvas.targetDisplay = 0;

                Canvas.ForceUpdateCanvases();
                captureCamera.Render();

                RenderTexture.active = renderTexture;
                Texture2D capturedTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
                capturedTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
                capturedTexture.Apply();
                return capturedTexture;
            }
            finally
            {
                RenderTexture.active = previousActive;
                rootCanvas.renderMode = originalRenderMode;
                rootCanvas.worldCamera = originalWorldCamera;
                rootCanvas.planeDistance = originalPlaneDistance;
                rootCanvas.targetDisplay = originalTargetDisplay;
                captureCamera.targetTexture = null;

                if (renderTexture.IsCreated())
                {
                    renderTexture.Release();
                }

                Destroy(renderTexture);
                Destroy(cameraObject);
                Canvas.ForceUpdateCanvases();
            }
        }

        /// <summary> 
        /// 서버 업로드를 Task 기반으로 처리합니다. 
        /// </summary>
        private async Task UploadImageAsync(byte[] imageBytes, string fileName, int requestCount, bool useSecondaryUrl)
        {
            if (UserDataManager.Instance.IsUser() == false)
            {
                Debug.LogWarning("[PhotoCompositor] 업로드 실패: 사용자 정보가 없습니다.");
                return;
            }

            string idxUser = UserDataManager.Instance.FindValue("IDX_USER");
            string uid = UnityWebRequest.EscapeURL(UserDataManager.Instance.FindValue("UID_LEFT"));
            string uid2 = UnityWebRequest.EscapeURL(UserDataManager.Instance.FindValue("UID_RIGHT"));

            string code = UnityWebRequest.EscapeURL(ServerData.Instance.Code);
            int safeUploadCount = Mathf.Max(1, requestCount);
            string selectedEndpoint = useSecondaryUrl && !string.IsNullOrWhiteSpace(uploadUrl2) ? uploadUrl2 : uploadUrl;
            string selectedUid = useSecondaryUrl ? uid2 : uid;

            // 파라미터 type=png, count 사용
            string requestUrl = $"{selectedEndpoint}?idx_user={idxUser}&uid={selectedUid}&code={code}&type=png&count={safeUploadCount}";

            Debug.Log("UploadImageRequest URL: " + requestUrl);

            // 전역 변수로 설정된 횟수와 딜레이 사용
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                using (UnityWebRequest webRequest = new UnityWebRequest(requestUrl, UnityWebRequest.kHttpVerbPOST))
                {
                    webRequest.uploadHandler = new UploadHandlerRaw(imageBytes);
                    webRequest.uploadHandler.contentType = "image/png";
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.timeout = 15;

                    await SendWebRequestAsync(webRequest);

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"[PhotoCompositor] 업로드 성공: {webRequest.responseCode}");
                        return; // 성공 시 루프 종료
                    }

                    if (attempt < maxRetries - 1)
                    {
                        Debug.LogWarning($"[PhotoCompositor] 업로드 실패 ({attempt + 1}/{maxRetries}): {webRequest.error}. {retryDelay}초 후 재시도...");
                        await Task.Delay(TimeSpan.FromSeconds(retryDelay));
                    }
                    else
                    {
                        Debug.LogError($"[PhotoCompositor] 업로드 최종 실패: {webRequest.error}");
                    }
                }
            }
        }

        private Task SendWebRequestAsync(UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<bool>();
            request.SendWebRequest().completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }

        private Texture2D LoadTextureFromFile(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(bytes);
                return tex;
            }
            catch { return null; }
        }

        private string GetRootPath()
        {
            string dataPath = Application.dataPath;
            DirectoryInfo parentDir = Directory.GetParent(dataPath);
            string rootPath = parentDir != null ? parentDir.FullName : dataPath;
            string savePath = Path.Combine(rootPath, saveFolderName, DateTime.Now.ToString("yyyy-MM-dd"));
            Directory.CreateDirectory(savePath);
            return savePath;
        }
    }
}