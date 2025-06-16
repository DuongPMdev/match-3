using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaSpinController : MonoBehaviour {

    #region Singleton
    public static BananaSpinController Instance;
    private void Awake() {
        Instance = this;
        LoadVariables();
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Transform s_tfRotor;
    [SerializeField]
    private GameObject s_goBananaSpinLocker;
    #endregion

    #region Variables
    private float m_fSpinStartSpeed;
    private float m_fSpinUpAcceleration;
    private float m_fSpinDownAcceleration;
    private float m_fSpinMaxSpeed;

    private float m_fSpinCurrentSpeed;
    private bool m_bIsSpinning;

    private float m_fDecelDistance;
    private float m_fPartRange;
    private float m_fThreshold;
    #endregion

    #region Functions
    private void LoadVariables() {
        m_fSpinStartSpeed = 720.0f;
        m_fSpinUpAcceleration = 900.0f;
        m_fSpinDownAcceleration = 180.0f;
        m_fSpinMaxSpeed = 900.0f;
        m_fSpinCurrentSpeed = 0.0f;
        m_bIsSpinning = false;

        m_fDecelDistance = m_fSpinMaxSpeed * m_fSpinMaxSpeed / (2.0f * m_fSpinDownAcceleration);
        m_fPartRange = 360.0f / s_tfRotor.childCount;
        m_fThreshold = Mathf.Min(5.0f, m_fPartRange / 4.0f);
    }

    public void Spin() {
        m_fSpinCurrentSpeed = m_fSpinStartSpeed;
        StartCoroutine(SpinIE(2));
    }

    private IEnumerator SpinIE(int p_nTarget) {
        m_bIsSpinning = true;
        s_goBananaSpinLocker.SetActive(true);

        float _fCurrentRotateZ = s_tfRotor.rotation.eulerAngles.z;
        while (m_fSpinCurrentSpeed < m_fSpinMaxSpeed) {
            if (m_fSpinCurrentSpeed < m_fSpinMaxSpeed) {
                m_fSpinCurrentSpeed = Mathf.Min(m_fSpinCurrentSpeed + m_fSpinUpAcceleration * Time.deltaTime, m_fSpinMaxSpeed);
            }

            _fCurrentRotateZ -= m_fSpinCurrentSpeed * Time.deltaTime;
            if (_fCurrentRotateZ > 360.0f) {
                _fCurrentRotateZ -= 360.0f;
            }
            s_tfRotor.localRotation = Quaternion.Euler(0.0f, 0.0f, _fCurrentRotateZ);
            for (int i = 0; i < s_tfRotor.childCount; i++) {
                s_tfRotor.GetChild(i).GetChild(0).localRotation = Quaternion.Euler(0.0f, 0.0f, -_fCurrentRotateZ);
            }
            yield return null;
        }

        float _fMinTargetStopAngle = p_nTarget * m_fPartRange + m_fThreshold;
        float _fMaxTargetStopAngle = (p_nTarget + 1) * m_fPartRange - m_fThreshold;
        float _fTargetStopAngle = Random.Range(_fMinTargetStopAngle, _fMaxTargetStopAngle);
        float _fTargetStartDecelAngle = (360f - _fTargetStopAngle + m_fDecelDistance) % 360f;

        while (Mathf.Abs(Mathf.DeltaAngle(_fCurrentRotateZ, _fTargetStartDecelAngle)) > m_fThreshold) {
            _fCurrentRotateZ -= m_fSpinCurrentSpeed * Time.deltaTime;
            if (_fCurrentRotateZ > 360.0f) {
                _fCurrentRotateZ -= 360.0f;
            }
            s_tfRotor.localRotation = Quaternion.Euler(0.0f, 0.0f, _fCurrentRotateZ);
            for (int i = 0; i < s_tfRotor.childCount; i++) {
                s_tfRotor.GetChild(i).GetChild(0).localRotation = Quaternion.Euler(0.0f, 0.0f, -_fCurrentRotateZ);
            }
            yield return null;
        }

        while (m_fSpinCurrentSpeed > 0.0f) {
            m_fSpinCurrentSpeed = Mathf.Max(m_fSpinCurrentSpeed - m_fSpinDownAcceleration * Time.deltaTime, 0.0f);

            _fCurrentRotateZ -= m_fSpinCurrentSpeed * Time.deltaTime;
            if (_fCurrentRotateZ > 360.0f) {
                _fCurrentRotateZ -= 360.0f;
            }
            s_tfRotor.localRotation = Quaternion.Euler(0.0f, 0.0f, _fCurrentRotateZ);
            for (int i = 0; i < s_tfRotor.childCount; i++) {
                s_tfRotor.GetChild(i).GetChild(0).localRotation = Quaternion.Euler(0.0f, 0.0f, -_fCurrentRotateZ);
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        PopupBananaSpinController.Instance.Show();

        s_goBananaSpinLocker.SetActive(false);
        m_bIsSpinning = false;
    }
    #endregion

}
