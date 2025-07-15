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
    #endregion

    #region Functions
    private void Start() {
        m_bIsShowing = false;
    }

    public bool IsShowing() {
        return m_bIsShowing;
    }

    public void Show(List<TargetModel> p_lTargetModel) {
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
