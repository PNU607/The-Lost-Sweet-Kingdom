using UnityEngine;

public class towerAnimEvent : MonoBehaviour
{
    private Tower tower;

    private void Awake()
    {
        tower = this.GetComponentInParent<Tower>();
    }

    private void Attack()
    {
        tower.Attack();
    }
}
