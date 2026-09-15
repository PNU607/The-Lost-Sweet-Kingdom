using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.U2D.Animation;
using System.Sound;

public class Enemy : MonoBehaviour
{
    [System.NonSerialized]
    public EnemyData currentEnemyData;
    public float hp;

    private TextMeshProUGUI healthText;

    private Camera mainCamera;
    private Canvas uiCanvas;

    public AStar aStarScript;
    private float baseSpeed;
    private float moveSpeed;

    private Vector3 originalScale;

    private List<Vector2> path;

    private Animator enemyAnim;
    public SpriteLibrary spriteLibrary;
    private SpriteResolver spriteResolver;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();

        healthText = GetComponentInChildren<TextMeshProUGUI>();
        if (healthText == null)
        {
            Debug.LogWarning("Not exist HpText.");
        }

        mainCamera = Camera.main;
        uiCanvas = GetComponentInChildren<Canvas>();
        if (uiCanvas != null && uiCanvas.renderMode == RenderMode.WorldSpace && uiCanvas.worldCamera == null)
        {
            uiCanvas.worldCamera = mainCamera;
        }
    }

    private void InitializeEnemy()
    {
        StopAllCoroutines();

        int currentWave = WaveManager.instance.waveCount;
        float calculatedMaxHealth = currentEnemyData.maxHealth;

        if (currentWave > 0)
        {
            int multiplier = (currentWave - 1) / 10;
            calculatedMaxHealth += multiplier * currentEnemyData.increaseHealth;
        }

        hp = calculatedMaxHealth;

        transform.localScale = originalScale;
        baseSpeed = currentEnemyData.moveSpeed;
        moveSpeed = currentEnemyData.moveSpeed;
        spriteRenderer.color = Color.white;

        UpdateHealthText();

        path = null;

        if (aStarScript == null)
        {
            aStarScript = FindObjectOfType<AStar>();
        }

        if (aStarScript == null)
        {
            Debug.LogError("AStar script not found in the scene!");
            return;
        }

        path = aStarScript.FindPath(aStarScript.start, aStarScript.goal);
        if (path != null && path.Count > 0)
        {
            StartCoroutine(FollowPath());
        }

        enemyAnim = GetComponent<Animator>();
        spriteLibrary = GetComponent<SpriteLibrary>();
        spriteResolver = GetComponent<SpriteResolver>();

        if (spriteLibrary != null && currentEnemyData.spriteLibraryAsset != null)
        {
            spriteLibrary.spriteLibraryAsset = currentEnemyData.spriteLibraryAsset;
        }

        if (enemyAnim != null)
        {
            enemyAnim.SetBool("isOnEnable", true);
        }
    }

    IEnumerator FollowPath()
    {
        foreach (Vector2 point in path)
        {
            Vector3 targetPos = new Vector3(point.x, point.y, 0f);

            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                Vector3 dir = (targetPos - transform.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
        }
    }

    private void Update()
    {
        if (healthText != null)
        {
            Canvas canvas = healthText.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.transform.rotation = mainCamera.transform.rotation;
            }
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = Mathf.CeilToInt(Mathf.Max(0f, hp)).ToString();
        }
    }

    public virtual void TakeDamage(float damage)
    {
        TakeDamage(damage, null);
    }

    public virtual void TakeDamage(float damage, TowerType? attackerType)
    {
        if (hp <= 0) return;

        damage = CalculateDamage(damage, attackerType);

        SoundObject sound = Sound.Play("EnemyAttacked", false);
        //sound?.SetVolume(0.03f);

        //Debug.Log($"Take Damage {damage} | Total HP: {hp}");

        hp -= damage;

        //Debug.Log($"Enemy HP: {hp} / {currentEnemyData.maxHealth}");

        UpdateHealthText();

        if (hp > 0)
        {
            StartCoroutine(DoDamageReaction());
        }
        else
        {
            OnDie();
        }
    }

    private float CalculateDamage(float damage, TowerType? attackerType)
    {
        if (currentEnemyData == null)
        {
            return damage;
        }

        if (currentEnemyData.enemyType != EnemyType.Special)
        {
            return damage;
        }

        switch (currentEnemyData.specialType)
        {
            case EnemySpecialType.Bear:
                // Bear: 토끼 타워에게 50% 추가 피해
                if (attackerType == TowerType.토끼)
                {
                    damage *= 1.5f;
                }
                break;

            case EnemySpecialType.Biscuit:
                // Biscuit: 다람쥐 타워에게 50% 추가 피해
                if (attackerType == TowerType.다람쥐)
                {
                    damage *= 1.5f;
                }
                break;

            case EnemySpecialType.Pudding:
                // Pudding: 받는 피해를 30 감소
                damage = Mathf.Max(0f, damage - 30f);
                break;
        }

        return damage;
    }

    private IEnumerator DoDamageReaction()
    {
        Vector3 startScale = originalScale;
        Vector3 targetScale = originalScale * 1.3f;

        float hitDuration = 0.1f;
        float halfDuration = hitDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            transform.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }

        transform.localScale = startScale;
    }

    public void SetSpeedMultiplier(float multiplier, float duration)
    {
        //Debug.Log("Set Speed Multiplier: " + multiplier + " for duration: " + duration);
        //Debug.Log("baseSpeed: " + baseSpeed);

        moveSpeed = baseSpeed * multiplier;
        StartCoroutine(ResetSpeed(duration));
    }

    private IEnumerator ResetSpeed(float duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = baseSpeed;
    }

    public void TakeContinuousDamageForBullet(float damage, float duration)
    {
        StartCoroutine(TakeContinuousDamage(damage, duration));
    }

    private IEnumerator TakeContinuousDamage(float damage, float duration)
    {
        float timer = 0;

        while (timer <= duration)
        {
            if (!gameObject.activeInHierarchy)
            {
                yield break;
            }

            TakeDamage(damage);

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(PoisonEffect());
            }

            yield return new WaitForSeconds(0.5f);
            timer += 0.5f;
        }
    }

    private IEnumerator PoisonEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;

            yield return new WaitForSeconds(0.2f);

            spriteRenderer.color = Color.white;
        }
    }

    private void OnDie()
    {
        //Debug.Log("Die");

        SoundObject _soundObject;
        _soundObject = Sound.Play("EnemyDeath", false);
        //_soundObject.SetVolume(0.1f);

        GoldManager.instance.AddGold(currentEnemyData.goldReward);

        ObjectPool.Instance.ReturnEnemy(this.gameObject);

        if (healthText != null)
        {
            healthText.text = "0";
        }

        this.gameObject.SetActive(false);

        WaveManager.instance.enemyCountDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Castle.instance.gameObject)
        {
            Castle.instance.HealCastle(10);

            ObjectPool.Instance.ReturnEnemy(this.gameObject);

            this.gameObject.SetActive(false);

            WaveManager.instance.enemyCountDown();
        }
    }

    public EnemyData GetEnemyData()
    {
        return currentEnemyData;
    }

    public void SetEnemyData(EnemyData data)
    {
        currentEnemyData = data;

        gameObject.SetActive(true);

        InitializeEnemy();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}