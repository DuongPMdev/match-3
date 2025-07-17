using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbySceneController : MonoBehaviour {

    #region Singleton
    public static LobbySceneController Instance;
    private void Awake() {
        Instance = this;
        Application.targetFrameRate = 60;
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private TMP_Text s_uiLabelDisplayName;
    [SerializeField]
    private TMP_Text s_uiLabelHeart;
    [SerializeField]
    private TMP_Text s_uiLabelCoin;
    [SerializeField]
    private GameObject s_goPopupBuySuccessed;
    [SerializeField]
    private GameObject s_goPopupNotEnoughCoin;
    #endregion

    #region Functions
    private void Start() {
        SettingsManager.Instance.PlayMusic(SoundController.Instance.GetMusic());
        LoadDataToUI();
    }

    private void LoadDataToUI() {
        if (APIController.Instance != null) {
            TelegramUser _oTelegramUser = APIController.Instance.GetTelegramUser();
            if (_oTelegramUser != null) {
                if (s_uiLabelDisplayName != null) {
                    s_uiLabelDisplayName.text = _oTelegramUser.first_name + " " + _oTelegramUser.last_name;
                }
                if (s_uiLabelHeart != null) {
                    s_uiLabelHeart.text = _oTelegramUser.lives.ToString();
                }
                if (s_uiLabelCoin != null) {
                    s_uiLabelCoin.text = _oTelegramUser.coins.ToString();
                }
            }
        }
        else {
            s_uiLabelHeart.text = PlayerPrefsController.Instance.GetUserModel().heart.ToString();
            s_uiLabelCoin.text = PlayerPrefsController.Instance.GetUserModel().coin.ToString();
        }
    }

    public void HideAllPopup() {
        PopupSettingController.Instance.Hide();
        PopupProfileController.Instance.Hide();
        PopupHeartController.Instance.Hide();
        PopupLevelInfoController.Instance.Hide();
        PopupBananaSpinController.Instance.Hide();
    }

    public void UpdateCoin() {
        s_uiLabelCoin.text = PlayerPrefsController.Instance.GetUserModel().coin.ToString();
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

    public void OnClickButtonBuyBooster1() {
        if (PlayerPrefsController.Instance.GetUserModel().coin < 100) {
            s_goPopupNotEnoughCoin.SetActive(true);
        }
        else {
            PlayerPrefsController.Instance.BuyBooster1(100, 3);
            s_goPopupBuySuccessed.SetActive(true);
        }
    }

    public void OnClickButtonBuyBooster2() {
        if (PlayerPrefsController.Instance.GetUserModel().coin < 150) {
            s_goPopupNotEnoughCoin.SetActive(true);
        }
        else {
            PlayerPrefsController.Instance.BuyBooster1(150, 3);
            s_goPopupBuySuccessed.SetActive(true);
        }
    }

    public void OnClickButtonBuyBooster3() {
        if (PlayerPrefsController.Instance.GetUserModel().coin < 200) {
            s_goPopupNotEnoughCoin.SetActive(true);
        }
        else {
            PlayerPrefsController.Instance.BuyBooster1(200, 3);
            s_goPopupBuySuccessed.SetActive(true);
        }
    }
    #endregion

}
