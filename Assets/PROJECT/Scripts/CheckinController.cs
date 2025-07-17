using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckinController : MonoBehaviour {

    #region Singleton
    public static CheckinController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Views
    [SerializeField]
    private Sprite s_oClaimed;
    [SerializeField]
    private Sprite s_oClaim;
    [SerializeField]
    private Sprite s_oClaimLock;
    [SerializeField]
    private List<Button> s_lButtonClaim;
    #endregion

    #region Functions
    private void Start() {
        LoadUI();
    }

    private void LoadUI() {
        int _nAnchor = PlayerPrefsController.Instance.GetUserModel().checkin_count;
        for (int i = 0; i < s_lButtonClaim.Count; i++) {
            if (i < _nAnchor) {
                s_lButtonClaim[i].interactable = false;
                s_lButtonClaim[i].image.sprite = s_oClaimed;
            }
            else if (i == _nAnchor) {
                string _sAnchor = PlayerPrefsController.Instance.GetUserModel().checkin_anchor;
                DateTime _oAnchor = DateTime.UtcNow;
                bool _bIsEmpty = true;
                if (string.IsNullOrEmpty(_sAnchor) == false) {
                    _oAnchor = DateTime.FromBinary(Convert.ToInt64(_sAnchor));
                    _bIsEmpty = false;
                }
                TimeSpan _oTimeSpan = _oAnchor - DateTime.UtcNow;
                if (_bIsEmpty == true || _oTimeSpan.Days > 0) {
                    s_lButtonClaim[i].interactable = true;
                    s_lButtonClaim[i].image.sprite = s_oClaim;
                }
                else {
                    s_lButtonClaim[i].interactable = false;
                    s_lButtonClaim[i].image.sprite = s_oClaimLock;
                }
            }
            else {
                s_lButtonClaim[i].interactable = false;
                s_lButtonClaim[i].image.sprite = s_oClaimLock;
            }
        }
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonClaim(int p_nCoin) {
        PlayerPrefsController.Instance.Checkin();
        PlayerPrefsController.Instance.AddCoin(p_nCoin);
        LoadUI();
    }
    #endregion

}
