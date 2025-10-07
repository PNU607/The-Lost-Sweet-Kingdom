using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    public static Castle instance;

    public int castleHp;
    public int maxHp = 200;

    [SerializeField]
    private Slider hpSlider;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        hpSlider = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        castleHp = 0;

        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHp;
            hpSlider.value = castleHp;
        }
    }

    private void Update()
    {
        if (castleHp >= maxHp && !GameManager.Instance.isCleared)
        {
            Debug.Log("와 건강해졌어");
            GameManager.Instance.isCleared = true;
            Time.timeScale = 0f;
            GameManager.Instance.GameOver();
        }
    }

    public void HealCastle(int healCount)
    {
        castleHp += healCount;
        castleHp = Mathf.Min(castleHp, maxHp);

        Debug.Log($"CastleHp : {castleHp}");

        if (hpSlider != null)
        {
            hpSlider.value = castleHp;
        }
    }
}
