using Fusion;
using UnityEngine;

// abstract class = 이 클래스는 직접 쓰지않고 뼈대만 제공한다.
// protected = 이 클래스 + 상속받은 자식 클래스도 사용 가능
// [Networked] = Host가 관리하고 모든 Client에 자동 동기화
// TickTimer = Fusion 전용 타이머. Unity의 Time.time 대신 네트워크 틱 기준으로 동작

public abstract class BaseSkill : NetworkBehaviour
{
    [Header("공통 설정")]
    [SerializeField] private string skillName = "스킬";
    [SerializeField] protected float cooldown = 2f;

    // 쿨타임 타이머 - 네트워크 동기화
    [Networked] private TickTimer CooldownTimer { get; set; }

    // 쿨타임 남은 시간 (UI용)
    public float CooldownRemaining =>
        CooldownTimer.ExpiredOrNotRunning(Runner) ? 0f : (float)CooldownTimer.RemainingTime(Runner);

    public bool IsReady => CooldownTimer.ExpiredOrNotRunning(Runner);

    // 스킬 사용 시도 - SkillController에서 호출
    public void TryUse()
    {
        // 쿨타임 중이면 못 씀
        if (!IsReady)
        {
            Debug.Log($"{skillName} 쿨타임 중 - 남은시간 : { CooldownRemaining:F1}초");
            return;
        }

        // Host만 실제 판정 || 왜 Host만 Execute() 하냐? => Client가 직접 데미지 계산하면 해킹 가능하기 때문
        if (Object.HasStateAuthority)
        {
            Execute();
        }

        // 쿨타임 시작 (Host + Client 둘 다)
        CooldownTimer = TickTimer.CreateFromSeconds(Runner, cooldown);

        Debug.Log($"{skillName} 사용!");
    }

    // 각 스킬에서 실제 동작 구현
    protected abstract void Execute();
}
