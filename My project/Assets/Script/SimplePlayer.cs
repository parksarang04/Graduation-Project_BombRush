using Fusion;
using UnityEngine;
 
/* Fusion 네트워크 오브젝트용 스크립트 MonoBehaviour 가 아니라 네트워크 틱 기준으로 동작 */

public class SimplePlayer : NetworkBehaviour    
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    /* Fusion 버전으 ㅣ네트워크 업데이트 함수 Unity의 업데이트 대신 쓴다. */

    public override void FixedUpdateNetwork()
    {
        if (GetInput<FusionBootstrap.NetworkInputData>(out var inputData))
        {
            Vector3 move = new Vector3(inputData.move.x, 0f, inputData.move.y);

            if (move.sqrMagnitude > 1f)
                move.Normalize();

            transform.position += move * moveSpeed * Runner.DeltaTime;

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
