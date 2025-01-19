using UnityEngine;
using System.Collections.Generic;

public class PathfindingGrid : MonoBehaviour
{
    public Vector2 gridWorldSize = new(50f, 50f);
    public float nodeRadius = 0.1f;
    public LayerMask obstacleLayer;

    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        var worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                var worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                var walkable = !Physics.CheckSphere(worldPoint, nodeRadius, obstacleLayer);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        var percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        var percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

        var x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        var y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public void UpdateNode(Vector3 worldPosition, bool walkable)
    {
        var node = NodeFromWorldPoint(worldPosition);

        if (node != null)
        {
            node.walkable = walkable;
        }
    }

    public void UpdateNodesForBuilding(BoxCollider boxCollider, bool walkable)
    {
        var boxCenter = boxCollider.transform.TransformPoint(boxCollider.center);
        var boxSize = boxCollider.size / 2f;
        var boxRotation = boxCollider.transform.rotation;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                var node = grid[x, y];

                var localNodePosition = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one).inverse.MultiplyPoint3x4(node.worldPosition);

                if (Mathf.Abs(localNodePosition.x) <= boxSize.x &&
                    Mathf.Abs(localNodePosition.y) <= boxSize.y &&
                    Mathf.Abs(localNodePosition.z) <= boxSize.z)
                {
                    node.walkable = walkable;
                }
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        var neighbors = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                {
                    continue;
                }

                var checkX = node.gridX + dx;
                var checkY = node.gridY + dy;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    //private void OnDrawGizmos()
    //{
    //    if (grid == null)
    //    {
    //        return;
    //    }

    //    foreach (Node node in grid)
    //    {
    //        Gizmos.color = node.walkable ? Color.white : Color.red;
    //        Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
    //    }
    //}
}
