using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITargetController : MonoBehaviour {

    #region Views
    [SerializeField]
    private List<Sprite> s_lSpritePiece;
    [SerializeField]
    private Sprite s_oSpriteWoodbox;
    [SerializeField]
    private Sprite s_oSpriteWoodboxHard;
    [SerializeField]
    private Sprite s_oSpriteBlueCrystal;
    [SerializeField]
    private Sprite s_oSpriteRedCrystal;

    [SerializeField]
    private Image s_uiIcon;
    [SerializeField]
    private TMP_Text s_uiLabelRequest;
    #endregion

    #region Functions
    public void SetTargetModel(TargetModel p_oTargetModel) {
        if (p_oTargetModel.type.Equals("piece") == true) {
            s_uiIcon.sprite = s_lSpritePiece[p_oTargetModel.value];
        }
        else if (p_oTargetModel.type.Equals("woodbox") == true) {
            s_uiIcon.sprite = s_oSpriteWoodbox;
        }
        else if (p_oTargetModel.type.Equals("woodbox_hard") == true) {
            s_uiIcon.sprite = s_oSpriteWoodboxHard;
        }
        else if (p_oTargetModel.type.Equals("blue_crystal") == true) {
            s_uiIcon.sprite = s_oSpriteBlueCrystal;
        }
        else if (p_oTargetModel.type.Equals("red_crystal") == true) {
            s_uiIcon.sprite = s_oSpriteRedCrystal;
        }

        int _nRequest = p_oTargetModel.request_amount - p_oTargetModel.collected;
        s_uiLabelRequest.text = _nRequest.ToString();
    }
    #endregion

}
