using UnityEngine;

public class MapData : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    public Transform[] SpawnPoints => spawnPoints;

    public Vector3 MapCenter
    {
        get
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return transform.position;

            Bounds bounds = renderers[0].bounds;
            foreach (Renderer r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }
            return bounds.center;
        }
    }
}