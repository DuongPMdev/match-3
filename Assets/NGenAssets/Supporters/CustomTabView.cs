using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomTabView : MonoBehaviour, IBeginDragHandler, IEndDragHandler {

    #region Views
    [Header("Views")]
    [SerializeField]
    private ScrollRect s_uiTabs;
    [SerializeField]
    private Transform s_tfTabButtonContainer;
    #endregion

    #region Variables
    private int m_nNumberTab;
    private float m_fStep;
    private int m_nSelectedTabIndex;

    private bool m_bUpdatingTabPosition;
    private float m_fTargetTabPosition;
    private float m_fCurrentTabPositionSpeed;
    private float m_fUpdateTabPositionSpeed;

    private float m_fLastPointerDownTime;
    private float m_fLastPointerDownTabPosition;
    #endregion

    #region Functions
    private void Awake() {
        LoadVariables();
    }

    private void LoadVariables() {
        m_nNumberTab = s_uiTabs.content.childCount;
        m_fStep = 1.0f / (m_nNumberTab - 1);
        m_fUpdateTabPositionSpeed = 0.2f;
        m_nSelectedTabIndex = -1;
        m_bUpdatingTabPosition = false;
    }

    private void Start() {
        FitTabSize();
        SelectTab(0, false);
    }

    private void FixedUpdate() {
        UpdateTabPosition();
    }

    private void UpdateTabPosition() {
        if (m_bUpdatingTabPosition == true) {
            float _fCurrentTabPosition = s_uiTabs.horizontalNormalizedPosition;
            _fCurrentTabPosition = Mathf.SmoothDamp(_fCurrentTabPosition, m_fTargetTabPosition, ref m_fCurrentTabPositionSpeed, m_fUpdateTabPositionSpeed);
            s_uiTabs.horizontalNormalizedPosition = _fCurrentTabPosition;
            if (Mathf.Abs(_fCurrentTabPosition - m_fTargetTabPosition) < 0.001f) {
                s_uiTabs.horizontalNormalizedPosition = m_fTargetTabPosition;
                m_bUpdatingTabPosition = false;
            }
        }
    }

    private void FitTabSize() {
        Vector2 _v2TabSize = s_uiTabs.GetComponent<RectTransform>().rect.size;
        for (int i = 0; i < s_uiTabs.content.childCount; i++) {
            s_uiTabs.content.GetChild(i).GetComponent<RectTransform>().sizeDelta = _v2TabSize;
        }
    }

    public void OnBeginDrag(PointerEventData p_oEvent) {
        m_fLastPointerDownTime = Time.time;
        m_fLastPointerDownTabPosition = s_uiTabs.horizontalNormalizedPosition;
    }

    public void OnEndDrag(PointerEventData p_oEvent) {
        float _fCurrentTabPosition = s_uiTabs.horizontalNormalizedPosition;
        float _fDeltaTime = Time.time - m_fLastPointerDownTime;
        float _fDeltaPosition = _fCurrentTabPosition - m_fLastPointerDownTabPosition;
        int _nSelectedTab = m_nSelectedTabIndex;
        if (_fDeltaTime < 0.3f || Mathf.Abs(_fDeltaPosition) > m_fStep / 2.0f) {
            _nSelectedTab = m_nSelectedTabIndex + (_fDeltaPosition > 0.0f ? 1 : -1);
            _nSelectedTab = Mathf.Clamp(_nSelectedTab, 0, m_nNumberTab - 1);
        }
        SelectTab(_nSelectedTab);
    }

    public void SelectTab(int p_nTabIndex, bool p_bAnimated = true) {
        int _nLastTab = m_nSelectedTabIndex;
        m_nSelectedTabIndex = p_nTabIndex;
        m_fTargetTabPosition = (float)m_nSelectedTabIndex / (m_nNumberTab - 1);
        if (p_bAnimated == true) {
            if (_nLastTab != m_nSelectedTabIndex) {
                if (_nLastTab >= 0) {
                    s_tfTabButtonContainer.GetChild(_nLastTab).GetComponent<Animator>().SetTrigger("tUnselected");
                }
                s_tfTabButtonContainer.GetChild(m_nSelectedTabIndex).GetComponent<Animator>().SetTrigger("tSelected");
            }
            m_bUpdatingTabPosition = true;
        }
        else {
            s_tfTabButtonContainer.GetChild(m_nSelectedTabIndex).GetComponent<Animator>().SetTrigger("tImmediately");
            m_bUpdatingTabPosition = false;
            s_uiTabs.horizontalNormalizedPosition = m_fTargetTabPosition;
        }
        if (m_nSelectedTabIndex == 1) {
            //MissionController.Instance.OnFocusQuest();
        }
    }

    public void OnSelectTabButtonClick(int p_nTabIndex) {
        SelectTab(p_nTabIndex);
    }
    #endregion

}
