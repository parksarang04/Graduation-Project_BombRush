using Fusion;
using UnityEditor;
using UnityEngine;

public class MapManager : NetworkBehaviour
{
    [Header("맵 목록")]
    [SerializeField] private GameObject[] maps;     // Demo1 ~ Demo5 드래그

    [Networked] private int SelectedMapIndex {  get; set; }

    public override void Spawned()
    {
        // 일단 모든 맵 비활성화
        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }

        // Host만 랜덤 선택
        if (Object.HasStateAuthority)
        {
            SelectedMapIndex = Random.Range(0, maps.Length);
            Debug.Log($"선택된 맵 : {maps[SelectedMapIndex].name}");
        }

        // 선택된 맵만 활성화
        maps[SelectedMapIndex].SetActive(true);
    }
    
}
