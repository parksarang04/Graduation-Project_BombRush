using Fusion;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("카메라 설정")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 15f, -10f);   // 카메라 위치 오프셋
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float lookDownAngle = 50f;                     // 내려다보는 각도

    private Transform target;       // 따라갈 플레이어

    private void LateUpdate()
    {
        // 타겟 없으면 매 프래임 내 플레이어 찾기
        if (target == null)
        {
            FindLocalPlayer();
            return;
        }

        // 타겟 따라가기 (부드럽게)
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // 비스듬히 아래를 바라봄
        transform.rotation = Quaternion.Euler(lookDownAngle, 0f, 0f);
    }

    private void FindLocalPlayer()
    {
        // 씬에 있는 모든 NetworkObject 중 내 것 찾기
        NetworkObject[] networkObjects = FindObjectsByType<NetworkObject>(FindObjectsSortMode.None);

        foreach (NetworkObject obj in networkObjects)
        {
            if (obj.HasInputAuthority)
            {
                target = obj.transform;
                Debug.Log("카메라 타겟 설정 완료");
                return;
            }
        }
    }
}
