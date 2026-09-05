using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Owned by the stage's WaveManager, so history and bonuses end with the battle.</summary>
public class RoundEventController : MonoBehaviour
{
    public static RoundEventController Instance { get; private set; }
    public static bool IsBlocking => Instance != null && Instance.IsOpen;
    [SerializeField] private RoundEventPanel panelPrefab;
    public bool IsOpen { get; private set; }
    public RoundEventData CurrentEvent { get; private set; }
    public IReadOnlyList<ResolvedEventChoice> CurrentChoices => choices;
    public string LastResult { get; private set; }

    private RoundEventRun run;
    private RoundEventPanel panel;
    private List<ResolvedEventChoice> choices;
    private readonly Dictionary<TowerColor, float> colorBonuses = new();
    private readonly Dictionary<TowerType, float> typeBonuses = new();
    private readonly Dictionary<Tower, int> sleepingTowers = new();
    private readonly List<(int first, int last, int amount)> rerollGrants = new();
    private int completedRound;
    private int preparedRound;
    private float previousTimeScale;
    private bool awaitingContinue;
    private bool applying;

    private void Awake()
    {
        Instance = this;
        run = new RoundEventRun(Environment.TickCount);
    }

    private List<Tower> FieldTowers() => TowerManager.Instance != null
        ? TowerManager.Instance.GetPlacedTowers() : new List<Tower>();

    public IEnumerator AfterRound(int round)
    {
        CompleteRound(round);
        GameDataRepository.EnsureLoaded();
        var towers = FieldTowers();
        var state = new RoundEventConditions(Castle.instance != null ? Castle.instance.RemainingHealthFraction : 0,
            GoldManager.instance != null ? GoldManager.instance.gold : 0, towers.Count, towers.Count(t => t.towerLevel == 1));
        var selected = run.Draw(GameDataRepository.EventCatalog, round, state, Debug.LogWarning);
        if (selected == null) yield break;
        Open(selected, round);
        while (IsOpen) yield return null;
    }

    private void Open(RoundEventData data, int round)
    {
        if (panel == null)
        {
            var prefab = panelPrefab != null ? panelPrefab : Resources.Load<RoundEventPanel>("Prefabs/Events/RoundEventPanel");
            if (prefab == null) throw new InvalidOperationException("RoundEventPanel 프리팹이 없습니다. Tools/Events/Create Event UI Prefab을 실행하세요.");
            panel = Instantiate(prefab);
        }
        completedRound = round;
        CurrentEvent = data;
        choices = GameDataRepository.EventCatalog.Choices[data.eventId]
            .Select(c => new ResolvedEventChoice(c, run.Random)).ToList();
        previousTimeScale = Time.timeScale;
        IsOpen = true;
        awaitingContinue = false;
        LastResult = "";
        Time.timeScale = 0;
        panel.Show(this, data, choices, round);
    }

    public string DisabledReason(ResolvedEventChoice choice)
    {
        var c = choice.Data;
        if (c.effect == RoundEventEffect.HealCastle)
        {
            if (Castle.instance == null) return "성을 찾을 수 없습니다.";
            if (Castle.instance.RemainingHealth >= Castle.instance.maxHp) return "HP가 최대입니다.";
            if (GoldManager.instance == null || GoldManager.instance.gold < choice.Cost) return "골드가 부족합니다.";
        }
        if (c.effect == RoundEventEffect.CastleTrade && (Castle.instance == null ||
            Castle.instance.RemainingHealth <= Mathf.CeilToInt(Castle.instance.maxHp * c.hpFraction)))
            return "성 체력이 부족합니다.";
        if (c.effect == RoundEventEffect.UpgradeTower && !FieldTowers().Any(t => t.towerLevel == 1))
            return "1성 타워가 없습니다.";
        if ((c.effect == RoundEventEffect.SnackTrade || c.effect == RoundEventEffect.RandomizeStars) && FieldTowers().Count < c.towerCount)
            return "타워가 부족합니다.";
        if (c.effect == RoundEventEffect.RoundRerolls && FieldTowers().Count < c.disabledTowers)
            return "타워가 부족합니다.";
        return null;
    }

    public void Choose(int index)
    {
        if (!IsOpen || applying || awaitingContinue || index < 0 || index >= choices.Count || !panel.Ready) return;
        var choice = choices[index];
        var disabled = DisabledReason(choice);
        if (disabled != null) { panel.RefreshChoices(disabled); return; }
        applying = true;
        try
        {
            LastResult = Apply(choice);
            if (choice.Data.effect == RoundEventEffect.HealCastle)
                panel.RefreshChoices(LastResult);
            else if (choice.Data.effect == RoundEventEffect.Leave)
                Continue();
            else
            {
                awaitingContinue = true;
                panel.ShowResult(LastResult);
            }
        }
        finally { applying = false; }
    }

