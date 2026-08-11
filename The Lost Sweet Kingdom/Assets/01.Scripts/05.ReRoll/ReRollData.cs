using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReRollData", menuName = "ScriptableObject/ReRollData")]
public class ReRollData : ScriptableObject
{
    public List<Unit> units;
}
