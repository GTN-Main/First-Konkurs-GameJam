using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class HexagonalGridPathFinding : MonoBehaviour
{
    public static HexagonalGridPathFinding Instance { get; private set; }
    MinHeap<HexCell> sortTree;
    public GameObject from;
    public GameObject to;

    public bool activated = false;
    HexCellComparer hexCellComparer = new HexCellComparer();

    [SerializeField]
    List<HexCell> closedSet = new List<HexCell>();

    [Serializable]
    public class PathResultData
    {
        public HexCell start;
        public HexCell target;
        public List<HexCell> path;

        public bool foundPath;

        public Vector2? GetPointOnPath(float percent)
        {
            if (!foundPath)
            {
                Debug.LogError("Path not found");
                return null;
            }
            if (path == null)
            {
                Debug.LogError("Path is null");
                return null;
            }
            if (path.Count == 0)
            {
                Debug.LogError("Path is empty");
                return null;
            }

            Vector2 point;
            percent = Mathf.Clamp01(percent);

            float pathPercent = percent * path.Count;

            if (percent == 1) // if end return last point
            {
                return path[path.Count - 1].coordinates;
            }
            else if (percent == 0) // if start return first point
            {
                return path[0].coordinates;
            }

            int from = Mathf.FloorToInt(pathPercent);
            HexCell fromCell = path[from];

            int to = Mathf.CeilToInt(pathPercent);
            HexCell toCell = path[to];

            bool nearFrom = Mathf.Approximately(from, pathPercent);
            bool nearTo = Mathf.Approximately(to, pathPercent);

            // check if the point is between two cells
            if (!nearFrom && !nearTo)
            {
                // |--x------| from--percent------to

                point = Vector2.Lerp(
                    a: fromCell.coordinates,
                    b: toCell.coordinates,
                    t: pathPercent - from
                );
            }
            else // if not just return the point of nearest cell
            {
                if (nearFrom) // from <- percent
                    point = fromCell.coordinates;
                else // percent -> to
                    point = toCell.coordinates;
            }
            return point;
        }
    }

    public class HexCellComparer : IComparer<HexCell>
    {
        public int Compare(HexCell x, HexCell y)
        {
            int comparison = x.fCost.CompareTo(y.fCost);

            if (comparison == 0)
            {
                return x.hCost.CompareTo(y.hCost);
            }
            return comparison;
        }
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        HexCell.CalculateOffsets();
    }

    bool isTracingPath = false;

    void Update()
    {
        
    }

    [ContextMenu("GiveTestPlots")]
    public void GiveTestPlots()
    {
        HexCell cellA = new HexCell(
            HexCell.SnapToHex(from.transform.position)
        );
        cellA.Plot(Color.yellow, 15);
        HexCell[] neighbors = cellA.GetNeighbors();
        foreach (HexCell neighbor in neighbors)
        {
            neighbor.Plot(Color.cyan, 15);
        }
    }

    public async Task ShowPath()
    {
        PathResultData pathResult = await FindPathAsync(
            from.transform.position,
            to.transform.position
        );
        pathResult.path.RemoveAt(0);
        float t = 0;
        while (t < 1)
        {
            //Debug.Log("Point on path at t = " + t);
            /*  Vector2 point = pathResult.GetPointOnPath(t) ?? Vector3.zero;
             var Obj = Instantiate(pathObj);
             Obj.transform.SetParent(this.transform);
             Obj.transform.position = point + Vector3.up;
             Obj.name = "PathPoint";
             Destroy(Obj, 5f); */
            t += Time.deltaTime * 3;
            await Task.Delay(100);
        }
    }

    public async Task<PathResultData> FindPathAsync(Vector3 start, Vector3 end)
    {
        var tcs = new TaskCompletionSource<PathResultData>();
        while (isTracingPath)
        {
            await Task.Yield();
        }
        StartCoroutine(TracePath(tcs, start, end));
        return await tcs.Task;
    }

    public IEnumerator TracePath(
        TaskCompletionSource<PathResultData> tcs,
        Vector2 start,
        Vector2 end
    )
    {
        isTracingPath = true;
        sortTree = new MinHeap<HexCell>(hexCellComparer);
        closedSet.Clear();
        // Home => Target
        HexCell cellHome = new HexCell(HexCell.SnapToHex(start));
        HexCell cellTarget = new HexCell(HexCell.SnapToHex(end));
        cellHome.gCost = 0;
        cellHome.hCost = HexCell.GetDistanceBetween(cellHome, cellTarget);
        cellTarget.hCost = 0;
        cellHome.Plot(Color.yellow);
        cellTarget.Plot(Color.black);

        PathResultData pathResult = new PathResultData
        {
            start = cellHome,
            target = cellTarget,
            path = new List<HexCell>(),
            foundPath = false,
        };

        sortTree.Enqueue(cellHome);
        int numberOfIterations = 0;
        if (
            cellHome.isWalkable
            && cellTarget.isWalkable
            && cellHome.coordinates != cellTarget.coordinates
        )
        {
            while (sortTree.Count > 0)
            {
                if (numberOfIterations % 100 == 0)
                    yield return null;
                if (numberOfIterations > 1500)
                {
                    //Debug.Log("Path not found!");
                    tcs.SetResult(pathResult);
                    isTracingPath = false;
                    break;
                }
                HexCell current = sortTree.Dequeue();
                closedSet.Add(current);
                //current.Plot(Color.blue);
                //Debug.Log("Checking cell at: " + current.coordinates);
                if (HexCell.SnapToHex(current.coordinates) == cellTarget.coordinates)
                {
                    //Debug.Log("Path found!");
                    HexCell currentCell = current;
                    while (currentCell != cellHome)
                    {
                        if (currentCell == null)
                        {
                            tcs.SetResult(pathResult);
                            isTracingPath = false;
                            Debug.LogError(
                                "Path trace reached a null cell, check parentCell links."
                            );
                        }
                        pathResult.path.Add(currentCell);
                        currentCell.Plot(Color.green);
                        currentCell = currentCell.parentCell;
                    }
                    pathResult.foundPath = true;
                    pathResult.path.Add(cellHome);
                    pathResult.path.Reverse();

                    tcs.SetResult(pathResult);

                    cellHome.Plot(Color.yellow);
                    cellTarget.Plot(Color.black);
                    isTracingPath = false;
                    break;
                }

                List<HexCell> neighbors = current
                    .GetNeighbors()
                    .Where(n => n.isWalkable && !closedSet.Any(c => c.coordinates == n.coordinates))
                    .ToList();
                foreach (HexCell neighbor in neighbors)
                {
                    HexCell existingNeighbor = sortTree.Items.FirstOrDefault(n =>
                        n.coordinates == neighbor.coordinates
                    );
                    bool inSearch = existingNeighbor != null;
                    HexCell neighborCurrent = neighbor;
                    if (inSearch)
                        neighborCurrent = existingNeighbor;

                    float costToNeighbor = current.gCost + 1;

                    if (!inSearch || costToNeighbor < neighborCurrent.gCost)
                    {
                        neighborCurrent.gCost = costToNeighbor;
                        neighborCurrent.parentCell = current;

                        if (!inSearch)
                        {
                            neighbor.hCost = HexCell.GetDistanceBetween(neighbor, cellTarget);
                            sortTree.Enqueue(neighbor);
                            neighbor.Plot(Color.white, onlyConnentions: true);
                        }
                    }
                }
                numberOfIterations++;
            }
        }
        if (!tcs.Task.IsCompleted)
        {
            tcs.SetResult(pathResult);
        }
        isTracingPath = false;
    }

    [Serializable]
    public class HexCell
    {
        public Vector2 coordinates;
        public bool isWalkable;

        public float gCost { get; set; } // Cost from start node
        public float hCost { get; set; } // Heuristic cost to end node
        public float fCost
        {
            get { return gCost + hCost; }
        }

        [SerializeField]
        public HexCell parentCell;

        public static Collider2D[] collidersHit = new Collider2D[1];

        public HexCell(float x, float y, HexCell parentCell = null)
        {
            coordinates = new Vector2(x, y);
            this.parentCell = parentCell;
            Init();
        }

        public HexCell(Vector2 v, HexCell parentCell = null)
        {
            coordinates = v;
            this.parentCell = parentCell;
            Init();
        }

        public void Init()
        {
            int hits = Physics2D.OverlapCircleNonAlloc(
                new Vector2(coordinates.x, coordinates.y),
                cellSideSize,
                collidersHit,
                LayerMask.GetMask("Obstacle")
            );
            //Debug.Log($"Hits: {hits}");
            isWalkable = hits == 0;
            /* isWalkable =
                Mathf.PerlinNoise(coordinates.x * 0.1f + 1000f, coordinates.y * 0.1f + 1000f)
                > 0.3f; */
            gCost = float.MaxValue;
            hCost = float.MaxValue;
            if (!isWalkable)
            {
                Plot(Color.magenta);
            }
        }

        public static float cellSideSize = 0.5f; // side length of hex (center → vertex)

        public static readonly float sqrt3 = Mathf.Sqrt(3f);
        public static readonly float sin30 = Mathf.Sin(30f * Mathf.Deg2Rad);

        public static void CalculateOffsets()
        {
            Vector2[] neighborOffsetNormalized = new Vector2[6];
            Vector2[] hexVertsPositionsNormalized = new Vector2[6];
            neighborOffsetPositions = new Vector2[6];
            hexVertsPositions = new Vector2[6];

            for (int i = 0; i < 6; i++)
            {
                float angle_deg = 60f * i;
                float angleN_rad = Mathf.Deg2Rad * (angle_deg + 60);
                float angleV_rad = Mathf.Deg2Rad * (angle_deg + 30);
                neighborOffsetNormalized[i] = new Vector2(
                    Mathf.Cos(angleN_rad),
                    Mathf.Sin(angleN_rad)
                ).normalized;
                hexVertsPositionsNormalized[i] = new Vector2(
                    Mathf.Cos(angleV_rad),
                    Mathf.Sin(angleV_rad)
                ).normalized;

                // * 2 * triangle h = cellSideSize * sqrt(3) / 2
                neighborOffsetPositions[i] = neighborOffsetNormalized[i] * cellSideSize * sqrt3;
                // * triangle d = h / sin(30)
                hexVertsPositions[i] = hexVertsPositionsNormalized[i] * cellSideSize * 0.9f;
            }

            /* Debug.Log("--- Neighbor Cell Offsets Calculated ---");
            foreach (Vector2 pos in neighborOffsetPositions)
            {
                Debug.Log("Neighbor Offset: " + pos);
            }

            Debug.Log("--- Hex Cell Offsets Calculated ---");
            foreach (Vector2 pos in hexVertsPositions)
            {
                Debug.Log("Hex Vertex Position: " + pos);
            } */
        }

        // Neighbor offsets (center-to-center spacing)
        public static Vector2[] neighborOffsetPositions;

        // Hex vertices (scaled by cellSize)
        public static Vector2[] hexVertsPositions;

        public HexCell[] GetNeighbors()
        {
            HexCell[] neighbors = new HexCell[6];
            for (int i = 0; i < 6; i++)
            {
                Vector2 neighborCoords = new Vector2(
                    coordinates.x + neighborOffsetPositions[i].x,
                    coordinates.y + neighborOffsetPositions[i].y
                );
                neighbors[i] = new HexCell(neighborCoords.x, neighborCoords.y, this);
            }
            return neighbors;
        }

        public static float GetDistanceBetween(HexCell first, HexCell second)
        {
            float dx = second.coordinates.x - first.coordinates.x;
            float dy = second.coordinates.y - first.coordinates.y;
            // return (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dx + dy)) / 2f;
            return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) * 0.1f
                + Mathf.Abs(dx) * Mathf.Abs(dx)
                + Mathf.Abs(dy) * Mathf.Abs(dy);
            //return Mathf.Abs(dx) * Mathf.Abs(dx) + Mathf.Abs(dy) * Mathf.Abs(dy);
        }

        public void Plot(Color color, float duration = 0.4f, bool onlyConnentions = false)
        {
            
            Vector2[] frame = hexVertsPositions
                .Select(n =>
                {
                    return n + coordinates;
                })
                .ToArray();
            if (!onlyConnentions)
                for (int i = 0; i < frame.Length; i++)
                {
                    Vector2 start = frame[i];
                    Vector2 end = frame[(i + 1) % frame.Length];
                    Debug.DrawLine(
                        start,
                        end,
                        color,
                        duration
                    );
                }
                
            /*if (parentCell != null)
                Debug.DrawLine(
                    this.coordinates,
                    parentCell.coordinates,
                    color,
                    duration
                );*/
        }

        public static Vector2 SnapToHex(Vector2 pos)
        {
            // Convert world → axial
            float q = (pos.x * sqrt3 / 3f - pos.y / 3f) / cellSideSize;
            float r = pos.y * 2f / 3f / cellSideSize;
            float s = -q - r;

            // Round axial
            int rq = Mathf.RoundToInt(q);
            int rr = Mathf.RoundToInt(r);
            int rs = Mathf.RoundToInt(s);

            float qDiff = Mathf.Abs(rq - q);
            float rDiff = Mathf.Abs(rr - r);
            float sDiff = Mathf.Abs(rs - s);

            if (qDiff > rDiff && qDiff > sDiff)
                rq = -rr - rs;
            else if (rDiff > sDiff)
                rr = -rq - rs;
            //else
            //    rs = -rq - rr;
            // redundant since we don't use s coordinate, but left for clarity and future reference

            // Convert back axial to world
            float x = cellSideSize * sqrt3 * (rq + rr / 2f); 
            float y = cellSideSize * 1.5f * rr;

            return new Vector2(x, y);
        }
    }

    public class GizmoDrawer : MonoBehaviour
    {
        public Vector2 coordinates;
        public float radius;
        public Color color;

        public void Init(Vector2 coordinates, float radius, Color color, float duration = 1f)
        {
            this.coordinates = coordinates;
            this.radius = radius;
            this.color = color;
            Destroy(this.gameObject, duration);
        }

        public void OnDrawGizmos()
        {
            if (Application.isEditor)
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(coordinates, radius);
            }
        }
    }

    public class MinHeap<T>
    {
        public List<T> Items => items;
        private readonly List<T> items = new List<T>();
        private readonly IComparer<T> comparer;

        public MinHeap(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public int Count => items.Count;

        public void Enqueue(T item)
        {
            items.Add(item);
            BubbleUp(items.Count - 1);
        }

        public T Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("Heap is empty");

            T root = items[0];
            int lastIndex = items.Count - 1;
            items[0] = items[lastIndex];
            items.RemoveAt(lastIndex);
            if (Count > 0)
            {
                BubbleDown(0);
            }
            return root;
        }

        public T Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("Heap is empty");
            return items[0];
        }

        private void BubbleUp(int index)
        {
            int parent = (index - 1) / 2;
            // comparer.Compare returns < 0 if items[index] is "smaller" than items[parent]
            while (index > 0 && comparer.Compare(items[index], items[parent]) < 0)
            {
                Swap(index, parent);
                index = parent;
                parent = (index - 1) / 2;
            }
        }

        private void BubbleDown(int index)
        {
            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                if (leftChild < Count && comparer.Compare(items[leftChild], items[smallest]) < 0)
                {
                    smallest = leftChild;
                }

                if (rightChild < Count && comparer.Compare(items[rightChild], items[smallest]) < 0)
                {
                    smallest = rightChild;
                }

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            T temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }
}
