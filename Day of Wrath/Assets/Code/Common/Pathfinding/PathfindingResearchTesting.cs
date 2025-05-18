using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Diagnostics;

public class PathfindingExperimentRunner : MonoBehaviour
{
    public Transform testAreaCenter;
    public float testAreaSize = 50f;

    public Pathfinding aStarPathfinder;
    public NavMeshAgent dummyAgentPrefab;

    public GameObject obstaclePrefab;

    public void Start()
    {
        RunEmptyMapTest();
        RunStaticObstacleTest();
        RunDynamicObstacleTest();
    }

    private void RunEmptyMapTest()
    {
        var start = GetRandomPoint();
        var end = GetRandomPoint();

        TestAStar("EmptyMap", start, end);
        TestNavMesh("EmptyMap", start, end);
    }

    private void RunStaticObstacleTest()
    {
        var start = GetRandomPoint();
        var end = GetRandomPoint();

        var middle = Vector3.Lerp(start, end, 0.5f);
        var obstacle = Instantiate(obstaclePrefab, middle, Quaternion.identity);
        obstacle.name = "StaticObstacle";

        aStarPathfinder.grid.UpdateNodesForBuilding(obstacle.GetComponent<BoxCollider>(), false);

        TestAStar("StaticObstacle", start, end);
        TestNavMesh("StaticObstacle", start, end);

        Destroy(obstacle);
    }

    private void RunDynamicObstacleTest()
    {
        var start = GetRandomPoint();
        var end = GetRandomPoint();

        TestAStar("DynamicObstacle", start, end);
        TestNavMesh("DynamicObstacle", start, end);

        // Simulate obstacle appearing mid-path
        var middle = Vector3.Lerp(start, end, 0.5f);
        var obstacle = Instantiate(obstaclePrefab, middle, Quaternion.identity);
        obstacle.name = "DynamicObstacle";

        aStarPathfinder.grid.UpdateNodesForBuilding(obstacle.GetComponent<BoxCollider>(), false);
    }

    private void TestAStar(string scenario, Vector3 start, Vector3 end)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var path = aStarPathfinder.FindPath(start, end);

        stopwatch.Stop();
        var time = stopwatch.ElapsedMilliseconds;
        var length = CalculatePathLength(path);

        UnityEngine.Debug.Log($"[A*][{scenario}] Time: {time}ms; Path Length: {length:F2}m; Success: {path.Count > 0}");
    }

    private void TestNavMesh(string scenario, Vector3 start, Vector3 end)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var navPath = new NavMeshPath();
        var success = NavMesh.CalculatePath(start, end, NavMesh.AllAreas, navPath);

        stopwatch.Stop();
        var time = stopwatch.ElapsedMilliseconds;
        var length = CalculateNavPathLength(navPath);

        UnityEngine.Debug.Log($"[NavMesh][{scenario}] Time: {time}ms; Path Length: {length:F2}m; Success: {success}");
    }

    private Vector3 GetRandomPoint()
    {
        var half = testAreaSize / 2f;
        var center = testAreaCenter.position;

        return new Vector3(
            Random.Range(center.x - half, center.x + half),
            0,
            Random.Range(center.z - half, center.z + half)
        );
    }

    private float CalculatePathLength(List<Vector3> path)
    {
        if (path == null || path.Count < 2) return 0f;
        var total = 0f;
        for (int i = 1; i < path.Count; i++)
        {
            total += Vector3.Distance(path[i - 1], path[i]);
        }
        return total;
    }

    private float CalculateNavPathLength(NavMeshPath path)
    {
        if (path == null || path.corners.Length < 2) return 0f;
        var total = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            total += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return total;
    }
}