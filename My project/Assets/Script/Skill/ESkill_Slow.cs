using Fusion;
using UnityEngine;

public class ESkill_Slow : BaseSkill
{
    [Header("E스킬 설정")]
    [SerializeField] private float radius = 4f;
    [SerializeField] private float slowMultiplier = 0.3f;       // 0.3 = 30% 속도
    [SerializeField] private float slowDuration = 2f;           // 슬로우 지속시간
    [SerializeField] private LayerMask targetMask;

    protected override void Execute()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius,
            targetMask
        );
        
        foreach (Collider hit in hits)
        {
            SimplePlayer target = hit.GetComponentInParent<SimplePlayer>();

            if (target == null) continue;
            if (target.Object == Object) continue;  // 자기 자신 제외

            target.ApplySlow(slowMultiplier, slowDuration);
            Debug.Log($"{target.name} 슬로우 적용!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
