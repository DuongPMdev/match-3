using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneController : MonoBehaviour {

    #region Singleton
    public static LobbySceneController Instance;
    private void Awake() {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    public void HideAllPopup() {
        PopupSettingController.Instance.Hide();
        PopupProfileController.Instance.Hide();
        PopupHeartController.Instance.Hide();
        PopupLevelInfoController.Instance.Hide();
        PopupBananaSpinController.Instance.Hide();
    }
    #endregion

    #region Functions
    private void Start() {
        SettingsManager.Instance.PlayMusic(SoundController.Instance.GetMusic());
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonSetting() {
        PopupSettingController.Instance.Show();
    }

    public void OnClickButtonProfile() {
        PopupProfileController.Instance.Show();
    }

    public void OnClickButtonHeart() {
        PopupHeartController.Instance.Show();
    }

    public void OnClickButtonPlayLevel(int p_nLevel) {
        PopupLevelInfoController.Instance.Show(p_nLevel);
    }

    public void OnClickButtonDailyReward() {
        PopupDailyRewardController.Instance.Show();
    }
    #endregion

}
