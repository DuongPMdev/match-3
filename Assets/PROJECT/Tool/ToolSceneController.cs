using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ToolSceneController : MonoBehaviour {

    #region Singleton
    public static ToolSceneController Instance;
    private void Awake() {
        Application.targetFrameRate = 60;
        Instance = this;
        LoadVariables();
    }
    #endregion

    #region Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private GameObject s_goPrefabToolTile;
    #endregion

    #region Views
    [Header("UI Views")]
    [SerializeField]
    private TMP_InputField s_uiInputFieldLevel;
    [SerializeField]
    private TMP_InputField s_uiInputFieldLevelModelSizeX;
    [SerializeField]
    private TMP_InputField s_uiInputFieldLevelModelSizeY;

    [Header("Views")]
    [SerializeField]
    private RectTransform s_rtfField;
    [SerializeField]
    private Transform s_tfSelector;
    #endregion

    #region Variables
    private LevelModel m_oLevelModel;
    private Vector2Int m_v2iSelectedPosition;
    private ToolTileController[,] m_arToolTile;
    #endregion

    #region Functions
    private void LoadVariables() {
        m_oLevelModel = null;
    }

    private void Start() {
        int _nLevel = PlayerPrefs.GetInt("level", 1);
        s_uiInputFieldLevel.text = _nLevel.ToString();
        OnClickButtonLoadLevel();
    }

    private void LoadLevel(int p_nLevel) {
        TextAsset _oTextAsset = Resources.Load<TextAsset>("Level " + p_nLevel);
        if (_oTextAsset != null) {
            string _sJSONData = _oTextAsset.text;
            m_oLevelModel = JsonUtility.FromJson<LevelModel>(_sJSONData);
        }
        else {
            m_oLevelModel = new LevelModel(p_nLevel, Vector2Int.one);
        }
        LoadLevelModelToUI(false);
        SelectTile(Vector2Int.zero);
    }

    private void SetSize(Vector2Int p_v2iSize) {
        m_oLevelModel.size = p_v2iSize;
        LoadLevelModelToUI(true);
    }

    public void SelectTile(Vector2Int p_v2iPosition) {
        ToolTileController _oToolTile = GetToolTileAt(p_v2iPosition);
        if (_oToolTile != null) {
            m_v2iSelectedPosition = p_v2iPosition;
            s_tfSelector.position = _oToolTile.transform.position;
        }
    }

    private void SetInitPiece(int p_nValue) {
        bool _bContained = false;
        for (int i = 0; i < m_oLevelModel.init_piece.Count; i++) {
            PieceModel _oPieceModel = m_oLevelModel.init_piece[i];
            if (_oPieceModel.position.Equals(m_v2iSelectedPosition) == true) {
                if (p_nValue == 0) {
                    m_oLevelModel.init_piece.Remove(_oPieceModel);
                }
                else {
                    _oPieceModel.piece = p_nValue;
                }
                _bContained = true;
                break;
            }
        }
        if (_bContained == false) {
            if (p_nValue != 0) {
                m_oLevelModel.init_piece.Add(new PieceModel(m_v2iSelectedPosition, p_nValue));
            }
        }
        LoadLevelModelToUI(true);
    }

    private void SetInitObstacle(string p_sObstacle) {
        bool _bContained = false;
        for (int i = 0; i < m_oLevelModel.init_obstacle.Count; i++) {
            ObstacleModel _oObstacleModel = m_oLevelModel.init_obstacle[i];
            if (_oObstacleModel.position.Equals(m_v2iSelectedPosition) == true) {
                if (p_sObstacle.Equals("empty") == true) {
                    m_oLevelModel.init_obstacle.Remove(_oObstacleModel);
                }
                else {
                    _oObstacleModel.type = p_sObstacle;
                }
                _bContained = true;
                break;
            }
        }
        if (_bContained == false) {
            if (p_sObstacle.Equals("empty") == false) {
                m_oLevelModel.init_obstacle.Add(new ObstacleModel(m_v2iSelectedPosition, p_sObstacle));
            }
        }
        LoadLevelModelToUI(true);
    }

    private void LoadLevelModelToUI(bool p_bSave) {
        s_uiInputFieldLevel.text = m_oLevelModel.level.ToString();
        s_uiInputFieldLevelModelSizeX.text = m_oLevelModel.size.x.ToString();
        s_uiInputFieldLevelModelSizeY.text = m_oLevelModel.size.y.ToString();

        ClearChild(s_rtfField.transform);
        s_rtfField.sizeDelta = m_oLevelModel.size * 100;

        m_arToolTile = new ToolTileController[m_oLevelModel.size.x, m_oLevelModel.size.y];
        for (int x = 0; x < m_oLevelModel.size.x; x++) {
            for (int y = 0; y < m_oLevelModel.size.y; y++) {
                GameObject _goToolTile = Instantiate(s_goPrefabToolTile, s_rtfField.position, Quaternion.identity, s_rtfField.transform);
                m_arToolTile[x, y] = _goToolTile.GetComponent<ToolTileController>();
                m_arToolTile[x, y].SetPosition(new Vector2Int(x, y));
            }
        }

        for (int i = 0; i < m_oLevelModel.init_piece.Count; i++) {
            PieceModel _oPieceModel = m_oLevelModel.init_piece[i];
            ToolTileController _oToolTile = GetToolTileAt(_oPieceModel.position);
            if (_oToolTile != null) {
                _oToolTile.SetPieceModel(_oPieceModel);
            }
        }

        for (int i = 0; i < m_oLevelModel.init_obstacle.Count; i++) {
            ObstacleModel _oObstacleModel = m_oLevelModel.init_obstacle[i];
            ToolTileController _oToolTile = GetToolTileAt(_oObstacleModel.position);
            if (_oToolTile != null) {
                _oToolTile.SetObstacleModel(_oObstacleModel);
            }
        }

        if (p_bSave == true) {
            string _sJson = JsonUtility.ToJson(m_oLevelModel);
            string _sPath = Path.Combine(Application.dataPath, "PROJECT/Datas/Resources/", "Level " + m_oLevelModel.level + ".json");
            File.WriteAllText(_sPath, _sJson);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }

    private ToolTileController GetToolTileAt(Vector2Int p_v2iPosition) {
        if (p_v2iPosition.x < 0 || p_v2iPosition.x >= m_oLevelModel.size.x) {
            return null;
        }
        if (p_v2iPosition.y < 0 || p_v2iPosition.y >= m_oLevelModel.size.y) {
            return null;
        }
        return m_arToolTile[p_v2iPosition.x, p_v2iPosition.y];
    }

    private void ClearChild(Transform p_tfParent) {
        foreach (Transform _tfChild in p_tfParent) {
            Destroy(_tfChild.gameObject);
        }
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonLoadLevel() {
        int _nLevel = int.Parse(s_uiInputFieldLevel.text);
        LoadLevel(_nLevel);
    }

    public void OnClickButtonSetSize() {
        int _nSizeX = int.Parse(s_uiInputFieldLevelModelSizeX.text);
        int _nSizeY = int.Parse(s_uiInputFieldLevelModelSizeY.text);
        SetSize(new Vector2Int(_nSizeX, _nSizeY));
    }

    public void OnClickButtonSetInitPiece(int p_nValue) {
        SetInitPiece(p_nValue);
    }

    public void OnClickButtonSetInitObstacle(string p_sObstacle) {
        SetInitObstacle(p_sObstacle);
    }

    public void OnClickButtonTest() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    #endregion

}