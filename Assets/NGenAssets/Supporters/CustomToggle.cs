using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomToggle : MonoBehaviour {
    
    #region Views
    [Header("Views")]
    [SerializeField]
    private Animator s_oAnimator;
    #endregion

    #region Variables
    private Action m_oOnTurn;
    private bool m_bIsOn;
    #endregion

    #region Functions
    private void Update() {
        s_oAnimator.SetBool("bIsOn", m_bIsOn);
    }

    public void SetValue(bool p_bIsOn) {
        m_bIsOn = p_bIsOn;
    }

    public void SetOnTurn(Action p_oOnTurn) {
        m_oOnTurn = p_oOnTurn;
    }
    #endregion

    #region Functions
    public void OnClickButtonTurn() {
        m_oOnTurn?.Invoke();
    }
    #endregion

}
