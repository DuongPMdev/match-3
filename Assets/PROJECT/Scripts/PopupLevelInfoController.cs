using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupLevelInfoController : MonoBehaviour {

    #region Singleton
    public static PopupLevelInfoController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Animator s_oPopupAnimator;
    [SerializeField]
    private TMP_Text s_uiLabelLevel;
    [SerializeField]
    private GameObject s_goButtonPlay;
    [SerializeField]
    private GameObject s_goButtonCannotPlay;
    #endregion

    #region Variables
    private int m_nLevel;
    #endregion

    #region Functions
    public void Show(int p_nLevel) {
        m_nLevel = p_nLevel;
        LoadUIs();
        s_oPopupAnimator.gameObject.SetActive(true);
        s_oPopupAnimator.SetBool("bIsShowing", true);
    }
    public void Hide() {
        s_oPopupAnimator.SetBool("bIsShowing", false);
    }

    private void LoadUIs() {
        s_uiLabelLevel.text = "LEVEL " + m_nLevel.ToString();

        int _nUnlockedLevel = PlayerPrefs.GetInt("unlocked_level", 1);
        if (m_nLevel > _nUnlockedLevel) {
            s_goButtonPlay.SetActive(false);
            s_goButtonCannotPlay.SetActive(true);
        }
        else {
            s_goButtonPlay.SetActive(true);
            s_goButtonCannotPlay.SetActive(false);
        }
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonPlay() {
        PlayerPrefs.SetInt("level", m_nLevel);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    #endregion

}
