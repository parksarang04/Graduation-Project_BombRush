using Fusion;
using UnityEngine;

public class WSkill_Dash : BaseSkill
{
    [Header("W스킬 설정")]
    [SerializeField] private float dashDistance = 5f;   // 돌진 거리
    [SerializeField] private float dashDuration = 0.2f; // 돌진 시간

    // 돌진 중인지 여부 - 네트워크 동기화
    [Networked] private NetworkBool isDashing {  get; set; }
    [Networked] private TickTimer DashTimer { get; set; }
    [Networked] private Vector3 DashDirection { get; set; }

    public override void FixedUpdateNetwork()
    {
        // 돌진 중이면 매 틱마다 이동
        if (isDashing)
        {
            float speed = dashDistance / dashDuration;
            transform.position += DashDirection * speed * Runner.DeltaTime;
            
            // 돌진 시간 끝나면 멈춤
            if (DashTimer.Expired(Runner))
            {
                isDashing = false;
            }
        }
    }

    protected override void Execute()
    {
        // 현재 바라보는 방향으로 돌진
        DashDirection = transform.forward;
        isDashing = true;
        DashTimer = TickTimer.CreateFromSeconds(Runner, dashDuration);

        Debug.Log("돌진!");
    }
}
