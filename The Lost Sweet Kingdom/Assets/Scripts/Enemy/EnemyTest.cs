using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField]
    private EnemyData currentEnemyData;
    public float hp;

    public GameObject healthBarPrefab;
    private Transform healthBarInstance;
    private Transform foreground;
    private Vector3 initialForegroundScale;

    public AStar aStarScript;
    private float baseSpeed;
    private float moveSpeed;

    private List<Vector2> path;
    private int currentTargetIndex = 0;

    private void OnEnable()
    {
        hp = currentEnemyData.maxHealth;
        baseSpeed = currentEnemyData.moveSpeed;
        moveSpeed = currentEnemyData.moveSpeed;

        currentTargetIndex = 0;
        path = null;

        if (healthBarInstance == null)
        {
            GameObject go = Instantiate(healthBarPrefab, transform);
            healthBarInstance = go.transform;
            healthBarInstance.localPosition = new Vector3(-0.3f, -0.7f, 0);
            foreground = healthBarInstance.Find("Foreground");
            initialForegroundScale = foreground.localScale;
        }

        UpdateHealthBar();

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
    }

    IEnumerator FollowPath()
    {
        while (currentTargetIndex < path.Count)
        {
            Vector2 targetPosition = path[currentTargetIndex];
            Vector3 targetPos3D = new Vector3(targetPosition.x, targetPosition.y, 0);

            while ((transform.position - targetPos3D).sqrMagnitude > 0.01f)
            {
                Vector3 newPosition = transform.position;

                if (Mathf.Abs(targetPos3D.x - transform.position.x) > 0.01f)
                {
                    newPosition.x = Mathf.MoveTowards(transform.position.x, targetPos3D.x, moveSpeed * Time.deltaTime);
                }
                else if (Mathf.Abs(targetPos3D.y - transform.position.y) > 0.01f)
                {
                    newPosition.y = Mathf.MoveTowards(transform.position.y, targetPos3D.y, moveSpeed * Time.deltaTime);
                }

                transform.position = newPosition;
                yield return null;
            }

            currentTargetIndex++;

        }
    }

    void UpdateHealthBar()
    {
        if (foreground != null)
        {
            float ratio = hp / currentEnemyData.maxHealth;
            foreground.localScale = new Vector3(initialForegroundScale.x * ratio, initialForegroundScale.y, initialForegroundScale.z);
        }
    }

    public void TakeDamage(float damage)
    {
        SoundObject _soundObject;
        _soundObject = Sound.Play("EnemyAttacked", false);

        Debug.Log("Take Damage " + damage + " Total HP " + hp);
        hp -= damage;
        UpdateHealthBar();

        if (hp <= 0)
        {
            OnDie();
        }
    }

    public void SetSpeedMultiplier(float multiplier, float duration)
    {
        moveSpeed = baseSpeed * multiplier;

        // 원래 속도로 복구
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
            timer += Time.deltaTime;
        }
    }

    private void OnDie()
    {
        Debug.Log("Die");
        GoldManager.instance.AddGold(10);
        ObjectPool.Instance.ReturnEnemy(this.gameObject, currentEnemyData.enemyPrefab);

        Destroy(healthBarInstance?.gameObject);

        this.gameObject.SetActive(false);
        WaveManager.instance.enemyCountDown();
    }

    void OnDestroy()
    {
        ObjectPool.Instance.ReturnEnemy(this.gameObject, currentEnemyData.enemyPrefab);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Castle.instance.gameObject)
        {
            Castle.instance.TakeDamage(10);
            ObjectPool.Instance.ReturnEnemy(this.gameObject, currentEnemyData.enemyPrefab);
            WaveManager.instance.enemyCountDown();
        }
    }
};