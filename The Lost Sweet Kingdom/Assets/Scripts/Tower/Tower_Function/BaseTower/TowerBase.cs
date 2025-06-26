using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour
{
    /// <summary>
    /// �߻�ü ������ ��ġ (transform)
    /// </summary>
    public Transform weaponSpawnTransform;

    /// <summary>
    /// Ÿ�� �巡�� �� ��ġ ������
    /// </summary>
    public Vector3 offset;
    /// <summary>
    /// Ÿ���� �ݶ��̴�
    /// </summary>
    public Collider2D towerCollider;

    /// <summary>
    /// Ÿ���� ���� ���� ǥ�ÿ� SpriteRenderer
    /// </summary>
    public SpriteRenderer rangeIndicator;

    /// <summary>
    /// �� ���̾�
    /// </summary>
    public LayerMask enemyLayer;

    /// <summary>
    /// Ÿ���� �ִϸ�����
    /// </summary>
    public Animator towerAnim;

    /// <summary>
    /// Ÿ���� ��������Ʈ ������
    /// </summary>
    public SpriteRenderer towerSprite;

    public GameObject StarArea; 
    public GameObject LevelStarObj;
}
