using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSlot : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI difficultyText;
    public GameObject lockIcon;

    public void SetData(StageData data)
    {
        nameText.text = data.stageName;
        difficultyText.text = "Lv." + data.difficulty;
        lockIcon.SetActive(data.isLocked);
    }

    public void OnItemClicked()
    {
        SceneManager.LoadScene(nameText.text);
    }
}