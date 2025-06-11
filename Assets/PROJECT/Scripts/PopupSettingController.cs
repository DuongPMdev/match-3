using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupSettingController : MonoBehaviour {

    #region Singleton
    public static PopupSettingController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Animator s_oPopupAnimator;
    [SerializeField]
    private CustomToggle s_oToggleMusic;
    [SerializeField]
    private CustomToggle s_oToggleSound;
    #endregion

    #region Variables
    private int m_nLevel;
    #endregion

    #region Functions
    private void Start() {
        s_oToggleMusic.SetOnTurn(OnToggleMusic);
        s_oToggleSound.SetOnTurn(OnToggleSound);
    }

    public void Show() {
        s_oPopupAnimator.gameObject.SetActive(true);
        s_oPopupAnimator.SetBool("bIsShowing", true);
        LoadSettingManager();
    }

    private void LoadSettingManager() {
        s_oToggleMusic.SetValue(SettingsManager.Instance.IsMusicOn());
        s_oToggleSound.SetValue(SettingsManager.Instance.IsSoundOn());
    }

    public void Hide() {
        s_oPopupAnimator.SetBool("bIsShowing", false);
    }

    private void OnToggleMusic() {
        SettingsManager.Instance.TurnMusic();
        LoadSettingManager();
    }

    private void OnToggleSound() {
        SettingsManager.Instance.TurnSound();
        LoadSettingManager();
    }
    #endregion

}
