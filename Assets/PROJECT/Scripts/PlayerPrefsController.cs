using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsController : MonoBehaviour {
    
    #region Singleton
    public static PlayerPrefsController Instance;
    private void Awake() {
        Instance = this;
        LoadVariables();
    }
    #endregion

    #region Variables
    private UserModel m_oUserModel;
    #endregion

    #region Functions
    private void LoadVariables() {
        string _sUserModel = PlayerPrefs.GetString("UserModel", "");
        if (string.IsNullOrEmpty(_sUserModel) == false) {
            m_oUserModel = JsonUtility.FromJson<UserModel>(_sUserModel);
        }
        else {
            m_oUserModel = new UserModel();
        }
    }

    private void Start() {

    }

    public UserModel GetUserModel() {
        return m_oUserModel;
    }

    public void Checkin() {
        m_oUserModel.checkin_count++;
        m_oUserModel.checkin_anchor = DateTime.UtcNow.ToBinary().ToString();
        PlayerPrefs.SetString("UserModel", JsonUtility.ToJson(m_oUserModel));
    }

    public void AddCoin(int p_nCoin) {
        m_oUserModel.coin += p_nCoin;
        PlayerPrefs.SetString("UserModel", JsonUtility.ToJson(m_oUserModel));

        if (LobbySceneController.Instance != null) {
            LobbySceneController.Instance.UpdateCoin();
        }
    }

    public void BuyBooster1(int p_nPrice, int p_nAmount) {
        m_oUserModel.coin -= p_nPrice;
        m_oUserModel.booster_1 += p_nAmount;
        PlayerPrefs.SetString("UserModel", JsonUtility.ToJson(m_oUserModel));

        if (LobbySceneController.Instance != null) {
            LobbySceneController.Instance.UpdateCoin();
        }
    }

    public void BuyBooster2(int p_nPrice, int p_nAmount) {
        m_oUserModel.coin -= p_nPrice;
        m_oUserModel.booster_2 += p_nAmount;
        PlayerPrefs.SetString("UserModel", JsonUtility.ToJson(m_oUserModel));

        if (LobbySceneController.Instance != null) {
            LobbySceneController.Instance.UpdateCoin();
        }
    }

    public void BuyBooster3(int p_nPrice, int p_nAmount) {
        m_oUserModel.coin -= p_nPrice;
        m_oUserModel.booster_3 += p_nAmount;
        PlayerPrefs.SetString("UserModel", JsonUtility.ToJson(m_oUserModel));

        if (LobbySceneController.Instance != null) {
            LobbySceneController.Instance.UpdateCoin();
        }
    }

    public MissionProceedModel GetMissionProceed(string p_sMissionID) {
        for (int i = 0; i < m_oUserModel.mission_proceed.Count; i++) {
            if (m_oUserModel.mission_proceed[i].mission_id.Equals(p_sMissionID) == true) {
                return m_oUserModel.mission_proceed[i];
            }
        }

        return new MissionProceedModel(p_sMissionID);
    }

    public void RewardMission(string p_sMissionID) {
        for (int i = 0; i < m_oUserModel.mission_proceed.Count; i++) {
            if (m_oUserModel.mission_proceed[i].mission_id.Equals(p_sMissionID) == true) {
                m_oUserModel.mission_proceed[i].rewarded = true;
            }
        }
        PlayerPrefs.SetString("UserModel", JsonUtility.ToJson(m_oUserModel));
    }

    public void SetMaxLevel(int p_nLevel) {
        m_oUserModel.max_level -= p_nLevel;
        PlayerPrefs.SetString("UserModel", JsonUtility.ToJson(m_oUserModel));
    }
    #endregion

}
