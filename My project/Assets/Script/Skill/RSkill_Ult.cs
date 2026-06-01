using Fusion;
using UnityEngine;

public class RSkill_Ult : BaseSkill
{
    [Header("R스킬 설정")]
    [SerializeField] private float radius = 6f;
    [SerializeField] private int damage = 60;
    [SerializeField] private float slowMiltiplier = 0.2f;       // 20% 속도
    [SerializeField] private float slowDuration = 3f;
    [SerializeField] private LayerMask targetMask;

    protected override void Execute()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius,
            targetMask
        );

        int hitCount = 0;

        foreach (Collider hit in hits)
        {
            HealthTarget healthTarget = hit.GetComponentInParent<HealthTarget>();
            SimplePlayer player = hit.GetComponentInParent<SimplePlayer>();

            if (healthTarget == null || player == null) continue;
            if (healthTarget.Object == Object) continue;        //자기자신 제외

            //  데미지 + 슬로우 동시 적용
            healthTarget.TakeDamage( damage );
            player.ApplySlow(slowMiltiplier, slowDuration);

            hitCount++;
            Debug.Log($"{healthTarget.name} 궁극기 피격 - {damage} 데미지 + 슬로우!");
        }

        Debug.Log($"궁극기 발동! 총 {hitCount} 명 피격");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
