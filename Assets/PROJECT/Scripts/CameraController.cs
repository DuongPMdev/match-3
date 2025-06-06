using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    #region Singleton
    public static CameraController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Functions
    public void SetCameraSize(float p_fWidth) {
        float _fSize = p_fWidth / 2.0f * Screen.height / Screen.width;
        Camera.main.orthographicSize = _fSize;
    }
    #endregion

}
