using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupFinishLevel : MonoBehaviour {

    #region Singleton
    public static PopupFinishLevel Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Animator s_oPopupAnimator;
    [SerializeField]
    private TMP_Text s_uiLabelTargetAmount;
    #endregion

    #region Variables
    private bool m_bIsShowing;
    #endregion

    #region Functions
    private void Start() {
        m_bIsShowing = false;
    }

    public bool IsShowing() {
        return m_bIsShowing;
    }

    public void Show(int p_nTarget) {
        s_uiLabelTargetAmount.text = p_nTarget.ToString();
        s_oPopupAnimator.gameObject.SetActive(true);
        s_oPopupAnimator.SetBool("bIsShowing", true);
        m_bIsShowing = true;
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonContinue() {
        PopupWinLevel.Instance.Show(3);
    }
    #endregion

}
