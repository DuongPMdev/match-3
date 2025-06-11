using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Theme {

    public List<Sprite> g_lPieceSprite;
    public List<Sprite> g_lItemBombSprite;
    public List<Sprite> g_lItemClearColumnSprite;
    public List<Sprite> g_lItemClearRowSprite;
    public Sprite g_oItemRainbow;

}

public class ThemeController : MonoBehaviour {

    #region Singleton
    public static ThemeController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Variables
    [Header("Variables")]
    [SerializeField]
    private int m_nTheme;
    [SerializeField]
    private List<Theme> m_lTheme;
    #endregion

    #region Functions
    public Sprite GetPieceSprite(int p_nIndex) {
        return m_lTheme[m_nTheme].g_lPieceSprite[p_nIndex];
    }

    public Sprite GetItemSprite(int p_nPiece, string p_sType) {
        if (p_sType.Equals("rainbow") == true) {
            return m_lTheme[m_nTheme].g_oItemRainbow;
        }
        if (p_sType.Equals("bomb") == true) {
            return m_lTheme[m_nTheme].g_lItemBombSprite[p_nPiece];
        }
        if (p_sType.Equals("clear_row") == true) {
            return m_lTheme[m_nTheme].g_lItemClearRowSprite[p_nPiece];
        }
        if (p_sType.Equals("clear_column") == true) {
            return m_lTheme[m_nTheme].g_lItemClearColumnSprite[p_nPiece];
        }
        return null;
    }
    #endregion

}
