using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneController : MonoBehaviour {

    #region OnClickButtons
    public void OnClickButtonPlay() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
    #endregion

}
