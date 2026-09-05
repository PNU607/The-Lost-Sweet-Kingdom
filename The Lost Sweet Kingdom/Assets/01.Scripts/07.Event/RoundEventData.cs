using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public enum RoundEventEffect
{
    Leave, CastleTrade, FreeRerolls, HealCastle, Ingredient, Snack,
    UpgradeTower, ColorDamage, TypeDamage, SnackTrade, RoundRerolls, RandomizeStars
}

[Serializable]
public class RoundEventData
{
    public string eventId;
    public string title;
    public string description;
    public string artResource;
    public int minRound = 1;
    public int maxRound = 30;
    public float minHpExclusive = -1;
    public float maxHpInclusive = 1;
    public int minGold;
    public int minTowers;
    public int minOneStarTowers;
    public string Key() => eventId;
}

[Serializable]
public class RoundEventChoiceData
{
    public string eventId;
    public int order;
    public string text;
    public RoundEventEffect effect;
    public float chance = 1;
    public int amountMin;
    public int amountMax;
    public float hpFraction;
    public int goldCost;
    public int costIncrease;
    public int towerCount;
    public int targetLevel;
    public string target;
    public float damageBonus;
    public int durationRounds;
    public int disabledTowers;
    public float oneStarWeight;
    public float twoStarWeight;
    public float threeStarWeight;
    public string Key() => eventId;
}

public class RoundEventSchedule
{
    public int round;
    public int Key() => round;
}

public readonly struct RoundEventConditions
{
    public readonly float HpFraction;
    public readonly int Gold, Towers, OneStarTowers;
    public RoundEventConditions(float hpFraction, int gold, int towers, int oneStarTowers)
    { HpFraction = hpFraction; Gold = gold; Towers = towers; OneStarTowers = oneStarTowers; }
}

public sealed class RoundEventCatalog
{
    public readonly IReadOnlyDictionary<string, RoundEventData> Events;
    public readonly IReadOnlyDictionary<string, List<RoundEventChoiceData>> Choices;
    public readonly HashSet<int> Rounds;

    public RoundEventCatalog(ExcelData data)
    {
        Events = data.roundEvents;
        Choices = data.roundEventChoices;
        Rounds = new HashSet<int>(data.roundEventSchedule.Keys);
        if (Events.Count == 0 || Rounds.Count == 0 || Rounds.Any(r => r < 1 || r > 30))
            throw new InvalidDataException("EventData: 이벤트/등장 라운드를 확인하세요.");
        foreach (var e in Events.Values)
        {
            if (string.IsNullOrWhiteSpace(e.eventId) || string.IsNullOrWhiteSpace(e.title) ||
                string.IsNullOrWhiteSpace(e.description) || e.minRound < 1 || e.maxRound > 30 ||
                e.minRound > e.maxRound || e.minHpExclusive < -1 || e.maxHpInclusive > 1 ||
                e.minHpExclusive >= e.maxHpInclusive || e.minGold < 0 || e.minTowers < 0 || e.minOneStarTowers < 0)
                throw new InvalidDataException($"EventData: 등장 조건 오류 ({e.eventId})");
            if (!Choices.TryGetValue(e.eventId, out var choices) || choices.Count < 2 || choices.Count > 4 ||
                choices.Select(c => c.order).Distinct().Count() != choices.Count)
                throw new InvalidDataException($"EventChoiceData: 선택지는 중복 순서 없이 2~4개여야 합니다 ({e.eventId})");
            choices.Sort((a, b) => a.order.CompareTo(b.order));
            foreach (var c in choices)
            {
                if (!Enum.IsDefined(typeof(RoundEventEffect), c.effect) || string.IsNullOrWhiteSpace(c.text) ||
                    c.order < 1 || float.IsNaN(c.chance) || c.chance < 0 || c.chance > 1 ||
                    c.amountMin < 0 || c.amountMax < c.amountMin || c.amountMax == int.MaxValue ||
                    c.goldCost < 0 || c.costIncrease < 0 || c.hpFraction < 0 || c.hpFraction >= 1 ||
                    c.towerCount < 0 || c.disabledTowers < 0 || c.durationRounds < 0 || c.damageBonus < 0)
                    throw new InvalidDataException($"EventChoiceData: 수치 오류 ({e.eventId}/{c.order})");
                if (c.effect == RoundEventEffect.ColorDamage && !Enum.TryParse<TowerColor>(c.target, out _))
                    throw new InvalidDataException($"EventChoiceData: 타워 색상 오류 ({c.target})");
                if (c.effect == RoundEventEffect.TypeDamage && !Enum.TryParse<TowerType>(c.target, out _))
                    throw new InvalidDataException($"EventChoiceData: 타워 타입 오류 ({c.target})");
                if (c.effect == RoundEventEffect.UpgradeTower && (c.targetLevel < 2 || c.targetLevel > 3))
                    throw new InvalidDataException("EventChoiceData: 진화 등급은 2 또는 3이어야 합니다.");
                if (c.effect == RoundEventEffect.RandomizeStars && (c.oneStarWeight < 0 || c.twoStarWeight < 0 ||
                    c.threeStarWeight < 0 || c.oneStarWeight + c.twoStarWeight + c.threeStarWeight <= 0 || c.towerCount < 1))
                    throw new InvalidDataException("EventChoiceData: 별 등급 가중치/타워 수 오류");
                if (c.effect == RoundEventEffect.RoundRerolls && c.durationRounds < 1)
                    throw new InvalidDataException("EventChoiceData: 추가 리롤 지속 라운드 오류");
            }
        }
        foreach (string id in Choices.Keys)
            if (!Events.ContainsKey(id)) throw new InvalidDataException($"EventChoiceData: 없는 eventId ({id})");
    }

