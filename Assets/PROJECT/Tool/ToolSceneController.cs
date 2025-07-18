using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    private Image s_uiMapStable;
    [SerializeField]
    private TMP_InputField s_uiInputFieldLevel;
    [SerializeField]
    private TMP_InputField s_uiInputFieldLevelModelMap;
    [SerializeField]
    private TMP_InputField s_uiInputFieldLevelModelSizeX;
    [SerializeField]
    private TMP_InputField s_uiInputFieldLevelModelSizeY;
    [SerializeField]
    private TMP_InputField s_uiInputFieldLevelModelMove;


    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetPiece1;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetPiece2;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetPiece3;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetPiece4;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetPiece5;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetPiece6;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetWoodbox;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetWoodboxHard;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetBlueCrystal;
    [SerializeField]
    private TMP_InputField s_uiInputFieldTargetRedCrystal;

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

    private void Update() {
        if (Input.GetKeyDown(KeyCode.N) == true) {
            OnClickButtonSetInitObstacle(ObstacleTypes.NULL);
        }
        if (Input.GetKeyDown(KeyCode.X) == true) {
            OnClickButtonSetInitObstacle("empty");
        }
        if (Input.GetKeyDown(KeyCode.W) == true) {
            OnClickButtonSetInitObstacle(ObstacleTypes.WOODBOX);
        }
        if (Input.GetKeyDown(KeyCode.R) == true) {
            OnClickButtonSetInitObstacle(ObstacleTypes.WOODBOX_HARD);
        }
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
        m_oLevelModel.level = p_nLevel;
        LoadLevelModelToUI(false);
        SelectTile(Vector2Int.zero);
    }

    private void SetSize(Vector2Int p_v2iSize) {
        m_oLevelModel.size = p_v2iSize;
        LoadLevelModelToUI(true);
    }

    private void SetMapAndMove(int p_nMap, int p_nMove) {
        m_oLevelModel.map = p_nMap;
        m_oLevelModel.move = p_nMove;
        LoadLevelModelToUI(true);
    }

    private void SetTarget() {
        int _nTargetPiece1 = int.Parse(s_uiInputFieldTargetPiece1.text);
        int _nTargetPiece2 = int.Parse(s_uiInputFieldTargetPiece2.text);
        int _nTargetPiece3 = int.Parse(s_uiInputFieldTargetPiece3.text);
        int _nTargetPiece4 = int.Parse(s_uiInputFieldTargetPiece4.text);
        int _nTargetPiece5 = int.Parse(s_uiInputFieldTargetPiece5.text);
        int _nTargetPiece6 = int.Parse(s_uiInputFieldTargetPiece6.text);
        int _nTargetWoodbox = int.Parse(s_uiInputFieldTargetWoodbox.text);
        int _nTargetWoodboxHard = int.Parse(s_uiInputFieldTargetWoodboxHard.text);
        int _nTargetBlueCrystal = int.Parse(s_uiInputFieldTargetBlueCrystal.text);
        int _nTargetRedCrystal = int.Parse(s_uiInputFieldTargetRedCrystal.text);

        m_oLevelModel.targets.Clear();
        if (_nTargetPiece1 > 0) {
            m_oLevelModel.targets.Add(new TargetModel("piece", 1, _nTargetPiece1));
        }
        if (_nTargetPiece2 > 0) {
            m_oLevelModel.targets.Add(new TargetModel("piece", 2, _nTargetPiece2));
        }
        if (_nTargetPiece3 > 0) {
            m_oLevelModel.targets.Add(new TargetModel("piece", 3, _nTargetPiece3));
        }
        if (_nTargetPiece4 > 0) {
            m_oLevelModel.targets.Add(new TargetModel("piece", 4, _nTargetPiece4));
        }
        if (_nTargetPiece5 > 0) {
            m_oLevelModel.targets.Add(new TargetModel("piece", 5, _nTargetPiece5));
        }
        if (_nTargetPiece6 > 0) {
            m_oLevelModel.targets.Add(new TargetModel("piece", 6, _nTargetPiece6));
        }

        if (_nTargetWoodbox > 0) {
            m_oLevelModel.targets.Add(new TargetModel("woodbox", 0, _nTargetWoodbox));
        }
        if (_nTargetWoodboxHard > 0) {
            m_oLevelModel.targets.Add(new TargetModel("woodbox_hard", 0, _nTargetWoodboxHard));
        }
        if (_nTargetBlueCrystal > 0) {
            m_oLevelModel.targets.Add(new TargetModel("blue_crystal", 0, _nTargetBlueCrystal));
        }
        if (_nTargetRedCrystal > 0) {
            m_oLevelModel.targets.Add(new TargetModel("red_crystal", 0, _nTargetRedCrystal));
        }
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
        if (p_sObstacle.Equals(ObstacleTypes.LOCKER) == false && p_sObstacle.Equals(ObstacleTypes.BLUE_CRYSTAL) == false && p_sObstacle.Equals(ObstacleTypes.RED_CRYSTAL) == false) {
            PieceModel _oPieceModel = GetInitPieceAt(m_v2iSelectedPosition);
            if (_oPieceModel != null) {
                m_oLevelModel.init_piece.Remove(_oPieceModel);
            }
            ItemModel _oItemModel = GetInitItemAt(m_v2iSelectedPosition);
            if (_oItemModel != null) {
                m_oLevelModel.init_item.Remove(_oItemModel);
            }
        }
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

    private void SetInitItem(int p_nPiece, string p_sItem) {
        bool _bContained = false;
        for (int i = 0; i < m_oLevelModel.init_item.Count; i++) {
            ItemModel _oItemModel = m_oLevelModel.init_item[i];
            if (_oItemModel.position.Equals(m_v2iSelectedPosition) == true) {
                if (p_sItem.Equals("empty") == true) {
                    m_oLevelModel.init_item.Remove(_oItemModel);
                }
                else {
                    _oItemModel.type = p_sItem;
                }
                _bContained = true;
                break;
            }
        }
        if (_bContained == false) {
            if (p_sItem.Equals("empty") == false) {
                m_oLevelModel.init_item.Add(new ItemModel(m_v2iSelectedPosition, p_nPiece, p_sItem));
            }
        }
        LoadLevelModelToUI(true);
    }

    private void AutoGeneratePiece() {
        m_oLevelModel.init_piece.Clear();
        for (int x = 0; x < m_oLevelModel.size.x; x++) {
            for (int y = 0; y < m_oLevelModel.size.y; y++) {
                ItemModel _oItemModel = GetInitItemAt(new Vector2Int(x, y));
                ObstacleModel _oObstacleModel = GetInitObstacleAt(new Vector2Int(x, y));
                if (_oItemModel != null) {
                    continue;
                }
                if (_oObstacleModel != null) {
                    if (_oObstacleModel.type.Equals(ObstacleTypes.NULL) == true) {
                        continue;
                    }
                    if (_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX) == true) {
                        continue;
                    }
                    if (_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX_HARD) == true) {
                        continue;
                    }
                    if (_oObstacleModel.type.Equals(ObstacleTypes.BUSH) == true) {
                        continue;
                    }
                }
                PieceModel _oPieceModel = new PieceModel(new Vector2Int(x, y), Random.Range(1, ThemeController.Instance.GetMaxPieceValue()));
                m_oLevelModel.init_piece.Add(_oPieceModel);
                while (IsMatch() == true) {
                    m_oLevelModel.init_piece.Remove(_oPieceModel);
                    _oPieceModel = new PieceModel(new Vector2Int(x, y), Random.Range(1, ThemeController.Instance.GetMaxPieceValue()));
                    m_oLevelModel.init_piece.Add(_oPieceModel);
                }
            }
        }
        LoadLevelModelToUI(true);
    }

    private bool IsMatch() {
        return IsHorizontalMatch() || IsVerticalMatch();
    }

    private bool IsHorizontalMatch() {
        for (int y = 0; y < m_oLevelModel.size.y; y++) {
            for (int x = 0; x < m_oLevelModel.size.x - 2; x++) {
                PieceModel _oFirstPiece = GetInitPieceAt(new Vector2Int(x, y));
                PieceModel _oSecondPiece = GetInitPieceAt(new Vector2Int(x + 1, y));
                PieceModel _oThirdPiece = GetInitPieceAt(new Vector2Int(x + 2, y));
                if (_oFirstPiece == null || _oSecondPiece == null || _oThirdPiece == null) {
                    continue;
                }
                if (_oFirstPiece.piece == _oSecondPiece.piece && _oSecondPiece.piece == _oThirdPiece.piece) {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsVerticalMatch() {
        for (int x = 0; x < m_oLevelModel.size.x; x++) {
            for (int y = 0; y < m_oLevelModel.size.y - 2; y++) {
                PieceModel _oFirstPiece = GetInitPieceAt(new Vector2Int(x, y));
                PieceModel _oSecondPiece = GetInitPieceAt(new Vector2Int(x, y + 1));
                PieceModel _oThirdPiece = GetInitPieceAt(new Vector2Int(x, y + 2));
                if (_oFirstPiece == null || _oSecondPiece == null || _oThirdPiece == null) {
                    continue;
                }
                if (_oFirstPiece.piece == _oSecondPiece.piece && _oSecondPiece.piece == _oThirdPiece.piece) {
                    return true;
                }
            }
        }

        return false;
    }

    private PieceModel GetInitPieceAt(Vector2Int p_v2iPosition) {
        for (int i = 0; i < m_oLevelModel.init_piece.Count; i++) {
            PieceModel _oPieceModel = m_oLevelModel.init_piece[i];
            if (_oPieceModel.position.Equals(p_v2iPosition) == true) {
                return _oPieceModel;
            }
        }
        return null;
    }

    private ItemModel GetInitItemAt(Vector2Int p_v2iPosition) {
        for (int i = 0; i < m_oLevelModel.init_item.Count; i++) {
            ItemModel _oItemModel = m_oLevelModel.init_item[i];
            if (_oItemModel.position.Equals(p_v2iPosition) == true) {
                return _oItemModel;
            }
        }
        return null;
    }

    private ObstacleModel GetInitObstacleAt(Vector2Int p_v2iPosition) {
        for (int i = 0; i < m_oLevelModel.init_obstacle.Count; i++) {
            ObstacleModel _oObstacleModel = m_oLevelModel.init_obstacle[i];
            if (_oObstacleModel.position.Equals(p_v2iPosition) == true) {
                return _oObstacleModel;
            }
        }
        return null;
    }

    private void LoadLevelModelToUI(bool p_bSave) {
        s_uiInputFieldLevel.text = m_oLevelModel.level.ToString();
        s_uiInputFieldLevelModelSizeX.text = m_oLevelModel.size.x.ToString();
        s_uiInputFieldLevelModelSizeY.text = m_oLevelModel.size.y.ToString();
        s_uiInputFieldLevelModelMove.text = m_oLevelModel.move.ToString();
        s_uiInputFieldLevelModelMap.text = m_oLevelModel.map.ToString();

        m_oLevelModel.targets.Clear();
        m_oLevelModel.targets.Add(new TargetModel("piece", 1, 10));

        s_uiMapStable.sprite = ThemeController.Instance.GetMapStable(m_oLevelModel.map);

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

        for (int i = 0; i < m_oLevelModel.init_item.Count; i++) {
            ItemModel _oItemModel = m_oLevelModel.init_item[i];
            ToolTileController _oToolTile = GetToolTileAt(_oItemModel.position);
            if (_oToolTile != null) {
                _oToolTile.SetItemModel(_oItemModel);
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

    public void OnClickButtonSetMapAndMove() {
        int _nMove = int.Parse(s_uiInputFieldLevelModelMove.text);
        int _nMap = int.Parse(s_uiInputFieldLevelModelMap.text);
        SetMapAndMove(_nMap, _nMove);
    }

    public void OnClickButtonSetTarget() {
        SetTarget();
    }

    public void OnClickButtonSetInitPiece(int p_nValue) {
        SetInitPiece(p_nValue);
    }

    public void OnClickButtonSetInitItemEmpty() {
        SetInitItem(0, "empty");
    }

    public void OnClickButtonSetInitItemRainbow() {
        SetInitItem(0, "rainbow");
    }

    public void OnClickButtonSetInitItemBomb(int p_nPiece) {
        SetInitItem(p_nPiece, "bomb");
    }

    public void OnClickButtonSetInitItemClearRow(int p_nPiece) {
        SetInitItem(p_nPiece, "clear_row");
    }

    public void OnClickButtonSetInitItemClearColumn(int p_nPiece) {
        SetInitItem(p_nPiece, "clear_column");
    }

    public void OnClickButtonSetInitObstacle(string p_sObstacle) {
        SetInitObstacle(p_sObstacle);
    }

    public void OnClickButtonAutoGeneratePiece() {
        AutoGeneratePiece();
    }

    public void OnClickButtonTest() {
        int _nLevel = int.Parse(s_uiInputFieldLevel.text);
        PlayerPrefs.SetInt("level", _nLevel);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    #endregion

}