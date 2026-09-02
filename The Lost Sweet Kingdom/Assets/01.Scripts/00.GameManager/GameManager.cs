using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    private const float MinimumLogoSeconds = 1f;

    public static GameManager Instance { get; private set; }
    public static bool IsReady { get; private set; }
    public static Exception InitializationError { get; private set; }

    private bool showBootstrapView = true;
    private GUIStyle logoStyle;
    private GUIStyle statusStyle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateBootstrapManager()
    {
        if (FindFirstObjectByType<GameManager>() != null)
        {
            return;
        }

        var bootstrapObject = new GameObject(nameof(GameManager));
        bootstrapObject.AddComponent<GameManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        float startTime = Time.realtimeSinceStartup;
        IsReady = false;
        InitializationError = null;

        try
        {
            // 첫 프레임에 로고를 실제로 그린 뒤 무거운 초기화를 시작한다.
            await UniTask.Yield();
            await AddressablesManager.Init();
            GameDataRepository.LoadAll();
            GameDataRepository.ValidateRequiredAssets();

            float elapsed = Time.realtimeSinceStartup - startTime;
            float remaining = MinimumLogoSeconds - elapsed;
            if (remaining > 0f)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(remaining),
                    ignoreTimeScale: true);
            }

            IsReady = true;
            showBootstrapView = false;
            Debug.Log("게임 초기화 완료. 타이틀 화면을 표시합니다.");
        }
        catch (Exception exception)
        {
            InitializationError = exception;
            Debug.LogException(exception);
        }
    }

    private void OnGUI()
    {
        if (!showBootstrapView)
        {
            return;
        }

        EnsureStyles();
        GUI.depth = -10000;

        Color previousColor = GUI.color;
        GUI.color = new Color(0.035f, 0.025f, 0.055f, 1f);
        GUI.DrawTexture(
            new Rect(0f, 0f, Screen.width, Screen.height),
            Texture2D.whiteTexture,
            ScaleMode.StretchToFill);
        GUI.color = previousColor;

        float logoHeight = Mathf.Min(240f, Screen.height * 0.35f);
        var logoRect = new Rect(0f, Screen.height * 0.28f, Screen.width, logoHeight);
        GUI.Label(logoRect, "THE LOST\nSWEET KINGDOM", logoStyle);

        string status = InitializationError == null
            ? "LOADING..."
            : "초기화에 실패했습니다.\n" + InitializationError.Message;
        var statusRect = new Rect(24f, Screen.height * 0.72f, Screen.width - 48f, 100f);
        GUI.Label(statusRect, status, statusStyle);

        if (Event.current != null &&
            (Event.current.isMouse || Event.current.isKey))
        {
            Event.current.Use();
        }
    }

    private void EnsureStyles()
    {
        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Clamp(Screen.height / 14, 32, 78),
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.83f, 0.43f) },
            };
        }

        if (statusStyle == null)
        {
            statusStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = Mathf.Clamp(Screen.height / 45, 16, 28),
                wordWrap = true,
                normal = { textColor = Color.white },
            };
        }
    }
}
