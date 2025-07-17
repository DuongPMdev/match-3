using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupBananaSpinController : MonoBehaviour {

    #region Singleton
    public static PopupBananaSpinController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Animator s_oPopupAnimator;
    [SerializeField]
    private List<GameObject> s_lIconReward;
    [SerializeField]
    private List<GameObject> s_lLabelReward;
    #endregion

    #region Variables
    #endregion

    #region Functions
    private void Start() {

    }

    public void Show(int p_nReward) {
        int _nReward = p_nReward - 1;
        for (int i = 0; i < s_lIconReward.Count; i++) {
            s_lIconReward[i].SetActive(i == _nReward);
            s_lLabelReward[i].SetActive(i == _nReward);
        }
        s_oPopupAnimator.gameObject.SetActive(true);
        s_oPopupAnimator.SetBool("bIsShowing", true);
    }

    public void Hide() {
        s_oPopupAnimator.SetBool("bIsShowing", false);
    }
    #endregion

}
