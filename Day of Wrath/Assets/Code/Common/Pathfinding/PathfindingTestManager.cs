using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Diagnostics;

public class PathfindingTestManager : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform navMeshTargetMarker;

    public Pathfinding aStarPathfinder;
    public PathfindingGrid aStarGrid;

    public int testCount = 10;
    public Vector2 testAreaSize = new Vector2(40f, 40f);
    public GameObject dynamicObstaclePrefab;

    private void Start()
    {
        StartCoroutine(RunAllTests());
    }

    private IEnumerator RunAllTests()
    {
        yield return RunTestScenario("EmptyMap", false, false);
        yield return RunTestScenario("ObstacleMap", true, false);
        yield return RunTestScenario("DynamicObstacle", true, true);

        yield return RunUpdateOnlyTest();
    }

    private IEnumerator RunTestScenario(string scenarioName, bool placeObstacle, bool dynamic)
    {
        for (int i = 0; i < testCount; i++)
        {
            var start = RandomPosition();
            var end = RandomPosition();
            var straightLine = Vector3.Distance(start, end);

            if (placeObstacle)
            {
                SpawnStaticObstacleBetween(start, end);
            }

            // A* Test
            var stopwatch = Stopwatch.StartNew();
            var aStarPath = aStarPathfinder.FindPath(start, end);
            stopwatch.Stop();

            var aStarTime = stopwatch.ElapsedMilliseconds;
            var aStarLength = CalculatePathLength(aStarPath);
            var aStarSuccess = aStarPath.Count > 0;

            UnityEngine.Debug.Log($"[A*][{scenarioName}] From {Format(start)} to {Format(end)} | Straight: {straightLine:F2}m | Path: {aStarLength:F2}m | Time: {aStarTime:F2}ms | Success: {aStarSuccess}");

            // NavMesh Test
            stopwatch.Reset();
            navMeshAgent.Warp(start);
            navMeshTargetMarker.position = end;
            stopwatch.Start();

            var navPath = new NavMeshPath();
            var navSuccess = NavMesh.CalculatePath(start, end, NavMesh.AllAreas, navPath);
            stopwatch.Stop();

            var navTime = stopwatch.ElapsedMilliseconds;
            var navLength = CalculatePathLength(navPath);

            UnityEngine.Debug.Log($"[NavMesh][{scenarioName}] From {Format(start)} to {Format(end)} | Straight: {straightLine:F2}m | Path: {navLength:F2}m | Time: {navTime:F2}ms | Success: {navSuccess}");

            if (dynamic)
            {
                var mid = (start + end) / 2f;
                var dynamicObstacle = Instantiate(dynamicObstaclePrefab, mid, Quaternion.identity);

                aStarGrid.GenerateGrid();
                var surface = FindObjectOfType<NavMeshSurface>();

                if (surface != null)
                {
                    surface.BuildNavMesh();
                }

                yield return new WaitForSeconds(2f);
                Destroy(dynamicObstacle);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator RunUpdateOnlyTest()
    {
        for (int i = 0; i < testCount; i++)
        {
            var randomPos = RandomPosition();
            GameObject obstacle = Instantiate(dynamicObstaclePrefab, randomPos, Quaternion.identity);

            var stopwatch = Stopwatch.StartNew();
            aStarGrid.GenerateGrid();
            stopwatch.Stop();

            UnityEngine.Debug.Log($"[A*][GridUpdate] Update at {Format(randomPos)} | Time: {stopwatch.ElapsedMilliseconds}ms");

            stopwatch.Reset();
            var surface = FindObjectOfType<NavMeshSurface>();

            stopwatch.Start();
            surface.BuildNavMesh();
            stopwatch.Stop();

            UnityEngine.Debug.Log($"[NavMesh][Update] Update at {Format(randomPos)} | Time: {stopwatch.ElapsedMilliseconds}ms");

            Destroy(obstacle);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private Vector3 RandomPosition()
    {
        return new Vector3(
            Random.Range(-testAreaSize.x / 2, testAreaSize.x / 2),
            0,
            Random.Range(-testAreaSize.y / 2, testAreaSize.y / 2)
        );
    }

    private void SpawnStaticObstacleBetween(Vector3 a, Vector3 b)
    {
        Vector3 middle = (a + b) / 2f;
        Instantiate(dynamicObstaclePrefab, middle, Quaternion.identity);
    }

    private float CalculatePathLength(List<Vector3> path)
    {
        if (path == null || path.Count < 2) return 0f;

        float length = 0f;
        for (int i = 1; i < path.Count; i++)
        {
            length += Vector3.Distance(path[i - 1], path[i]);
        }
        return length;
    }

    private float CalculatePathLength(NavMeshPath path)
    {
        if (path == null || path.corners.Length < 2) return 0f;

        float length = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }

    private string Format(Vector3 pos)
    {
        return $"({pos.x:F1}, {pos.z:F1})";
    }

    private class NavMeshSurface : MonoBehaviour
    {
        public void BuildNavMesh()
        {
            return;
        }
    }
}
