using Fusion;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [Header("맵 크기 설정")]
    [SerializeField] private float mapLength = 70f; // 맵 길이 (앞 뒤)
    [SerializeField] private float mapWidth = 10f;  // 맵 너비 (좌우)
    [SerializeField] private float wallHeight = 5f; // 벽 높이

    // static으로 다은 스크립트에서 접근 가능
    public static float MaxLength { get; private set; }
    public static float MaxWidth { get; private set; }

    private void Start()
    {
        MaxLength = mapLength / 2f;
        MaxWidth = mapWidth / 2f;
        CreateWalls();
    }

    private void CreateWalls()
    {
        // 앞쪽 벽
        CreateWall("Wall_Front",
            new Vector3(0, wallHeight / 2f, mapLength / 2f),
            new Vector3(mapWidth, wallHeight, 1f));

        // 뒤쪽 벽
        CreateWall("Wall_Back",
            new Vector3(0, wallHeight / 2f, -mapLength / 2f),
            new Vector3(mapWidth, wallHeight, 1f));

        // 왼쪽 벽
        CreateWall("Wall_Left",
            new Vector3(-mapWidth / 2f, wallHeight / 2f, 0),
            new Vector3(1f, wallHeight, mapLength));

        // 오른쪽 벽
        CreateWall("Wall_Right",
            new Vector3(mapWidth / 2f, wallHeight / 2f, 0),
            new Vector3(1f, wallHeight, mapLength));
    }

    private void CreateWall(string wallName, Vector3 position, Vector3 size)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.parent = transform;      // MapBoundary 하위로
        wall.transform.localPosition = position;

        // 콜라이더만 추가 (눈에 안 보임)
        BoxCollider collider = wall.AddComponent<BoxCollider>();
        collider.size = size;
    }

    // 씬 뷰에서 경계 시각화
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(mapWidth, wallHeight, mapLength)
        );
    }
}
