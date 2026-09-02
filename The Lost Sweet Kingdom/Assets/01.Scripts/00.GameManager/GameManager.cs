using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsReady { get; private set; }
    public static Exception InitializationError { get; private set; }

    [SerializeField] private GameObject logoPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (logoPanel != null)
            {
                logoPanel.SetActive(false);
            }
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (logoPanel != null)
        {
            logoPanel.SetActive(true);
        }
    }

    private void Start()
    {
        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        IsReady = false;
        InitializationError = null;

        try
        {
            await UniTask.Yield();
            await AddressablesManager.Init();
            GameDataRepository.LoadAll();
            GameDataRepository.ValidateRequiredAssets();

            IsReady = true;
            if (logoPanel != null)
            {
                logoPanel.SetActive(false);
            }
            Debug.Log("게임 초기화 완료. 타이틀 화면을 표시합니다.");
        }
        catch (Exception exception)
        {
            InitializationError = exception;
            Debug.LogException(exception);
        }
    }
}
