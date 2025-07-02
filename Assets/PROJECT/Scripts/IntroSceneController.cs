using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroSceneController : MonoBehaviour {

    #region Views
    [Header("Views")]
    [SerializeField]
    private TMP_InputField s_uiInputFieldTest;
    #endregion

    #region OnClickButtons
    public void OnClickButtonPlay() {
        string _sData = DeepLinkController.Instance.GetData();
        Debug.Log(_sData);
        //s_uiInputFieldTest.text = _sData;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
    #endregion

}
