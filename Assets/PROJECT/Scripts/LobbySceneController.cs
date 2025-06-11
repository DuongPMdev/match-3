using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneController : MonoBehaviour {

    #region Singleton
    public static LobbySceneController Instance;
    private void Awake() {
        Instance = this;
        Application.targetFrameRate = 60;
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonSetting() {
        PopupSettingController.Instance.Show();
    }

    public void OnClickButtonPlayLevel(int p_nLevel) {
        PopupLevelInfoController.Instance.Show(p_nLevel);
    }
    #endregion

}
