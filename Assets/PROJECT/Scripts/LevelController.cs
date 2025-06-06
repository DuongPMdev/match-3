using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePair {

    public TileController g_oFirstPair;
    public TileController g_oSecondPair;

    public TilePair(TileController p_oFirstPair, TileController p_oSecondPair) {
        g_oFirstPair = p_oFirstPair;
        g_oSecondPair = p_oSecondPair;
    }

}

public class LevelController : MonoBehaviour {

    #region Singleton
    public static LevelController Instance;
    private void Awake() {
        Instance = this;
        LoadVariables();
    }
    #endregion

    #region Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private GameObject s_goPrefabTile;
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Transform s_tfFieldStable;
    [SerializeField]
    private Transform s_tfTileContainer;
    [SerializeField]
    private Transform s_tfSelector;
    #endregion

    #region Variables
    private LevelModel m_oLevelModel;
    private TileController[,] m_arTile;
    private TileController m_oSelectedTile;
    private TilePair m_oSwapingTilePair;

    private bool m_bMainHandleThread;
    private bool m_bIsUsingBoosterHammer;
    private int m_nNumberMovingObstacle;
    private int m_nNumberMovingItem;
    private int m_nNumberMovingPiece;
    private int m_nNumberCollectingPiece;

    private enum STATE {
        IDLE,
        SWAP,
        SWAP_BACK,
        BOOSTER,
        MOVE,
        MATCH,
        COLLECT
    }
    private STATE m_oState;
    #endregion

    #region Functions
    private void LoadVariables() {
        m_bMainHandleThread = false;
        m_bIsUsingBoosterHammer = false;
        m_nNumberMovingObstacle = 0;
        m_nNumberMovingItem = 0;
        m_nNumberMovingPiece = 0;
        m_nNumberCollectingPiece = 0;

        m_oState = STATE.IDLE;
    }

    private void Start() {
        InputController.Instance.SetOnPointerDown(OnPointerDown);
        InputController.Instance.SetOnDrag(OnDrag);
        InputController.Instance.SetOnPointerUp(OnPointerUp);
    }

