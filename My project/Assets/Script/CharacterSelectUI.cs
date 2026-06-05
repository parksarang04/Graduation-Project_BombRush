using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Select Buttons")]
    [SerializeField] private Button tankerButton;
    [SerializeField] private Button healerButton;
    [SerializeField] private Button dealerButton;

    [Header("Card Highlight (선택 시 테두리)")]
    [SerializeField] private Image tankerCardBorder;
    [SerializeField] private Image healerCardBorder;
    [SerializeField] private Image dealerCardBorder;

    [Header("Class Info Text")]
    [SerializeField] private TextMeshProUGUI classNameText;
    [SerializeField] private TextMeshProUGUI classDescText;

    [Header("Confirm Button")]
    [SerializeField] private Button confirmButton;

    [Header("Locked Overlay (중복 방지)")]
    [SerializeField] private GameObject tankerLockedOverlay;
    [SerializeField] private GameObject healerLockedOverlay;
    [SerializeField] private GameObject dealerLockedOverlay;

    private string selectedClass = "";

    // 클래스 설명
    private readonly string[] classNames = { "탱커", "힐러", "딜러" };
    private readonly string[] classDescs =
    {
        "팀의 방패!\n높은 체력과 방어력으로\n적의 공격을 막아냅니다.",
        "팀의 생명줄!\n아군을 치료하고\n버프를 부여합니다.",
        "팀의 창!\n강력한 스킬로\n적을 섬멸합니다."
    };

    private void Start()
    {
        tankerButton.onClick.AddListener(() => SelectClass("Tanker", 0));
        healerButton.onClick.AddListener(() => SelectClass("Healer", 1));
        dealerButton.onClick.AddListener(() => SelectClass("Dealer", 2));
        confirmButton.onClick.AddListener(OnConfirm);

        confirmButton.interactable = false;

        // 초기 테두리 숨기기
        SetAllBordersInactive();
    }

    private void SelectClass(string className, int index)
    {
        selectedClass = className;
        PlayerPrefs.SetString("SelectedClass", selectedClass);

        // 텍스트 업데이트
        classNameText.text = classNames[index];
        classDescText.text = classDescs[index];

        // 테두리 하이라이트
        SetAllBordersInactive();
        Image selectedBorder = index == 0 ? tankerCardBorder :
                               index == 1 ? healerCardBorder : dealerCardBorder;
        selectedBorder.color = new Color(1f, 0.8f, 0f, 1f); // 골드 테두리

        confirmButton.interactable = true;
    }

    private void SetAllBordersInactive()
    {
        Color off = new Color(1f, 1f, 1f, 0f);
        if (tankerCardBorder) tankerCardBorder.color = off;
        if (healerCardBorder) healerCardBorder.color = off;
        if (dealerCardBorder) dealerCardBorder.color = off;
    }

    // 다른 플레이어가 이미 선택한 클래스 잠금
    public void LockClass(string className)
    {
        if (className == "Tanker" && tankerLockedOverlay) tankerLockedOverlay.SetActive(true);
        if (className == "Healer" && healerLockedOverlay) healerLockedOverlay.SetActive(true);
        if (className == "Dealer" && dealerLockedOverlay) dealerLockedOverlay.SetActive(true);
    }

    private void OnConfirm()
    {
        if (string.IsNullOrEmpty(selectedClass)) return;

        PlayerPrefs.SetString("SelectedClass", selectedClass);
        PlayerPrefs.Save();

        // NetworkRunner로 씬 전환
        var runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            runner.LoadScene(SceneRef.FromIndex(2));
        }
        else
        {
            // NetworkRunner 없으면 일반 씬 전환
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
    }
}