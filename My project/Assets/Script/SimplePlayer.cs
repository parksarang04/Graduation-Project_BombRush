using Fusion;
using UnityEngine;
 
/* Fusion 네트워크 오브젝트용 스크립트 MonoBehaviour 가 아니라 네트워크 틱 기준으로 동작 */

public class SimplePlayer : NetworkBehaviour    
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    // 슬로우 관련 추가
    [Networked] private float SlowMultiplier {  get; set; } // 1 = 정상, 0.5 = 절반속도
    [Networked] private TickTimer SlowTimer { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            SlowMultiplier = 1f;    // 기본값 정상속도
        }
    }

    // 슬로우 적용 - ESkill에서 호출
    public void ApplySlow(float multplier, float duration)
    {
        if (!Object.HasStateAuthority) return;

        SlowMultiplier = multplier;
        SlowTimer = TickTimer.CreateFromSeconds(Runner, duration);
    }

    /* Fusion 버전의 네트워크 업데이트 함수 Unity의 업데이트 대신 쓴다. */
    public override void FixedUpdateNetwork()
    {
        // 슬로우 시간 끝나면 정상속도로 복귀
        if (SlowMultiplier < 1f && SlowTimer.Expired(Runner))
        {
            SlowMultiplier = 1f;
            Debug.Log("슬로우 해제");
        }

        if (GetInput<NetworkInputData>(out var inputData))
        {
            Vector3 move = new Vector3(inputData.move.x, 0f, inputData.move.y);

            if (move.sqrMagnitude > 1f)
                move.Normalize();

            // 슬로우 적용해서 이동
            transform.position += move * moveSpeed * SlowMultiplier * Runner.DeltaTime;

            // 맵 경계 클램프 추가
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x,
                MapBoundary.Center.x - MapBoundary.MaxWidth,
                MapBoundary.Center.x + MapBoundary.MaxWidth);

            pos.z = Mathf.Clamp(pos.z,
                MapBoundary.Center.z - MapBoundary.MaxLength,
                MapBoundary.Center.z + MapBoundary.MaxLength);

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Runner.DeltaTime
                );
            }
        }
    }
}
