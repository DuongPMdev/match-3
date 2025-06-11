using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequenceScaleObject : MonoBehaviour {

    #region Views
    [SerializeField] private Transform s_tfTarget;
    #endregion

    #region Variables
    [Header("Frequency")]
    [SerializeField] private float s_fFrequency;
    [Header("Min")]
    [SerializeField] private float s_fMin;
    [Header("Max")]
    [SerializeField] private float s_fMax;
    private float m_fScale;
    private float m_fStep;
    #endregion

    #region Functions
    private void Start() {
        if (s_tfTarget == null) {
            s_tfTarget = transform;
            s_fFrequency = 0.8f;
            s_fMin = 0.95f;
            s_fMax = 1.05f;
        }
        m_fScale = s_tfTarget.localScale.x;
        m_fStep = (s_fMax - s_fMin) / (s_fFrequency / 0.02f);
    }

    private void FixedUpdate() {
        FixedUpdateAlphaObject();
    }

    private void FixedUpdateAlphaObject() {
        if (s_tfTarget != null) {
            m_fScale += m_fStep;
            s_tfTarget.localScale = Vector3.one * m_fScale;
            if (s_tfTarget.localScale.x <= s_fMin) {
                m_fStep = (s_fMax - s_fMin) / (s_fFrequency / 0.02f);
            }
            if (s_tfTarget.localScale.x >= s_fMax) {
                m_fStep = (s_fMin - s_fMax) / (s_fFrequency / 0.02f);
            }
        }
    }
    #endregion

}
