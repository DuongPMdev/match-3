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
    #endregion

    #region Variables
    private bool m_bIsShowing;
    private int m_nNumberStar;
    #endregion

    #region Functions
    private void Start() {
        m_bIsShowing = false;
        m_nNumberStar = 0;
    }

    public bool IsShowing() {
        return m_bIsShowing;
    }

    public void Show(List<TargetModel> p_lTargetModel, int p_nNumberStar) {
        m_nNumberStar = p_nNumberStar;
        s_oPopupAnimator.gameObject.SetActive(true);
        s_oPopupAnimator.SetBool("bIsShowing", true);
        m_bIsShowing = true;
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonContinue() {
        PopupWinLevel.Instance.Show(m_nNumberStar);
    }
    #endregion

}
