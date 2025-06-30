using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWinLevel : MonoBehaviour {

    #region Singleton
    public static PopupWinLevel Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Animator s_oPopupAnimator;
    [SerializeField]
    private List<GameObject> s_lWinStar;
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

    public void Show(int p_nStar) {
        s_lWinStar[p_nStar - 1].SetActive(true);
        s_oPopupAnimator.gameObject.SetActive(true);
        s_oPopupAnimator.SetBool("bIsShowing", true);
        m_bIsShowing = true;
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonContinue() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
    #endregion

}
