using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public AStar aStarScript;
    public float moveSpeed = 2f;

    private List<Vector2> path;
    private int currentTargetIndex = 0;

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

            while ((transform.position - targetPos3D).sqrMagnitude > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos3D, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentTargetIndex++;
        }
    }

    void OnDestroy()
    {
        ObjectPool.Instance.ReturnEnemy(this.gameObject, this.gameObject);

    }
}
