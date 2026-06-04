using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button buttonTank;
    [SerializeField] private Button buttonHealer;
    [SerializeField] private Button buttonDealer;
    [SerializeField] private Button buttonReady;
    [SerializeField] private Button buttonStart;

    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI textMySelect;
    [SerializeField] private TextMeshProUGUI textStatus;

    private string selectedClass = "";
    private bool isReady = false;

    private void Start()
    {
        // Start 버튼은 Host만 보임
        if (FusionBootstrap.Instance != null)
        {
            buttonStart.gameObject.SetActive(
                FusionBootstrap.Instance.Runner.IsServer
            );
        }

        buttonStart.interactable = false;

        buttonTank.onClick.AddListener(() => SelectClass("Tank"));
        buttonHealer.onClick.AddListener(() => SelectClass("Healer"));
        buttonDealer.onClick.AddListener(() => SelectClass("Dealer"));
        buttonReady.onClick.AddListener(OnClickReady);
        buttonStart.onClick.AddListener(OnClickStart);
    }

    private void SelectClass(string className)
    {
        selectedClass = className;
        textMySelect.text = $"선택 : {className}";
        Debug.Log($"클래스 선택 : {className}");
    }

    private void OnClickReady()
    {
        if (selectedClass == "")
        {
            textStatus.text = "캐릭터를 먼저 선택하세요!";
            return;
        }

        isReady = true;
        buttonReady.interactable = false;
        textStatus.text = "준비 완료!";

        // 선택 정보 저장
        PlayerPrefs.SetString("SelectedClass", selectedClass);

        // Host라면 Start 버튼 활성화
        if (FusionBootstrap.Instance.Runner.IsServer)
        {
            buttonStart.interactable = true;
        }

        Debug.Log($"준비 완료 - 선택 클래스 : {selectedClass}");
    }

    private void OnClickStart()
    {
        if (!isReady)
        {
            textStatus.text = "먼저 준비 완료를 눌러주세요!";
            return;
        }

        FusionBootstrap.Instance.LoadGameScene();
    }
}