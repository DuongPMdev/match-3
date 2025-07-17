using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public static class MissionTypes {

    public const string COLLECT_BANANA = "COLLECT_BANANA";
    public const string DESTROY_WOODBOX = "DESTROY_WOODBOX";
    public const string COLLECT_GOLD_TEETH = "COLLECT_GOLD_TEETH";

}

public class MissionController : MonoBehaviour {

    #region Singleton
    public static MissionController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Prefabs
    [SerializeField]
    private GameObject s_goPrefabMissionInProgress;
    [SerializeField]
    private GameObject s_goPrefabMissionCompleted;
    #endregion

    #region Views
    [SerializeField]
    private GameObject s_goNoMission;
    [SerializeField]
    private Transform s_tfMissionContainer;
    #endregion

    #region Variables
    private List<MissionConfigModel> m_lMissionConfig;
    #endregion

    #region Functions
    private void Start() {
        LoadConfig();
        LoadUI();
    }

    private void LoadConfig() {
        m_lMissionConfig = new List<MissionConfigModel>();

        MissionConfigModel _oMissionConfigModel1 = new MissionConfigModel();
        _oMissionConfigModel1.mission_id = MissionTypes.COLLECT_BANANA;
        _oMissionConfigModel1.icon = "banana";
        _oMissionConfigModel1.description = "Collect 300 gold bananas";
        _oMissionConfigModel1.request = 300;
        _oMissionConfigModel1.coin_reward = 1000;
        m_lMissionConfig.Add(_oMissionConfigModel1);

        MissionConfigModel _oMissionConfigModel2 = new MissionConfigModel();
        _oMissionConfigModel2.mission_id = MissionTypes.DESTROY_WOODBOX;
        _oMissionConfigModel2.icon = "woodbox";
        _oMissionConfigModel2.description = "Destroy 50 crates";
        _oMissionConfigModel2.request = 50;
        _oMissionConfigModel2.coin_reward = 1000;
        m_lMissionConfig.Add(_oMissionConfigModel2);

        MissionConfigModel _oMissionConfigModel3 = new MissionConfigModel();
        _oMissionConfigModel3.mission_id = MissionTypes.COLLECT_GOLD_TEETH;
        _oMissionConfigModel3.icon = "banana";
        _oMissionConfigModel3.description = "Collect all 5 gold teeth";
        _oMissionConfigModel3.request = 5;
        _oMissionConfigModel3.coin_reward = 1000;
        m_lMissionConfig.Add(_oMissionConfigModel3);
    }

    private void LoadUI() {
        ClearChild(s_tfMissionContainer);
        for (int i = 0; i < m_lMissionConfig.Count; i++) {
            MissionConfigModel _oMissionConfigModel = m_lMissionConfig[i];
            MissionProceedModel _oMissionProceedModel = PlayerPrefsController.Instance.GetMissionProceed(_oMissionConfigModel.mission_id);

            if (_oMissionProceedModel.rewarded == false) {
                if (_oMissionProceedModel.proceeded < _oMissionConfigModel.request) {
                    GameObject _goMissionInProgress = Instantiate(s_goPrefabMissionInProgress, s_tfMissionContainer.position, Quaternion.identity, s_tfMissionContainer);
                    _goMissionInProgress.transform.Find("Icon").Find(_oMissionConfigModel.mission_id).gameObject.SetActive(true);
                    _goMissionInProgress.transform.Find("LabelDescription").GetComponent<TMP_Text>().text = _oMissionConfigModel.description;

                    _goMissionInProgress.transform.Find("ProgressBar").Find("ProgressFill").GetComponent<Image>().fillAmount = _oMissionProceedModel.proceeded * 1.0f / _oMissionConfigModel.request;
                    _goMissionInProgress.transform.Find("ProgressBar").Find("LabelProgress").GetComponent<TMP_Text>().text = _oMissionProceedModel.proceeded + "/" + _oMissionConfigModel.request;
                }
                else {
                    GameObject _goMissionCompleted = Instantiate(s_goPrefabMissionCompleted, s_tfMissionContainer.position, Quaternion.identity, s_tfMissionContainer);
                    _goMissionCompleted.transform.Find("Icon").Find(_oMissionConfigModel.mission_id).gameObject.SetActive(true);
                    _goMissionCompleted.transform.Find("LabelDescription").GetComponent<TMP_Text>().text = _oMissionConfigModel.description;
                    _goMissionCompleted.transform.Find("ButtonReward").GetComponent<Button>().onClick.AddListener(() => {
                        _goMissionCompleted.transform.Find("Rewarded").gameObject.SetActive(true);
                        OnClickButtonReward(_oMissionConfigModel.mission_id);
                    });
                }
            }
        }
        s_goNoMission.SetActive(s_tfMissionContainer.childCount == 0);
    }

    private void OnClickButtonReward(string p_sMissionID) {
        for (int i = 0; i < m_lMissionConfig.Count; i++) {
            MissionConfigModel _oMissionConfigModel = m_lMissionConfig[i];
            if (_oMissionConfigModel.mission_id.Equals(p_sMissionID) == true) {
                PlayerPrefsController.Instance.AddCoin(_oMissionConfigModel.coin_reward);
            }
        }
        PlayerPrefsController.Instance.RewardMission(p_sMissionID);
    }

    private void ClearChild(Transform p_tfParent) {
        foreach (Transform _tfChild in p_tfParent) {
            Destroy(_tfChild.gameObject);
        }
    }
    #endregion

}
