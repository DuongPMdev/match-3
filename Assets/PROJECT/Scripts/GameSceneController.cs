using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameSceneController : MonoBehaviour {

    #region Singleton
    public static GameSceneController Instance;
    private void Awake() {
        Application.targetFrameRate = 60;
        Instance = this;
        LoadVariables();
    }
    #endregion

    #region Variables
    private float m_fTileStartSpeed;
    private float m_fTileAcceleration;
    private float m_fTileMaxSpeed;
    #endregion

    #region Functions
    private void LoadVariables() {
        m_fTileStartSpeed = 0.0f;
        m_fTileAcceleration = 40.0f;
        m_fTileMaxSpeed = 20.0f;
    }

    private void Start() {
        LoadLevel();
    }

    public float GetTileStartSpeed() {
        return m_fTileStartSpeed;
    }

    public float GetTileAcceleration() {
        return m_fTileAcceleration;
    }

    public float GetTileMaxSpeed() {
        return m_fTileMaxSpeed;
    }

    private void LoadLevel() {
        int _nLevel = PlayerPrefs.GetInt("level", 1);
        if (_nLevel > 100) {
            _nLevel = Random.Range(1, 101);
        }
        TextAsset _oTextAsset = Resources.Load<TextAsset>("Level " + _nLevel);
        if (_oTextAsset != null) {
            string _sJSONData = _oTextAsset.text;
            LevelModel _oLevelModel = JsonUtility.FromJson<LevelModel>(_sJSONData);
            LevelController.Instance.LoadLevel(_oLevelModel);
            if (APIController.Instance != null) {
                APIController.Instance.StartGame(_nLevel);
            }
        }
        else {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ToolScene");
        }
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonSetting() {
        PopupSettingController.Instance.Show();
        //UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
    #endregion

}
