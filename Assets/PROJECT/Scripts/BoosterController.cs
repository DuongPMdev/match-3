using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterController : MonoBehaviour {

    #region Singleton
    public static BoosterController Instance;
    private void Awake() {
        Instance = this;
        LoadVariables();
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Transform s_tfButtonBoosterContainer;
    [SerializeField]
    private Transform s_tfPopupBoosterContainer;
    #endregion

    #region Variables
    private List<int> m_lNumberBooster;
    #endregion

    #region Functions
    private void LoadVariables() {
        m_lNumberBooster = new List<int>() { 3, 3, 3, 3, 0 };
    }
    private void Start() {
        LoadUI();
    }

    private void LoadUI () {
        for (int i = 0; i < s_tfButtonBoosterContainer.childCount; i++) {
            if (m_lNumberBooster[i] > 0) {
                s_tfButtonBoosterContainer.GetChild(i).GetChild(0).gameObject.SetActive(true);
                s_tfButtonBoosterContainer.GetChild(i).GetChild(1).gameObject.SetActive(true);
                s_tfButtonBoosterContainer.GetChild(i).GetChild(2).gameObject.SetActive(false);
                s_tfButtonBoosterContainer.GetChild(i).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = m_lNumberBooster[i].ToString();
                s_tfButtonBoosterContainer.GetChild(i).GetComponent<Button>().interactable = true;

            }
            else {
                s_tfButtonBoosterContainer.GetChild(i).GetChild(0).gameObject.SetActive(false);
                s_tfButtonBoosterContainer.GetChild(i).GetChild(1).gameObject.SetActive(false);
                s_tfButtonBoosterContainer.GetChild(i).GetChild(2).gameObject.SetActive(true);
                s_tfButtonBoosterContainer.GetChild(i).GetComponent<Button>().interactable = false;
            }
        }
    }

    public void OnUseBoosterAperoad() {
        m_lNumberBooster[0]--;
        LoadUI();
        s_tfPopupBoosterContainer.GetChild(0).gameObject.SetActive(false);
    }

    public void OnUseBoosterBananaBomb() {
        m_lNumberBooster[1]--;
        LoadUI();
        s_tfPopupBoosterContainer.GetChild(1).gameObject.SetActive(false);
    }

    public void OnUseBoosterCombo() {
        m_lNumberBooster[2]--;
        LoadUI();
        s_tfPopupBoosterContainer.GetChild(2).gameObject.SetActive(false);
    }

    public void OnUseBoosterBarrelRoll() {
        m_lNumberBooster[3]--;
        LoadUI();
        s_tfPopupBoosterContainer.GetChild(3).gameObject.SetActive(false);
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonBoosterAperoad() {
        if (m_lNumberBooster[0] > 0) {
            s_tfPopupBoosterContainer.GetChild(0).gameObject.SetActive(true);
            LevelController.Instance.OnUseBoosterAperoad();
        }
    }

    public void OnClickButtonBoosterBananaBomb() {
        if (m_lNumberBooster[1] > 0) {
            s_tfPopupBoosterContainer.GetChild(1).gameObject.SetActive(true);
            LevelController.Instance.OnUseBoosterBananaBomb();
        }
    }

    public void OnClickButtonBoosterCombo() {
        return;
        if (m_lNumberBooster[2] > 0) {
            s_tfPopupBoosterContainer.GetChild(2).gameObject.SetActive(true);
            LevelController.Instance.OnUseBoosterCombo();
        }
    }

    public void OnClickButtonBoosterBarrelRoll() {
        if (m_lNumberBooster[3] > 0) {
            s_tfPopupBoosterContainer.GetChild(3).gameObject.SetActive(true);
            LevelController.Instance.OnUseBoosterBarrelRoll();
        }
    }

    public void OnClickButtonCancelBoosterAperoad() {
        s_tfPopupBoosterContainer.GetChild(0).gameObject.SetActive(false);
        LevelController.Instance.OnCancelBoosterAperoad();
    }

    public void OnClickButtonCancelBoosterBananaBomb() {
        s_tfPopupBoosterContainer.GetChild(1).gameObject.SetActive(false);
        LevelController.Instance.OnCancelBoosterBananaBomb();
    }

    public void OnClickButtonCancelBoosterCombo() {
        s_tfPopupBoosterContainer.GetChild(2).gameObject.SetActive(false);
        LevelController.Instance.OnCancelBoosterCombo();
    }

    public void OnClickButtonCancelBoosterBarrelRoll() {
        s_tfPopupBoosterContainer.GetChild(3).gameObject.SetActive(false);
        LevelController.Instance.OnCancelBoosterBarrelRoll();
    }
    #endregion

}
