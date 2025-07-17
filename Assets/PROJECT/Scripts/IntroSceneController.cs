using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroSceneController : MonoBehaviour {

    #region Singleton
    public static IntroSceneController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private GameObject s_goButtonPlay;
    #endregion

    #region Functions
    private void Start() {
        s_goButtonPlay.SetActive(true);
    }

    public void OnLoadedUserData() {
        s_goButtonPlay.SetActive(true);
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonPlay() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
    #endregion

}
