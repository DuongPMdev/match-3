using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private Transform s_tfBoosterContainer;
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
        for (int i = 0; i < s_tfBoosterContainer.childCount; i++) {
            if (m_lNumberBooster[i] > 0) {
                s_tfBoosterContainer.GetChild(i).GetChild(0).gameObject.SetActive(true);
                s_tfBoosterContainer.GetChild(i).GetChild(1).gameObject.SetActive(true);
                s_tfBoosterContainer.GetChild(i).GetChild(2).gameObject.SetActive(false);
                s_tfBoosterContainer.GetChild(i).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = m_lNumberBooster[i].ToString();

            }
            else {
                s_tfBoosterContainer.GetChild(i).GetChild(0).gameObject.SetActive(false);
                s_tfBoosterContainer.GetChild(i).GetChild(1).gameObject.SetActive(false);
                s_tfBoosterContainer.GetChild(i).GetChild(2).gameObject.SetActive(true);
            }
        }
    }
    #endregion

}
