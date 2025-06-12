using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolItemController : MonoBehaviour {

    #region Views
    [Header("Views")]
    [SerializeField]
    private Image s_uiItemImage;
    #endregion

    #region Functions
    public void SetItemModel(ItemModel p_oItemModel) {
        Sprite _oItemSprite = ThemeController.Instance.GetItemSprite(p_oItemModel.piece, p_oItemModel.type);
        s_uiItemImage.sprite = _oItemSprite;
    }
    #endregion

}
