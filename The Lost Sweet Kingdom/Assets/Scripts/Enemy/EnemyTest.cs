using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using System.Sound;

public class EnemyTest : MonoBehaviour
{
    [SerializeField]
    private EnemyData currentEnemyData;
    public float hp;

    private Slider healthSlider;

    private Camera mainCamera;
    private Canvas uiCanvas;

    public AStar aStarScript;
    private float baseSpeed;
    private float moveSpeed;

    private Vector3 originalScale;

    private List<Vector2> path;

    private Animator enemyAnim;
    private SpriteLibrary spriteLibrary;
    private SpriteResolver spriteResolver;

    private void OnEnable()
    {
        if (currentEnemyData == null)
        {
            return;
        }

        InitializeEnemy();
    }

    private void Awake()
    {
        originalScale = transform.localScale;

        healthSlider = GetComponentInChildren<Slider>();
        if (healthSlider == null)
        {
            Debug.LogWarning("Not exist Hpbar.");
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

        hp = currentEnemyData.maxHealth;
        transform.localScale = originalScale;
        baseSpeed = currentEnemyData.moveSpeed;
        moveSpeed = currentEnemyData.moveSpeed;

        UpdateHealthBar();

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
        if (healthSlider != null)
        {
            Canvas canvas = healthSlider.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.transform.rotation = mainCamera.transform.rotation;
            }
        }
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null && currentEnemyData != null)
        {
            healthSlider.value = hp / currentEnemyData.maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (hp <= 0) return;

        SoundObject sound = Sound.Play("EnemyAttacked", false);
        sound?.SetVolume(0.03f);

        //Debug.Log($"Take Damage {damage} | Total HP: {hp}");

        hp -= damage;
        UpdateHealthBar();

        if (hp > 0)
        {
            StartCoroutine(DoDamageReaction());
        }
        else
        {
            OnDie();
        }
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
        Debug.Log("Set Speed Multiplier: " + multiplier + " for duration: " + duration);
        Debug.Log("baseSpeed: " + baseSpeed);

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
            TakeDamage(damage);
            yield return new WaitForSeconds(0.5f);
            timer += 0.5f;
        }
    }

    private void OnDie()
    {
        Debug.Log("Die");
        GoldManager.instance.AddGold(currentEnemyData.goldReward);
        ObjectPool.Instance.ReturnEnemy(this.gameObject, currentEnemyData);

        if (healthSlider != null)
        {
            healthSlider.value = 0f;
        }

        this.gameObject.SetActive(false);

        WaveManager.instance.enemyCountDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Castle.instance.gameObject)
        {
            Castle.instance.TakeDamage(10);
            ObjectPool.Instance.ReturnEnemy(this.gameObject, currentEnemyData);
            this.gameObject.SetActive(false);

            WaveManager.instance.enemyCountDown();
        }
    }

    public void SetEnemyData(EnemyData data)
    {
        currentEnemyData = data;
        InitializeEnemy();
    }
}
