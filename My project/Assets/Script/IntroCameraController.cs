using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroCameraController : MonoBehaviour
{
    [Header("인트로 설정")]
    [SerializeField] private float introDuration = 5f;      // 3 → 5초로 늘림
    [SerializeField] private float introHeight = 60f;       // 25 → 60으로 높임
    [SerializeField] private Image fadeImage;

    private CameraController cameraController;
    private bool introFinished = false;

    public bool IntroFinished => introFinished;

    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
        if (cameraController != null)
            cameraController.enabled = false;
    }

    public void StartIntro(Vector3 mapCenter, float mapSize = 30f)
    {
        StartCoroutine(IntroRoutine(mapCenter, mapSize));
    }

    private IEnumerator IntroRoutine(Vector3 mapCenter, float mapSize)
    {
        // 1. 카메라 위치 먼저 세팅 (검은화면 상태에서)
        float panRange = Mathf.Clamp(mapSize * 0.2f, 15f, 40f);  // 범위 제한

        Vector3 startPos = new Vector3(mapCenter.x, mapCenter.y + introHeight, mapCenter.z - panRange);
        Vector3 endPos = new Vector3(mapCenter.x, mapCenter.y + introHeight, mapCenter.z + panRange);

        transform.position = startPos;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Debug.Log($"인트로 카메라 시작위치 : {startPos}");
        Debug.Log($"인트로 카메라 끝위치 : {endPos}");
        Debug.Log($"panRange : {panRange}");

        // 2. 페이드인 (검은화면 → 맵 보임)
        yield return StartCoroutine(Fade(1f, 0f, 1.5f));   // 1.5초로 늘림

        // 3. 맵 훑기
        float elapsed = 0f;
        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / introDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 4. 페이드아웃 (맵 → 검은화면)
        yield return StartCoroutine(Fade(0f, 1f, 1f));     // 1초로 늘림

        // 5. CameraController 활성화
        if (cameraController != null)
            cameraController.enabled = true;

        introFinished = true;

        // 6. 페이드인 (검은화면 → 게임 시작)
        yield return StartCoroutine(Fade(1f, 0f, 1f));     // 1초로 늘림
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeImage == null)
        {
            Debug.LogError("FadeImage가 연결되지 않았습니다!");
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, to);
    }
}