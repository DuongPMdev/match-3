using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupIngameLoadingController : MonoBehaviour {

    #region Singleton
    public static PopupIngameLoadingController Instance;
    private void Awake() {
        Instance = this;
        LoadVariables();
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private CanvasGroup s_goCanvasGroup;
    #endregion

    #region Variables
    private bool m_bIsShowing;
    #endregion

    #region Functions
    private void LoadVariables() {
        m_bIsShowing = true;
    }

    private void Start() {
        StartCoroutine(HideIE());
    }

    public bool IsShowing() {
        return m_bIsShowing;
    }

    private IEnumerator HideIE() {
        s_goCanvasGroup.alpha = 1.0f;
        s_goCanvasGroup.interactable = true;
        s_goCanvasGroup.blocksRaycasts = true;
        yield return new WaitForSeconds(1.0f);

        float _fDuration = 0.5f;
        float _fElapsedTime = 0.0f;
        while (_fElapsedTime < _fDuration) {
            _fElapsedTime += Time.deltaTime;
            float _fProceed = _fElapsedTime / _fDuration;
            s_goCanvasGroup.alpha = 1.0f - _fProceed;
            yield return null;
        }
        s_goCanvasGroup.alpha = 0.0f;
        s_goCanvasGroup.interactable = false;
        s_goCanvasGroup.blocksRaycasts = false;
        m_bIsShowing = false;
    }
    #endregion

}
