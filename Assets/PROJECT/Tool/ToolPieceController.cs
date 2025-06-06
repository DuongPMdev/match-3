using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolPieceController : MonoBehaviour {

    #region Views
    [Header("Views")]
    [SerializeField]
    private Image s_uiPieceImage;
    #endregion

    #region Functions
    public void SetPieceModel(PieceModel p_oPieceModel) {
        Sprite _oPieceSprite = ThemeController.Instance.GetPieceSprite(p_oPieceModel.piece);
        s_uiPieceImage.sprite = _oPieceSprite;
    }
    #endregion

}
