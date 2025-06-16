using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolObstacleController : MonoBehaviour {

    #region Views
    [Header("Views")]
    [SerializeField]
    private Image s_uiObstacleImage;
    #endregion

    #region Functions
    public void SetObstacleModel(ObstacleModel p_oObstacleModel) {
        Sprite _oObstacleSprite = ThemeController.Instance.GetObstacleSprite(p_oObstacleModel.type);
        s_uiObstacleImage.sprite = _oObstacleSprite;
    }
    #endregion

}