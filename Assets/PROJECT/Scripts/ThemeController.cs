using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PieceSprite {

    public List<Sprite> g_lPieceSprite;

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
    private List<PieceSprite> m_lPieceSprite;
    #endregion

    #region Functions
    public Sprite GetPieceSprite(int p_nIndex) {
        return m_lPieceSprite[m_nTheme].g_lPieceSprite[p_nIndex];
    }
    #endregion

}
