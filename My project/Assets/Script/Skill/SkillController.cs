using Fusion;
using UnityEngine;

public class SkillController : NetworkBehaviour
{
    private BaseSkill[] skills = new BaseSkill[4];      // Q W E R

    public override void Spawned()
    {
        // 이 오브젝트에 붙은 스킬들 자동으로 가져오기
        BaseSkill[] found = GetComponents<BaseSkill>();

        for (int i = 0; i < found.Length && i < 4; i++)
        {
            skills[i] = found[i];
        }
    }

    private void Update()
    {
        // 내 캐릭터만 입력 처리
        if (Object == null || !Object.HasInputAuthority) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseSkill(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseSkill(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseSkill(3);
    }

    private void UseSkill(int index)
    {
        if (skills[index] == null)
        {
            Debug.Log($"슬롯 {index} 에 스킬 없음");
            return;
        }
        skills[index].TryUse();
    }
}
