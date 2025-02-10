/* 
 * @file: EnemyManger.cs
 * @author: 서지혜
 * @date: 2025-02-05
 * @brief: 적군을 관리하는 임시 스크립트
 * @details:
 *  - 타워의 적 공격을 위한 임시 스크립트
 * @see: Enemy.cs
 * @todo: 타워 기능에 필요한 일부 함수를 제외한 기능 삭제
 * @deprecated: 다른 팀원이 작업한 기능으로 교체
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * @class: EnemyManger
 * @author: 서지혜
 * @date: 2025-02-05
 * @brief: 적군을 관리하는 클래스
 * @details:
 *  - 적군의 체력, 이동 등을 처리
 * @todo: 타워 기능에 필요한 일부 함수를 제외한 기능 삭제
 * @deprecated: 다른 팀원이 작업한 기능으로 교체
 */
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// 적 프리팹
    /// </summary>
    [SerializeField]
    private GameObject enemyPrefab;

    /// <summary>
    /// 적 생성 주기
    /// </summary>
    [SerializeField]
    private float spawnTime;

    /// <summary>
    /// 현재 스테이지의 이동 경로
    /// </summary>
    [SerializeField]
    private Transform[] wayPoints;

    /// <summary>
    /// 현재 맵에 존재하는 모든 적의 정wayPoints
    /// </summary>
    private List<Enemy> enemyList = new List<Enemy>();
    public List<Enemy> EnemeyList => enemyList;

    private void Awake()
    {
        StartCoroutine("SpawnEnemy");
    }

    /// <summary>
    /// spawnTime에 한 번씩 적 생성
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);

            yield return new WaitForSeconds(spawnTime);
        }
    }

    /// <summary>
    /// 적의 체력이 다 떨어졌을 때 적 오브젝트 파괴
    /// </summary>
    /// <param name="enemy"></param>
    public void DestroyEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    private void OnDestroy()
    {
        StopCoroutine("SpawnEnemy");
    }
}
