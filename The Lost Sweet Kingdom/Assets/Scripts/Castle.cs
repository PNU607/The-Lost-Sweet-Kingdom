using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public static Castle instance;

    public int castleHp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        castleHp = 100;
    }

    public void TakeDamage(int damage)
    {
        castleHp -= damage;
        Debug.Log($"CastleHp : {castleHp}");
    }
}