    private string Apply(ResolvedEventChoice choice)
    {
        var c = choice.Data;
        if (run.Random.NextDouble() >= c.chance) return "아쉽게도 행운이 찾아오지 않았습니다. 아무 변화 없이 길을 이어갑니다.";
        switch (c.effect)
        {
            case RoundEventEffect.CastleTrade:
                int damage = Mathf.CeilToInt(Castle.instance.maxHp * c.hpFraction);
                Castle.instance.TakeEventDamage(damage);
                GoldManager.instance.AddGold(choice.Amount);
                return $"성 체력 {damage} 감소. {choice.Amount}G를 얻었습니다.";
            case RoundEventEffect.FreeRerolls:
                WaveManager.instance.reRollManager.AddFreeRerolls(choice.Amount);
                return $"무료 리롤 {choice.Amount}회를 얻었습니다.";
            case RoundEventEffect.HealCastle:
                int cost = choice.Cost;
                GoldManager.instance.SpendGold(cost);
                int healed = Castle.instance.RestoreHealth(choice.Amount);
                choice.RecordPurchase();
                return $"{cost}G를 내고 체력 {healed}을 회복했습니다.";
            case RoundEventEffect.Ingredient:
            case RoundEventEffect.Snack:
                return "선택을 마쳤습니다. 재료·과자 지급은 추후 추가될 예정입니다.";
            case RoundEventEffect.UpgradeTower:
                var upgraded = Pick(FieldTowers().Where(t => t.towerLevel == 1).ToList(), 1)[0];
                upgraded.Setup(upgraded.CurrentTowerData, c.targetLevel);
                TowerBonusManager.Instance.RefreshBonuses();
                return $"{upgraded.CurrentTowerData.towerName} 타워가 {c.targetLevel}성으로 진화했습니다.";
            case RoundEventEffect.ColorDamage:
                var color = (TowerColor)Enum.Parse(typeof(TowerColor), c.target);
                colorBonuses.TryGetValue(color, out float colorBonus);
                colorBonuses[color] = colorBonus + c.damageBonus;
                TowerBonusManager.Instance.RefreshBonuses();
                return choice.Text;
            case RoundEventEffect.TypeDamage:
                var type = (TowerType)Enum.Parse(typeof(TowerType), c.target);
                typeBonuses.TryGetValue(type, out float typeBonus);
                typeBonuses[type] = typeBonus + c.damageBonus;
                TowerBonusManager.Instance.RefreshBonuses();
                return choice.Text;
            case RoundEventEffect.SnackTrade:
                foreach (var tower in Pick(FieldTowers(), c.towerCount)) TowerManager.Instance.DestroyTower(tower.gameObject);
                return $"타워 {c.towerCount}개를 보냈습니다. 과자 지급은 추후 추가될 예정입니다.";
            case RoundEventEffect.RoundRerolls:
                rerollGrants.Add((completedRound + 1, completedRound + c.durationRounds, choice.Amount));
                foreach (var tower in Pick(FieldTowers(), c.disabledTowers))
                {
                    sleepingTowers[tower] = completedRound + 1;
                    tower.SetEventResting(true);
                }
                return choice.Text;
            case RoundEventEffect.RandomizeStars:
                var results = new List<string>();
                foreach (var tower in Pick(FieldTowers(), c.towerCount))
                {
                    int level = RollStarLevel(c, run.Random.NextDouble());
                    tower.Setup(tower.CurrentTowerData, level);
                    results.Add($"{tower.CurrentTowerData.towerName}: {level}성");
                }
                TowerBonusManager.Instance.RefreshBonuses();
                return string.Join("\n", results);
            default: return "";
        }
    }

    public static int RollStarLevel(RoundEventChoiceData data, double random01)
    {
        double value = random01 * (data.oneStarWeight + data.twoStarWeight + data.threeStarWeight);
        return value < data.oneStarWeight ? 1 : value < data.oneStarWeight + data.twoStarWeight ? 2 : 3;
    }

    private List<Tower> Pick(List<Tower> towers, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int j = run.Random.Next(i, towers.Count);
            (towers[i], towers[j]) = (towers[j], towers[i]);
        }
        return towers.Take(count).ToList();
    }

    public float DamageMultiplier(TowerData data)
    {
        colorBonuses.TryGetValue(data.towerColor, out float color);
        typeBonuses.TryGetValue(data.towerType, out float type);
        return 1 + color + type;
    }

    public void PrepareRound(int round)
    {
        if (round == preparedRound) return;
        preparedRound = round;
        int amount = rerollGrants.Where(g => round >= g.first && round <= g.last).Sum(g => g.amount);
        if (WaveManager.instance.reRollManager != null) WaveManager.instance.reRollManager.SetRoundFreeRerolls(amount);
    }

    public void CompleteRound(int round)
    {
        foreach (var entry in sleepingTowers.ToArray())
        {
            if (entry.Value > round) continue;
            if (entry.Key != null) entry.Key.SetEventResting(false);
            sleepingTowers.Remove(entry.Key);
        }
        rerollGrants.RemoveAll(g => g.last <= round);
        if (WaveManager.instance.reRollManager != null) WaveManager.instance.reRollManager.SetRoundFreeRerolls(0);
    }

    public void Continue()
    {
        if (!IsOpen) return;
        IsOpen = false;
        awaitingContinue = false;
        panel.Hide();
        Time.timeScale = BattleManager.Instance != null && BattleManager.Instance.isCleared ? 0 : previousTimeScale;
    }

    private void OnDestroy()
    {
        if (IsOpen) Time.timeScale = previousTimeScale;
        if (panel != null) Destroy(panel.gameObject);
        if (Instance == this) Instance = null;
    }

#if UNITY_EDITOR
    public void PreviewEvent(string id, int round)
    {
        if (!Application.isPlaying || IsOpen) return;
        GameDataRepository.EnsureLoaded();
        Open(GameDataRepository.EventCatalog.Events[id], round);
    }
#endif
}
