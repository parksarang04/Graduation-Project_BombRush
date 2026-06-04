using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [Header("맵 크기 설정")]
    [SerializeField] private float mapLength = 70f;
    [SerializeField] private float mapWidth = 10f;
    [SerializeField] private float wallHeight = 5f;

    public static Vector3 Center { get; private set; }
    public static float MaxLength { get; private set; }
    public static float MaxWidth { get; private set; }

    private void OnEnable()
    {
        // 기존 벽 제거
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Center = transform.position;

        float worldScale = transform.lossyScale.x;
        MaxLength = (mapLength / 2f) * worldScale;
        MaxWidth = (mapWidth / 2f) * worldScale;

        Debug.Log($"MapBoundary - Center:{Center} Width:{MaxWidth} Length:{MaxLength} Scale:{worldScale}");

        CreateWalls();
    }

    private void CreateWalls()
    {
        CreateWall("Wall_Front",
            new Vector3(0, wallHeight / 2f, mapLength / 2f),
            new Vector3(mapWidth, wallHeight, 1f));

        CreateWall("Wall_Back",
            new Vector3(0, wallHeight / 2f, -mapLength / 2f),
            new Vector3(mapWidth, wallHeight, 1f));

        CreateWall("Wall_Left",
            new Vector3(-mapWidth / 2f, wallHeight / 2f, 0),
            new Vector3(1f, wallHeight, mapLength));

        CreateWall("Wall_Right",
            new Vector3(mapWidth / 2f, wallHeight / 2f, 0),
            new Vector3(1f, wallHeight, mapLength));
    }

    private void CreateWall(string wallName, Vector3 position, Vector3 size)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.parent = transform;
        wall.transform.localPosition = position;

        BoxCollider col = wall.AddComponent<BoxCollider>();
        col.size = size;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(mapWidth, wallHeight, mapLength)
        );
    }
}