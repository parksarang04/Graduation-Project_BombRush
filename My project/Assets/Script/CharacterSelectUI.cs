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

    private readonly string[] classNames = { "탱커", "힐러", "딜러" };
    private readonly string[] classDescs =
    {
        "팀의 방패!\n높은 체력과 방어력으로\n적의 공격을 막아냅니다.",
        "팀의 생명줄!\n아군을 치료하고\n버프를 부여합니다.",
        "팀의 창!\n강력한 스킬로\n적을 섬멸합니다."
    };

    private void Awake()
    {
        Debug.Log("[CharacterSelectUI] Awake 호출됨!");

        // null 체크
        if (tankerButton == null) Debug.LogError("tankerButton 연결 안됨!");
        if (healerButton == null) Debug.LogError("healerButton 연결 안됨!");
        if (dealerButton == null) Debug.LogError("dealerButton 연결 안됨!");
        if (confirmButton == null) Debug.LogError("confirmButton 연결 안됨!");
    }

    private void Start()
    {
        Debug.Log("[CharacterSelectUI] Start 호출됨!");

        tankerButton.onClick.AddListener(OnTankerSelected);
        healerButton.onClick.AddListener(OnHealerSelected);
        dealerButton.onClick.AddListener(OnDealerSelected);
        confirmButton.onClick.AddListener(OnConfirm);

        confirmButton.interactable = false;
        SetAllBordersInactive();
    }

    private void SelectClass(string className, int index)
    {
        Debug.Log($"[CharacterSelectUI] SelectClass 호출됨: {className}");
        selectedClass = className;
        PlayerPrefs.SetString("SelectedClass", selectedClass);

        if (classNameText != null) classNameText.text = classNames[index];
        if (classDescText != null) classDescText.text = classDescs[index];

        SetAllBordersInactive();
        Image selectedBorder = index == 0 ? tankerCardBorder :
                               index == 1 ? healerCardBorder : dealerCardBorder;
        if (selectedBorder != null)
            selectedBorder.color = new Color(1f, 0.8f, 0f, 1f);

        confirmButton.interactable = true;
    }

    public void OnTankerSelected()
    {
        Debug.Log("[CharacterSelectUI] 탱커 버튼 클릭됨!");
        SelectClass("Tanker", 0);
    }

    public void OnHealerSelected()
    {
        Debug.Log("[CharacterSelectUI] 힐러 버튼 클릭됨!");
        SelectClass("Healer", 1);
    }

    public void OnDealerSelected()
    {
        Debug.Log("[CharacterSelectUI] 딜러 버튼 클릭됨!");
        SelectClass("Dealer", 2);
    }

    private void SetAllBordersInactive()
    {
        Color off = new Color(1f, 1f, 1f, 0f);
        if (tankerCardBorder) tankerCardBorder.color = off;
        if (healerCardBorder) healerCardBorder.color = off;
        if (dealerCardBorder) dealerCardBorder.color = off;
    }

    public void LockClass(string className)
    {
        if (className == "Tanker" && tankerLockedOverlay) tankerLockedOverlay.SetActive(true);
        if (className == "Healer" && healerLockedOverlay) healerLockedOverlay.SetActive(true);
        if (className == "Dealer" && dealerLockedOverlay) dealerLockedOverlay.SetActive(true);
    }

    public void OnConfirm()
    {
        Debug.Log("[CharacterSelectUI] 확인 버튼 클릭됨!");
        if (string.IsNullOrEmpty(selectedClass))
        {
            Debug.LogWarning("선택된 클래스 없음!");
            return;
        }

        PlayerPrefs.SetString("SelectedClass", selectedClass);
        PlayerPrefs.Save();

        var runner = FindFirstObjectByType<NetworkRunner>();
        if (runner != null)
        {
            Debug.Log("NetworkRunner로 씬 전환!");
            runner.LoadScene(SceneRef.FromIndex(2));
        }
        else
        {
            Debug.Log("일반 씬 전환!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
    }
}