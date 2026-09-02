using System;

[Serializable]
public class Unit
{
    public string unitName;
    public string towerId;
    [ExcelParer(ignore: true)]
    public TowerData towerData;
    public float spawnProbability;
}