    public static bool IsEligible(RoundEventData e, int round, RoundEventConditions state) =>
        round >= e.minRound && round <= e.maxRound && state.HpFraction > e.minHpExclusive &&
        state.HpFraction <= e.maxHpInclusive && state.Gold >= e.minGold &&
        state.Towers >= e.minTowers && state.OneStarTowers >= e.minOneStarTowers;
}

public sealed class ResolvedEventChoice
{
    public readonly RoundEventChoiceData Data;
    public readonly int Amount;
    public int Purchases { get; private set; }
    public int Cost => Data.goldCost + Purchases * Data.costIncrease;
    public ResolvedEventChoice(RoundEventChoiceData data, Random random)
    { Data = data; Amount = random.Next(data.amountMin, data.amountMax + 1); }
    public void RecordPurchase() => Purchases++;
    public string Text => Data.text.Replace("{amount}", Amount.ToString())
        .Replace("{cost}", Cost.ToString())
        .Replace("{chance}", (Data.chance * 100).ToString("0.#"))
        .Replace("{hpPercent}", (Data.hpFraction * 100).ToString("0.#"))
        .Replace("{bonusPercent}", (Data.damageBonus * 100).ToString("0.#"))
        .Replace("{level}", Data.targetLevel.ToString())
        .Replace("{count}", Data.towerCount.ToString())
        .Replace("{rounds}", Data.durationRounds.ToString())
        .Replace("{disabled}", Data.disabledTowers.ToString());
}

/// <summary>A battle-local history; shown events are consumed even when the player leaves.</summary>
public sealed class RoundEventRun
{
    private readonly HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<int> processedRounds = new();
    public readonly Random Random;
    public RoundEventRun(int seed) { Random = new Random(seed); }

    public RoundEventData Draw(RoundEventCatalog catalog, int round, RoundEventConditions state, Action<string> warn)
    {
        if (!catalog.Rounds.Contains(round) || !processedRounds.Add(round)) return null;
        var eligible = catalog.Events.Values.Where(e => RoundEventCatalog.IsEligible(e, round, state)).ToList();
        var available = eligible.Where(e => !seen.Contains(e.eventId)).ToList();
        if (available.Count == 0 && eligible.Count > 0)
        {
            warn?.Invoke("등장 가능한 이벤트를 모두 소진하여 이벤트 기록을 초기화합니다.");
            seen.Clear();
            available = eligible;
        }
        if (available.Count == 0) { warn?.Invoke("현재 조건에 맞는 이벤트가 없습니다."); return null; }
        var selected = available[Random.Next(available.Count)];
        seen.Add(selected.eventId);
        return selected;
    }
}
