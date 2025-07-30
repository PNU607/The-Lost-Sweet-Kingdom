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
        castleHp = 0;
    }

    private void Update()
    {
        if (castleHp == 200)
        {
            Debug.Log("와 건강해졌어");
        }
    }

    public void HealCastle(int healCount)
    {
        castleHp += healCount;
        Debug.Log($"CastleHp : {castleHp}");
    }
}
