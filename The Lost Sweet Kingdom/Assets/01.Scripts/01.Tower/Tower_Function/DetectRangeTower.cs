using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectRangeTower : RangeTower
{
    [SerializeField]
    private int attackEnemyCount = 4;

    public override void Attack()
    {
        Vector3Int towerCellPos = attackableTilemap.WorldToCell(transform.position);

        foreach (Vector2Int dir in attackDirections)
        {
            for (int i = 1; i <= applyLevelData.attackRange; i++)
            {
                Vector3Int tilePos = towerCellPos + new Vector3Int(dir.x, dir.y, 0);

                if (!attackableTilemap.HasTile(tilePos))
                    continue;

                Vector3 tileWorldPos = attackableTilemap.GetCellCenterWorld(tilePos);
                Vector2 boxSize = GetTileCheckBoxSize();

                Collider2D[] enemies = Physics2D.OverlapBoxAll(tileWorldPos, boxSize, 0f, towerBase.enemyLayer);

                if (enemies.Length > 0)
                {
                    // 배열 길이가 attackEnemyCount 이상이면 attackEnemyCount개, 그 이하면 배열 길이만큼
                    int count = enemies.Length >= attackEnemyCount ? attackEnemyCount : enemies.Length;

                    System.Random rand = new System.Random();

                    // 중복 없이 랜덤 추출
                    var randomResult = enemies
                        .OrderBy(x => rand.Next()) // 랜덤 섞기
                        .Take(count)               // count 개만 가져오기
                        .ToArray();

                    foreach (var enemyCol in randomResult)
                    {
                        if (enemyCol == null) continue;

                        if (enemyCol.TryGetComponent(out Enemy enemy))
                        {
                            candidates.Add(enemy);
                            AttackRangeTarget(enemyCol);
                        }
                    }

                    break; // 한 방향에서 첫 타일만 처리 (멀리 있는 적은 무시)
                }
            }
        }
    }
}
