using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Theme {

    public List<Sprite> g_lMapStable;
    public List<Sprite> g_lPieceSprite;
    public List<Sprite> g_lItemBombSprite;
    public List<Sprite> g_lItemClearColumnSprite;
    public List<Sprite> g_lItemClearRowSprite;
    public List<Sprite> g_lItemFooter;
    public Sprite g_oItemRainbow;
    public Sprite g_oObstacleNull;
    public Sprite g_oObstacleLocker;
    public Sprite g_oObstacleWoodbox;

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
    public int GetMaxPieceValue() {
        return m_lTheme[m_nTheme].g_lPieceSprite.Count;
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

    public Sprite GetItemFooter(int p_nPiece) {
        return m_lTheme[m_nTheme].g_lItemFooter[p_nPiece];
    }

    public Sprite GetObstacleSprite(string p_sType) {
        if (p_sType.Equals("null") == true) {
            return m_lTheme[m_nTheme].g_oObstacleNull;
        }
        else if (p_sType.Equals("locker") == true) {
            return m_lTheme[m_nTheme].g_oObstacleLocker;
        }
        else if (p_sType.Equals("woodbox") == true) {
            return m_lTheme[m_nTheme].g_oObstacleWoodbox;
        }
        return null;
    }

    public Sprite GetMapStable(int p_nMap) {
        if (0 < p_nMap && p_nMap < m_lTheme[m_nTheme].g_lMapStable.Count) {
            return m_lTheme[m_nTheme].g_lMapStable[p_nMap];
        }
        return null;
    }
    #endregion

}
