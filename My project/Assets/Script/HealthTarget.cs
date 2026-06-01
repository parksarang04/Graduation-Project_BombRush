using Fusion;
using UnityEngine;

public class HealthTarget : NetworkBehaviour
{
    [SerializeField] private int maxHp = 100;

    // [Networked] : Host가 관리하고 모든 Client에 자동 동기화
    [Networked] public int Hp { get; private set; }

    public bool IsDead => Hp <= 0;

    public override void Spawned()
    {
        // Host만 초기화 설정
        if (Object.HasStateAuthority)
        {
            Hp = maxHp;
        }
    }

    // 데미지 받기 - Host만 실행
    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority) return;
        if (IsDead) return;

        Hp -= damage;
        Hp = Mathf.Max(Hp, 0);    // 0 아래로 안 내려가게

        Debug.Log($"{name} 피격 - 남은 HP : {Hp}");

        if (Hp <= 0)
        {
            OnDead();
        }
    }

    // 회복 - 힐러용 (나중에 사용)
    public void Heal(int amount)
    {
        if (!Object.HasStateAuthority) return;
        if (IsDead) return;

        Hp += amount;
        Hp = Mathf.Min(Hp, maxHp);    // maxHp 초과 안 되게
    }

    private void OnDead()
    {
        Debug.Log($"{name} 사망 - 리스폰");

        // 리스폰 (2초후)
        // 나중에 RPC로 연출 추가 가능
        Hp = maxHp;
        transform.position = Vector3.zero;  //임시 리스폰 위치
    }
}
