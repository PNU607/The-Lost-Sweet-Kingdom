using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{
    public class Node
    {
        public Vector2 Position;
        public Node Parent;
        public float G;
        public float H;
        public float F => G + H;

        public Node(Vector2 position, Node parent = null)
        {
            Position = position;
            Parent = parent;
            G = 0;
            H = 0;
        }
    }

    public Tilemap tilemap;
    public Vector2 start;
    public Vector2 goal;

    public List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        List<Node> openList = new List<Node>();
        HashSet<Vector2> closedList = new HashSet<Vector2>();

        Node startNode = new Node(start);
        Node goalNode = new Node(goal);

        openList.Add(startNode);

        int safetyCounter = 10000;
        while (openList.Count > 0 && safetyCounter-- > 0)
        {
            Node currentNode = openList[0];
            foreach (Node node in openList)
            {
                if (node.F < currentNode.F)
                {
                    currentNode = node;
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.Position);

            if (currentNode.Position == goalNode.Position)
            {
                List<Vector2> path = new List<Vector2>();
                Node current = currentNode;
                while (current != null)
                {
                    path.Add(current.Position);
                    current = current.Parent;
                }
                path.Reverse();
                return path;
            }

            List<Vector2> neighbors = GetNeighbors(currentNode.Position);
            foreach (Vector2 neighborPos in neighbors)
            {
                if (closedList.Contains(neighborPos) || !IsWalkable(neighborPos)) continue;

                float gCost = currentNode.G + 1;
                float hCost = Vector2.Distance(neighborPos, goalNode.Position);

                Node neighborNode = new Node(neighborPos, currentNode);
                neighborNode.G = gCost;
                neighborNode.H = hCost;

                if (!openList.Contains(neighborNode))
                {
                    openList.Add(neighborNode);
                }
            }
        }

        if (safetyCounter <= 0)
        {
            Debug.LogError("A* search exceeded safety limit!");
            return null;
        }

        return null;
    }

    List<Vector2> GetNeighbors(Vector2 position)
    {
        List<Vector2> neighbors = new List<Vector2>
        {
            position + Vector2.up,
            position + Vector2.down,
            position + Vector2.left,
            position + Vector2.right
        };

        return neighbors;
    }

    bool IsWalkable(Vector2 position)
    {
        Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        return tilemap.GetTile(new Vector3Int(gridPos.x, gridPos.y, 0)) == null;
    }

    void Start()
    {
        List<Vector2> path = FindPath(start, goal);
        if (path == null)
        {
            Debug.Log("Path not found!");
        }
    }
}
