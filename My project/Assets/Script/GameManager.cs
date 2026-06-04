using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("맵")]
    [SerializeField] private GameObject[] maps;

    [Header("캐릭터 프리팹")]
    [SerializeField] private NetworkPrefabRef[] characterPrefabs;

    [Header("인트로")]
    [SerializeField] private IntroCameraController introCamera;

    [Networked] private int SelectedMapIndex { get; set; }

    private MapData currentMapData;
    private Dictionary<PlayerRef, NetworkObject> playerObjects = new();

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            SelectedMapIndex = Random.Range(0, maps.Length);
        }

        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }

        maps[SelectedMapIndex].SetActive(true);

        currentMapData = maps[SelectedMapIndex].GetComponent<MapData>();

        StartCoroutine(StartAfterBoundaryInit());
    }

    private IEnumerator StartAfterBoundaryInit()
    {
        yield return null;

        // MapData.MapCenter 대신 MapBoundary.Center 사용
        Vector3 introCenter = new Vector3(
            MapBoundary.Center.x,
            MapBoundary.Center.y,
            MapBoundary.Center.z
        );

        Debug.Log($"인트로 중앙 : {introCenter}");

        float mapSize = MapBoundary.MaxLength * 2f;
        introCamera.StartIntro(introCenter, mapSize);

        StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitUntil(() => introCamera.IntroFinished);

        if (Object.HasStateAuthority)
        {
            SpawnPlayers();
        }
    }

    private void SpawnPlayers()
    {
        int index = 0;

        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            Transform spawnPoint = currentMapData.SpawnPoints[
                index % currentMapData.SpawnPoints.Length
            ];

            // 스폰포인트 아래로 Raycast → 실제 땅 위치 찾기
            Vector3 spawnPos = GetGroundPosition(spawnPoint.position);

            Debug.Log($"최종 스폰 위치 : {spawnPos}");

            NetworkObject obj = Runner.Spawn(
                characterPrefabs[0],
                spawnPos,
                Quaternion.identity,
                player
            );

            playerObjects[player] = obj;
            index++;
        }
    }
    private Vector3 GetGroundPosition(Vector3 point)
    {
        // 위로 10 올린 다음 아래로 Raycast
        if (Physics.Raycast(point + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f))
        {
            return hit.point + Vector3.up * 0.5f;   // 땅에서 0.5 위에 스폰
        }
        return point;
    }
}