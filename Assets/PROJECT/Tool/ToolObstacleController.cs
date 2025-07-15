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

    #region Views
    [Header("Views")]
    [SerializeField]
    private GameObject s_goLocker;
    [SerializeField]
    private GameObject s_goWoodbox;
    [SerializeField]
    private GameObject s_goBush;
    [SerializeField]
    private GameObject s_goBlueCrystal;
    [SerializeField]
    private GameObject s_goRedCrystal;
    #endregion

    #region Functions
    public void SetObstacleModel(ObstacleModel p_oObstacleModel) {
        if (p_oObstacleModel.type.Equals(ObstacleTypes.LOCKER) == true) {
            s_goLocker.gameObject.SetActive(true);
        }
        else if (p_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX) == true) {
            s_goWoodbox.gameObject.SetActive(true);
        }
        else if (p_oObstacleModel.type.Equals(ObstacleTypes.BUSH) == true) {
            s_goBush.gameObject.SetActive(true);
        }
        else if (p_oObstacleModel.type.Equals(ObstacleTypes.BLUE_CRYSTAL) == true) {
            s_goBlueCrystal.gameObject.SetActive(true);
        }
        else if (p_oObstacleModel.type.Equals(ObstacleTypes.RED_CRYSTAL) == true) {
            s_goRedCrystal.gameObject.SetActive(true);
        }
    }
    #endregion

}