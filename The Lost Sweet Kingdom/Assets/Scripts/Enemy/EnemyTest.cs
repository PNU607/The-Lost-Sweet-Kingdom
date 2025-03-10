using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField]
    private EnemyData currentEnemyData;
    private float hp;

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
    }

    void Start()
    {
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

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            OnDie();
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        moveSpeed = baseSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        moveSpeed = baseSpeed;
    }

    private void OnDie()
    {
        Debug.Log("Die");
        GoldManager.instance.AddGold(10);
        ObjectPool.Instance.ReturnEnemy(this.gameObject, currentEnemyData.enemyPrefab);

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