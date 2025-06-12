using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHeartController : MonoBehaviour {

    #region Singleton
    public static PopupHeartController Instance;
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
    #endregion

    #region Functions
    private void Start() {

    }

    public void Show() {
        s_oPopupAnimator.gameObject.SetActive(true);
        s_oPopupAnimator.SetBool("bIsShowing", true);
    }

    public void Hide() {
        s_oPopupAnimator.SetBool("bIsShowing", false);
    }
    #endregion

}
