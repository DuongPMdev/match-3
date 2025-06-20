using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonLevelController : MonoBehaviour {

    #region Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private GameObject s_goPrefabButtonLevelPointer;
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private TMP_Text s_uiLabelLevel;
    [SerializeField]
    private GameObject s_goMapOn;
    [SerializeField]
    private GameObject s_goIconPlay;

    [SerializeField]
    private TMP_FontAsset s_oFontActive;
    [SerializeField]
    private TMP_FontAsset s_oFontInactive;

    [SerializeField]
    private Color s_oMapOnLabelColor;
    [SerializeField]
    private Color s_oMapOffLabelColor;
    #endregion

    #region Variables
    private int m_nLevel;
    #endregion

    #region Functions
    public void SetLevel(int p_nLevel) {
        m_nLevel = p_nLevel;
        s_uiLabelLevel.text = m_nLevel.ToString();

        int _nUnlockedLevel = PlayerPrefs.GetInt("unlocked_level", 1);
        if (m_nLevel < _nUnlockedLevel) {
            s_uiLabelLevel.font = s_oFontActive;
            s_uiLabelLevel.color = s_oMapOnLabelColor;
            s_goIconPlay.SetActive(false);
            s_goMapOn.SetActive(true);
        }
        else if (m_nLevel == _nUnlockedLevel) {
            s_uiLabelLevel.font = s_oFontActive;
            s_uiLabelLevel.color = s_oMapOnLabelColor;
            s_goIconPlay.SetActive(true);
            s_goMapOn.SetActive(true);
            GameObject _goButtonLevelPointer = Instantiate(s_goPrefabButtonLevelPointer, transform.position, Quaternion.identity, transform);
            _goButtonLevelPointer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else {
            s_uiLabelLevel.font = s_oFontInactive;
            s_uiLabelLevel.color = s_oMapOffLabelColor;
            s_goIconPlay.SetActive(false);
            s_goMapOn.SetActive(false);
        }
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonPlay() {
        LobbySceneController.Instance.OnClickButtonPlayLevel(m_nLevel);
    }
    #endregion

}
