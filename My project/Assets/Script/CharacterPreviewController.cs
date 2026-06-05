using UnityEngine;

public class CharacterPreviewController : MonoBehaviour
{
    [Header("Preview Characters")]
    [SerializeField] private Transform tankerPreview;
    [SerializeField] private Transform healerPreview;
    [SerializeField] private Transform dealerPreview;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 30f;

    void Update()
    {
        // 캐릭터 천천히 회전
        if (tankerPreview) tankerPreview.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        if (healerPreview) healerPreview.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        if (dealerPreview) dealerPreview.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
