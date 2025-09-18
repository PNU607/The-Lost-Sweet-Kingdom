using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSlot : MonoBehaviour
{
    //[SerializeField]
    //private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI difficultyText;
    [SerializeField]
    private GameObject lockIcon;
    [SerializeField]
    private TextMeshProUGUI clearText;
    [SerializeField]
    private Button stageBtn;
    [SerializeField]
    private CanvasGroup overlayImage;

    private StageData currentData;

    public void Awake()
    {
        stageBtn.onClick.AddListener(LoadStageScene);
    }

    public void SetData(StageData data)
    {
        currentData = data;

        //nameText.text = data.stageName;
        stageBtn.targetGraphic.GetComponent<Image>().sprite = data.stageSprite;
        difficultyText.text = data.difficulty.ToString();
        lockIcon.SetActive(data.isLocked);
        clearText.text = data.isCleared ? "클리어 완료!" : "";
    }

    public StageData GetStageData()
    {
        return currentData;
    }

    public string GetStageName()
    {
        return currentData?.stageName ?? "Unknown Stage";
    }

    public void OverlayOn()
    {
        // 오버레이 효과 구현
        overlayImage.blocksRaycasts = true;
        overlayImage.alpha = 1f;

        stageBtn.interactable = false;
    }

    public void OverlayOff()
    {
        // 오버레이 효과 제거 구현
        overlayImage.blocksRaycasts = false;
        overlayImage.alpha = 0f;

        if (!currentData.isLocked)
            stageBtn.interactable = true;
    }

    private void LoadStageScene()
    {
        SceneManager.LoadScene(currentData.name);
    }

    public void OnDestroy()
    {
        stageBtn.onClick.RemoveListener(LoadStageScene);
    }
}