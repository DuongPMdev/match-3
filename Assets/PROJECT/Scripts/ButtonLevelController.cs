using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonLevelController : MonoBehaviour {

    #region Views
    [Header("Views")]
    [SerializeField]
    private TMP_Text s_uiLabelLevel;
    #endregion

    #region Variables
    private int m_nLevel;
    #endregion

    #region Functions
    public void SetLevel(int p_nLevel) {
        m_nLevel = p_nLevel;
        s_uiLabelLevel.text = m_nLevel.ToString();
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonPlay() {
        LobbySceneController.Instance.OnClickButtonPlayLevel(m_nLevel);
    }
    #endregion

}
