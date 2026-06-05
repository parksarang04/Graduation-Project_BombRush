using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("맵")]
    [SerializeField] private GameObject[] maps;

    [Header("캐릭터 프리팹 (0:탱커, 1:힐러, 2:딜러)")]
    [SerializeField] private NetworkPrefabRef tankerPrefab;
    [SerializeField] private NetworkPrefabRef healerPrefab;
    [SerializeField] private NetworkPrefabRef dealerPrefab;

    [Header("인트로")]
    [SerializeField] private IntroCameraController introCamera;

    [Networked] private int SelectedMapIndex { get; set; }

    private MapData currentMapData;
    private Dictionary<PlayerRef, NetworkObject> playerObjects = new();

    // 플레이어별 클래스 저장 (호스트만)
    private Dictionary<PlayerRef, string> playerClassMap = new();
    private int totalPlayers = 0;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            SelectedMapIndex = Random.Range(0, maps.Length);
        }

        foreach (GameObject map in maps)
            map.SetActive(false);

        maps[SelectedMapIndex].SetActive(true);
        currentMapData = maps[SelectedMapIndex].GetComponent<MapData>();

        // 내 클래스를 호스트에게 전송
        string myClass = PlayerPrefs.GetString("SelectedClass", "Tanker");
        Debug.Log($"[GameManager] 내 선택 클래스: {myClass}");
        RPC_RegisterClass(Runner.LocalPlayer, myClass);

        StartCoroutine(StartAfterBoundaryInit());
    }

    // 클라이언트 → 호스트로 클래스 등록
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RegisterClass(PlayerRef player, string className)
    {
        playerClassMap[player] = className;
        Debug.Log($"[Host] {player} 클래스 등록: {className} ({playerClassMap.Count}/{Runner.ActivePlayers.Count()})");
    }

    private IEnumerator StartAfterBoundaryInit()
    {
        yield return null;

        Vector3 introCenter = new Vector3(
            MapBoundary.Center.x,
            MapBoundary.Center.y,
            MapBoundary.Center.z
        );

        float mapSize = MapBoundary.MaxLength * 2f;
        introCamera.StartIntro(introCenter, mapSize);

        StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitUntil(() => introCamera.IntroFinished);

        if (Object.HasStateAuthority)
        {
            // 모든 플레이어 클래스 등록 대기 (최대 5초)
            float timeout = 5f;
            while (playerClassMap.Count < Runner.ActivePlayers.Count() && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

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

            Vector3 spawnPos = GetGroundPosition(spawnPoint.position);

            // 클래스에 맞는 프리팹 선택
            string className = playerClassMap.ContainsKey(player)
                ? playerClassMap[player]
                : "Tanker";

            NetworkPrefabRef prefab = GetPrefabByClass(className);

            Debug.Log($"[Spawn] {player} → {className} 프리팹으로 스폰");

            NetworkObject obj = Runner.Spawn(
                prefab,
                spawnPos,
                Quaternion.identity,
                player
            );

            playerObjects[player] = obj;
            Runner.SetPlayerObject(player, obj);
            index++;
        }
    }

    private NetworkPrefabRef GetPrefabByClass(string className)
    {
        return className switch
        {
            "Tanker" => tankerPrefab,
            "Healer" => healerPrefab,
            "Dealer" => dealerPrefab,
            _ => tankerPrefab
        };
    }

    private Vector3 GetGroundPosition(Vector3 point)
    {
        if (Physics.Raycast(point + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f))
        {
            return hit.point + Vector3.up * 0.5f;
        }
        return point;
    }
}