    private void Update() {
        if (m_oState == STATE.IDLE) {

        }
        else if (m_oState == STATE.SWAP) {
            if (IsMoving() == false) {
                ResetSpeed();
                List<List<TileController>> _lListMatch = GetAllMergedMatch();
                if (_lListMatch.Count > 0) {
                    m_oState = STATE.MATCH;
                }
                else {
                    m_oSwapingTilePair.g_oFirstPair.SwapWith(m_oSwapingTilePair.g_oSecondPair);
                    m_oState = STATE.SWAP_BACK;
                }
            }
        }
        else if (m_oState == STATE.SWAP_BACK) {
            if (IsMoving() == false) {
                ResetSpeed();
                m_oState = STATE.IDLE;
            }
        }
        else if (m_oState == STATE.BOOSTER) {
            if (IsCollecting() == false) {
                m_oState = STATE.MOVE;
            }
        }
        else if (m_oState == STATE.MOVE) {
            for (int x = 0; x < m_oLevelModel.size.x; x++) {
                for (int y = 0; y < m_oLevelModel.size.y; y++) {
                    TileController _oTile = GetTileAt(new Vector2Int(x, y));
                    if (_oTile.IsNull() == false && _oTile.IsEmpty() == false) {
                        if (_oTile.IsMoving() == false) {
                            TileController _oShiftDownTile = GetShiftDownTile(_oTile);
                            if (_oShiftDownTile != null) {
                                _oTile.ShiftDown(_oShiftDownTile);
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < m_oLevelModel.size.x; x++) {
                TileController _oTile = GetTileAt(new Vector2Int(x, m_oLevelModel.size.y - 1));
                if (_oTile.IsEmpty() == true) {
                    PieceModel _oPieceModel = new PieceModel(_oTile.GetPosition(), Random.Range(1, 7));
                    _oTile.FillUp(_oPieceModel);
                }
            }

            if (IsMoving() == false) {
                m_oState = STATE.MATCH;
            }
        }
        else if (m_oState == STATE.MATCH) {
            ResetSpeed();
            List<List<TileController>> _lListMatch = GetAllMergedMatch();
            if (_lListMatch.Count > 0) {
                for (int i = 0; i < _lListMatch.Count; i++) {
                    List<TileController> _lMatch = _lListMatch[i];
                    for (int ii = 0; ii < _lMatch.Count; ii++) {
                        _lMatch[ii].TakeDamage(1);
                    }
                }
                m_oState = STATE.COLLECT;
            }
            else {
                m_oState = STATE.IDLE;
            }
        }
        else if (m_oState == STATE.COLLECT) {
            if (IsCollecting() == false) {
                m_oState = STATE.MOVE;
            }
        }
    }

    private void ResetSpeed() {
        for (int x = 0; x < m_oLevelModel.size.x; x++) {
            for (int y = 0; y < m_oLevelModel.size.y; y++) {
                TileController _oTile = GetTileAt(new Vector2Int(x, y));
                _oTile.ResetSpeed();
            }
        }
    }

    public void OnMoveObstacleStart() {
        m_nNumberMovingObstacle++;
    }

    public void OnMoveObstacleDone() {
        m_nNumberMovingObstacle--;
    }

    public void OnMoveItemStart() {
        m_nNumberMovingItem++;
    }

    public void OnMoveItemDone() {
        m_nNumberMovingItem--;
    }

    public void OnMovePieceStart() {
        m_nNumberMovingPiece++;
    }

    public void OnMovePieceDone() {
        m_nNumberMovingPiece--;
    }

    public void OnCollectPieceStart() {
        m_nNumberCollectingPiece++;
    }

    public void OnCollectPieceDone() {
        m_nNumberCollectingPiece--;
    }

    private void OnPointerDown(Vector3 p_v3PointerPosition) {
        if (IsMoving() == true || m_bIsUsingBoosterHammer == true || m_bMainHandleThread == true) {
            return;
        }
        
        Vector2Int _v2iSlotPosition = GetSlotPosition(p_v3PointerPosition);
        TileController _oTile = GetTileAt(_v2iSlotPosition);
        if (_oTile != null) {
            if (_oTile.IsMoveable() == false) {
                return;
            }
            if (m_oSelectedTile == null) {
                m_oSelectedTile = _oTile;
            }
            else {
                if (_oTile.IsSamePosition(m_oSelectedTile) == false) {
                    _oTile.SwapWith(m_oSelectedTile);
                    m_oSwapingTilePair = new TilePair(_oTile, m_oSelectedTile);
                    m_oState = STATE.SWAP;
                    m_oSelectedTile = null;
                }
                else {
                    if (true) {
                        if (m_oSelectedTile.IsSamePosition(_oTile) == true) {
                            UseBoosterHammer(_oTile);
                            m_oSelectedTile = null;
                        }
                    }
                }
            }
        }
        if (m_oSelectedTile != null) {
            s_tfSelector.gameObject.SetActive(true);
            s_tfSelector.position = m_oSelectedTile.transform.position;
        }
        else {
            s_tfSelector.gameObject.SetActive(false);
        }
    }

    private void OnDrag(Vector3 p_v3PointerPosition) {
        if (IsMoving() == true || m_bIsUsingBoosterHammer == true || m_bMainHandleThread == true) {
            return;
        }
        if (m_oSelectedTile == null) {
            return;
        }

        Vector2Int _v2iSlotPosition = GetSlotPosition(p_v3PointerPosition);
        TileController _oTile = GetTileAt(_v2iSlotPosition);
        if (_oTile != null) {
            if (_oTile.IsMoveable() == false) {
                return;
            }
            if (_oTile.IsSamePosition(m_oSelectedTile) == false) {
                _oTile.SwapWith(m_oSelectedTile);
                m_oSwapingTilePair = new TilePair(_oTile, m_oSelectedTile);
                m_oState = STATE.SWAP;
                m_oSelectedTile = null;
                s_tfSelector.gameObject.SetActive(false);
            }
        }
    }

    private void OnPointerUp(Vector3 p_v3PointerPosition) {
        if (IsMoving() == true || m_bIsUsingBoosterHammer == true || m_bMainHandleThread == true) {
            return;
        }
    }

    public bool IsMoving() {
        if (m_nNumberMovingObstacle > 0) {
            return true;
        }
        if (m_nNumberMovingItem > 0) {
            return true;
        }
        if (m_nNumberMovingPiece > 0) {
            return true;
        }
        return false;
    }

    public bool IsCollecting() {
        if (m_nNumberCollectingPiece > 0) {
            return true;
        }
        return false;
    }

    public void LoadLevel(LevelModel p_oLevelModel) {
        m_oLevelModel = p_oLevelModel;

        float _fCameraSize = Mathf.Max(m_oLevelModel.size.x + 0.5f, 5.0f);
        CameraController.Instance.SetCameraSize(_fCameraSize);

        ClearChild(s_tfTileContainer);
        s_tfFieldStable.localScale = new Vector3(m_oLevelModel.size.x + 0.1f, m_oLevelModel.size.y + 0.1f, 1.0f);
        s_tfTileContainer.localPosition = new Vector3((1 - m_oLevelModel.size.x) / 2.0f, (1 - m_oLevelModel.size.y) / 2.0f, 0.0f);

        m_arTile = new TileController[m_oLevelModel.size.x, m_oLevelModel.size.y];
        for (int x = 0; x < m_oLevelModel.size.x; x++) {
            for (int y = 0; y < m_oLevelModel.size.y; y++) {
                GameObject _goTile = Instantiate(s_goPrefabTile, Vector3.zero, Quaternion.identity, s_tfTileContainer);
                m_arTile[x, y] = _goTile.GetComponent<TileController>();
                m_arTile[x, y].SetPosition(new Vector2Int(x, y));
            }
        }

        for (int i = 0; i < m_oLevelModel.init_piece.Count; i++) {
            PieceModel _oPieceModel = m_oLevelModel.init_piece[i];
            TileController _oTile = GetTileAt(_oPieceModel.position);
            if (_oTile != null) {
                _oTile.CreatePieceModel(_oPieceModel);
            }
        }

        for (int i = 0; i < m_oLevelModel.init_item.Count; i++) {
            ItemModel _oItemModel = m_oLevelModel.init_item[i];
            TileController _oTile = GetTileAt(_oItemModel.position);
            if (_oTile != null) {
                _oTile.CreateItemModel(_oItemModel);
            }
        }

        for (int i = 0; i < m_oLevelModel.init_obstacle.Count; i++) {
            ObstacleModel _oObstacleModel = m_oLevelModel.init_obstacle[i];
            TileController _oTile = GetTileAt(_oObstacleModel.position);
            if (_oTile != null) {
                _oTile.CreateObstacleModel(_oObstacleModel);
            }
        }
    }

    private void UseBoosterHammer(TileController p_oTile) {
        StartCoroutine(UseBoosterHammerIE(p_oTile));
    }

    private IEnumerator UseBoosterHammerIE(TileController p_oTile) {
        m_bIsUsingBoosterHammer = true;
        yield return new WaitForSeconds(0.2f);
        m_bIsUsingBoosterHammer = false;
        if (p_oTile.IsNull() == false) {
            p_oTile.TakeDamage(1);
        }
        m_oState = STATE.BOOSTER;
    }

    private List<List<TileController>> GetAllMergedMatch() {
        List<List<TileController>> _lListMatch = new List<List<TileController>>();
        List<List<TileController>> _lListHorizontalMatch = GetHorizontalMatch();
        List<List<TileController>> _lListVerticalMatch = GetVerticalMatch();

        for (int i = 0; i < _lListHorizontalMatch.Count; i++) {
            bool _bMerge = false;
            for (int ii = 0; ii < _lListMatch.Count; ii++) {
                if (IsMergeableMatch(_lListHorizontalMatch[i], _lListMatch[ii]) == true) {
                    _lListMatch[ii] = MergeMatch(_lListHorizontalMatch[i], _lListMatch[ii]);
                    _bMerge = true;
                }
            }
            if (_bMerge == false) {
                _lListMatch.Add(_lListHorizontalMatch[i]);
            }
        }

        for (int i = 0; i < _lListVerticalMatch.Count; i++) {
            bool _bMerge = false;
            for (int ii = 0; ii < _lListMatch.Count; ii++) {
                if (IsMergeableMatch(_lListVerticalMatch[i], _lListMatch[ii]) == true) {
                    _lListMatch[ii] = MergeMatch(_lListVerticalMatch[i], _lListMatch[ii]);
                    _bMerge = true;
                }
            }
            if (_bMerge == false) {
                _lListMatch.Add(_lListVerticalMatch[i]);
            }
        }

        return _lListMatch;
    }

    private bool IsMergeableMatch(List<TileController> p_lFirstListMatch, List<TileController> p_lSecondListMatch) {
        for (int i = 0; i < p_lFirstListMatch.Count; i++) {
            for (int ii = 0; ii < p_lSecondListMatch.Count; ii++) {
                if (p_lFirstListMatch[i].IsSamePosition(p_lSecondListMatch[ii]) == true) {
                    return true;
                }
            }
        }
        return false;
    }

    private List<TileController> MergeMatch(List<TileController> p_lFirstListMatch, List<TileController> p_lSecondListMatch) {
        List<TileController> _lMergeMatch = new List<TileController>();
        for (int i = 0; i < p_lFirstListMatch.Count; i++) {
            if (_lMergeMatch.Contains(p_lFirstListMatch[i]) == false) {
                _lMergeMatch.Add(p_lFirstListMatch[i]);
            }
        }
        for (int i = 0; i < p_lSecondListMatch.Count; i++) {
            if (_lMergeMatch.Contains(p_lSecondListMatch[i]) == false) {
                _lMergeMatch.Add(p_lSecondListMatch[i]);
            }
        }
        return _lMergeMatch;
    }

    private List<List<TileController>> GetHorizontalMatch() {
        List<List<TileController>> _lListHorizontalMatch = new List<List<TileController>>();
        for (int y = 0; y < m_oLevelModel.size.y; y++) {
            for (int x = 0; x < m_oLevelModel.size.x - 2; x++) {
                TileController _oFirstTile = GetTileAt(new Vector2Int(x, y));
                TileController _oSecondTile = GetTileAt(new Vector2Int(x + 1, y));
                TileController _oThirdTile = GetTileAt(new Vector2Int(x + 2, y));
                if (_oFirstTile.IsMatch(_oSecondTile) == true && _oSecondTile.IsMatch(_oThirdTile) == true) {
                    _lListHorizontalMatch.Add(new List<TileController>() { _oFirstTile, _oSecondTile, _oThirdTile });
                }
            }
        }

        return _lListHorizontalMatch;
    }

    private List<List<TileController>> GetVerticalMatch() {
        List<List<TileController>> _lListVerticalMatch = new List<List<TileController>>();
        for (int x = 0; x < m_oLevelModel.size.x; x++) {
            for (int y = 0; y < m_oLevelModel.size.y - 2; y++) {
                TileController _oFirstTile = GetTileAt(new Vector2Int(x, y));
                TileController _oSecondTile = GetTileAt(new Vector2Int(x, y + 1));
                TileController _oThirdTile = GetTileAt(new Vector2Int(x, y + 2));
                if (_oFirstTile.IsMatch(_oSecondTile) == true && _oSecondTile.IsMatch(_oThirdTile) == true) {
                    _lListVerticalMatch.Add(new List<TileController>() { _oFirstTile, _oSecondTile, _oThirdTile });
                }
            }
        }

        return _lListVerticalMatch;
    }

    private TileController GetShiftDownTile(TileController p_oTile) {
        Vector2Int _v2iPosition = p_oTile.GetPosition();
        Vector2Int _v2iDownPosition = new Vector2Int(_v2iPosition.x, _v2iPosition.y - 1);
        TileController _oDownTile = GetTileAt(_v2iDownPosition);
        if (_oDownTile != null) {
            if (_oDownTile.IsEmpty() == true) {
                return _oDownTile;
            }
            else {
                Vector2Int _v2iLeftPosition = new Vector2Int(_v2iPosition.x - 1, _v2iPosition.y);
                Vector2Int _v2iDownLeftPosition = new Vector2Int(_v2iPosition.x - 1, _v2iPosition.y - 1);
                TileController _oLeftTile = GetTileAt(_v2iLeftPosition);
                TileController _oDownLeftTile = GetTileAt(_v2iDownLeftPosition);
                if (_oDownLeftTile != null && _oLeftTile != null) {
                    if (_oDownLeftTile.IsEmpty() == true) {
                        if (_oLeftTile.IsEmpty() == false && _oLeftTile.IsMoveable() == false) {
                            return _oDownLeftTile;
                        }
                    }
                }
                else {
                    Vector2Int _v2iRightPosition = new Vector2Int(_v2iPosition.x + 1, _v2iPosition.y);
                    Vector2Int _v2iDownRightPosition = new Vector2Int(_v2iPosition.x + 1, _v2iPosition.y - 1);
                    TileController _oRightTile = GetTileAt(_v2iRightPosition);
                    TileController _oDownRightTile = GetTileAt(_v2iDownRightPosition);
                    if (_oDownRightTile != null && _oRightTile != null) {
                        if (_oDownRightTile.IsEmpty() == true) {
                            if (_oRightTile.IsEmpty() == false && _oRightTile.IsMoveable() == false) {
                                return _oDownRightTile;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    private TileController GetTileAt(Vector2Int p_v2iPosition) {
        if (p_v2iPosition.x < 0 || p_v2iPosition.x >= m_oLevelModel.size.x) {
            return null;
        }
        if (p_v2iPosition.y < 0 || p_v2iPosition.y >= m_oLevelModel.size.y) {
            return null;
        }
        return m_arTile[p_v2iPosition.x, p_v2iPosition.y];
    }

    private Vector2Int GetSlotPosition(Vector3 p_v3Position) {
        Vector3 _v3AnchorPosition = p_v3Position + new Vector3(m_oLevelModel.size.x / 2.0f, m_oLevelModel.size.y / 2.0f, 0.0f);
        int _nX = _v3AnchorPosition.x >= 0.0f ? (int)_v3AnchorPosition.x : -1;
        int _nY = _v3AnchorPosition.y >= 0.0f ? (int)_v3AnchorPosition.y : -1;
        Vector2Int _v2iAnchorPosition = new Vector2Int(_nX, _nY);
        return _v2iAnchorPosition;
    }

    private void ClearChild(Transform p_tfParent) {
        foreach (Transform _tfChild in p_tfParent) {
            Destroy(_tfChild.gameObject);
        }
    }
    #endregion

}
