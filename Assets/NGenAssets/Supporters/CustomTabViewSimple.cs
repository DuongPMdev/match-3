using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomTabViewSimple : MonoBehaviour {

    #region Views
    [SerializeField] private Transform s_tfTabButtonContainer;
    [SerializeField] private Transform s_tfTabContainer;
    #endregion

    #region Variables
    private int m_nSelectedTabIndex;
    private Action<int> m_oOnSelectTab;
    #endregion

    #region Functions
    private void Start() {
        m_nSelectedTabIndex = 0;
    }

    public void SelectTab(int p_nTab) {
        m_nSelectedTabIndex = p_nTab;
        foreach (Transform _tfChild in s_tfTabButtonContainer) {
            _tfChild.Find("Selected").gameObject.SetActive(false);
        }
        s_tfTabButtonContainer.GetChild(m_nSelectedTabIndex).Find("Selected").gameObject.SetActive(true);
        foreach (Transform _tfChild in s_tfTabContainer) {
            _tfChild.gameObject.SetActive(false);
        }
        s_tfTabContainer.GetChild(m_nSelectedTabIndex).gameObject.SetActive(true);
        m_oOnSelectTab?.Invoke(m_nSelectedTabIndex);
    }

    public void SetSelectTabCallback(Action<int> p_oOnSelectTab) {
        m_oOnSelectTab = p_oOnSelectTab;
    }
    #endregion

}
