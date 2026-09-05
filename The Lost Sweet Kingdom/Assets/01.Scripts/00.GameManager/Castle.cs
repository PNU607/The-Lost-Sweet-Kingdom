using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    public static Castle instance;

    public int castleHp;
    public int maxHp = 200;

    // Legacy castleHp stores accumulated enemy healing/damage (max means defeat).
    // Event HP uses remaining durability without changing the existing enemy/slider contract.
    public int RemainingHealth => Mathf.Max(0, maxHp - castleHp);
    public float RemainingHealthFraction => maxHp > 0 ? (float)RemainingHealth / maxHp : 0;

    public int RestoreHealth(int amount)
    {
        int restored = Mathf.Min(Mathf.Max(0, amount), castleHp);
        castleHp -= restored;
        if (hpSlider != null) hpSlider.value = castleHp;
        return restored;
    }

    public void TakeEventDamage(int amount) => HealCastle(Mathf.Max(0, amount));

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
        if (castleHp >= maxHp && !BattleManager.Instance.isCleared)
        {
            Debug.Log("와 건강해졌어");
            BattleManager.Instance.isCleared = true;
            Time.timeScale = 0f;
            BattleManager.Instance.GameOver();
        }
    }

    public void HealCastle(int healCount)
    {
        castleHp += healCount;
        castleHp = Mathf.Min(castleHp, maxHp);

        //Debug.Log($"CastleHp : {castleHp}");
        Debug.Log($"CastleHp : {castleHp}");

        if (hpSlider != null)
        {
            hpSlider.value = castleHp;
        }
    }
}
