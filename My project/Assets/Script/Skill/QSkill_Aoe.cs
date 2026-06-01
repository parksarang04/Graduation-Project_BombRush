using Fusion;
using System.Runtime.CompilerServices;
using UnityEngine;

public class QSkill_Aoe : BaseSkill
{
    [Header("Q스킬 설정")]
    [SerializeField] private float radius = 3f;         // 범위
    [SerializeField] private int damage = 30;           // 데미지
    [SerializeField] private LayerMask targetmask;      // 적 레이어

    protected override void Execute()
    {
        // 내 위치 기준 범위 안 모든 콜라이더 탐색
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius,
            targetmask
        );

        foreach (Collider hit in hits)
        {
            HealthTarget target = hit.GetComponentInParent<HealthTarget>();

            if (target == null) continue;       
            
            // 자기 자신 제외
            if (target.Object == Object) continue;

            target.TakeDamage(damage);
            Debug.Log($"{target.name} 에게 {damage} 데미지!");
        }
    }

    // 씬 뷰에서 범위 시각화 (초록색 원)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere( transform.position, radius);
    }
}
