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

public class CreatingItem {

    public TileController g_oTile;
    public ItemModel g_oItem;

    public CreatingItem(TileController p_oTile, ItemModel p_oItem) {
        g_oTile = p_oTile;
        g_oItem = p_oItem;
    }

}

public class ActivingItem {

    public ItemController g_oItem;
    public int g_nPiece;

    public ActivingItem(ItemController p_oItem, int p_nPiece) {
        g_oItem = p_oItem;
        g_nPiece = p_nPiece;
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
    private List<CreatingItem> m_lCreatingItem;
    private List<ActivingItem> m_lActivingItem;
    private List<ActivingItem> m_lActivingItemTwice;

    private bool m_bMainHandleThread;
    private bool m_bIsUsingBoosterHammer;
    private int m_nNumberMovingObstacle;
    private int m_nNumberMovingItem;
    private int m_nNumberMovingPiece;
    private int m_nNumberCollectingPiece;
    private int m_nNumberCreatingItem;
    private int m_nNumberActivingItem;

    private enum STATE {
        IDLE,
        SWAP,
        SWAP_BACK,
        BOOSTER,
        MOVE,
        ACTIVE_ITEM,
        MATCH,
        COLLECT,
        CREATE_ITEM
    }
    private STATE m_oState;
    #endregion

    #region Functions
    private void LoadVariables() {
        m_lCreatingItem = new List<CreatingItem>();
        m_lActivingItem = new List<ActivingItem>();
        m_lActivingItemTwice = new List<ActivingItem>();

        m_bMainHandleThread = false;
        m_bIsUsingBoosterHammer = false;
        m_nNumberMovingObstacle = 0;
        m_nNumberMovingItem = 0;
        m_nNumberMovingPiece = 0;
        m_nNumberCollectingPiece = 0;
        m_nNumberCreatingItem = 0;
        m_nNumberActivingItem = 0;

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
                List<List<TileController>> _lListMatch = GetAllMergedMatch();
                if (_lListMatch.Count > 0) {
                    m_oState = STATE.MATCH;
                }
                else {
                    if (m_lActivingItem.Count > 0) {
                        for (int i = 0; i < m_lActivingItem.Count; i++) {
                            m_lActivingItem[i].g_oItem.Active(m_lActivingItem[i].g_nPiece);
                        }
                        m_lActivingItem.Clear();
                        m_oState = STATE.ACTIVE_ITEM;
                    }
                    else {
                        ResetSpeed();
                        m_oSwapingTilePair.g_oFirstPair.SwapWith(m_oSwapingTilePair.g_oSecondPair);
                        m_oState = STATE.SWAP_BACK;
                    }
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
            if (IsCollectingPiece() == false && IsActivingItem() == false) {
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
                if (m_lActivingItem.Count > 0 || m_lActivingItemTwice.Count > 0) {
                    for (int i = 0; i < m_lActivingItemTwice.Count; i++) {
                        m_lActivingItemTwice[i].g_oItem.ActiveTwice(m_lActivingItemTwice[i].g_nPiece);
                    }
                    m_lActivingItemTwice.Clear();
                    for (int i = 0; i < m_lActivingItem.Count; i++) {
                        m_lActivingItem[i].g_oItem.Active(m_lActivingItem[i].g_nPiece);
                    }
                    m_lActivingItem.Clear();
                    m_oState = STATE.ACTIVE_ITEM;
                }
                else {
                    m_oState = STATE.MATCH;
                }
            }
        }
        else if (m_oState == STATE.ACTIVE_ITEM) {
            if (IsActivingItem() == false) {
                ResetSpeed();
                m_oState = STATE.MOVE;
            }
        }
        else if (m_oState == STATE.MATCH) {
            List<List<TileController>> _lListMatch = GetAllMergedMatch();
            for (int i = 0; i < _lListMatch.Count; i++) {
                List<TileController> _lMatch = _lListMatch[i];
                CollectMatch(_lMatch);
            }
            ResetSpeed();
            if (_lListMatch.Count > 0) {
                m_oState = STATE.COLLECT;
            }
            else {
                m_oState = STATE.IDLE;
            }
        }
        else if (m_oState == STATE.COLLECT) {
            if (IsCollectingPiece() == false && IsActivingItem() == false) {
                if (m_lCreatingItem.Count > 0) {
                    for (int i = 0; i < m_lCreatingItem.Count; i++) {
                        CreatingItem _oCreatingItem = m_lCreatingItem[i];
                        _oCreatingItem.g_oTile.CreateItem(_oCreatingItem.g_oItem);
                    }
                    m_lCreatingItem.Clear();
                    m_oState = STATE.CREATE_ITEM;
                }
                else {
                    if (m_lActivingItem.Count > 0) {
                        for (int i = 0; i < m_lActivingItem.Count; i++) {
                            m_lActivingItem[i].g_oItem.Active(m_lActivingItem[i].g_nPiece);
                        }
                        m_lActivingItem.Clear();
                        m_oState = STATE.ACTIVE_ITEM;
                    }
                    else {
                        if (IsCreatingItem() == false) {
                            m_oState = STATE.MOVE;
                        }
                    }
                }
            }
        }
        else if (m_oState == STATE.CREATE_ITEM) {
            if (IsCollectingPiece() == false) {
                if (m_lActivingItem.Count > 0) {
                    for (int i = 0; i < m_lActivingItem.Count; i++) {
                        m_lActivingItem[i].g_oItem.Active(m_lActivingItem[i].g_nPiece);
                    }
                    m_lActivingItem.Clear();
                    m_oState = STATE.ACTIVE_ITEM;
                }
                else {
                    if (IsCreatingItem() == false) {
                        m_oState = STATE.MOVE;
                    }
                }
            }
        }
    }

    private void CollectMatch(List<TileController> p_lMatch) {
        if (p_lMatch.Count > 3) {
            TileController _oMaxCurrentSpeed = GetMaxCurrentSpeed(p_lMatch);
            string _sCreatingItemType = GetCreatingItemType(p_lMatch);
            m_lCreatingItem.Add(new CreatingItem(_oMaxCurrentSpeed, new ItemModel(_oMaxCurrentSpeed.GetPosition(), _oMaxCurrentSpeed.GetPieceValue(), _sCreatingItemType)));
        }
        for (int i = 0; i < p_lMatch.Count; i++) {
            p_lMatch[i].Match();
            OnMatchAt(p_lMatch[i]);
        }
    }

    private void OnMatchAt(TileController p_oTile) {
        List<TileController> _lNeighberTile = GetAllNeighber(p_oTile);
        for (int i = 0; i < _lNeighberTile.Count; i++) {
            _lNeighberTile[i].OnMatchAround();
        }
    }

    private TileController GetMaxCurrentSpeed(List<TileController> p_lMatch) {
        float _fMaxCurrentSpeed = -1;
        TileController _oMaxCurrentSpeed = null;
        for (int i = 0; i < p_lMatch.Count; i++) {
            if (p_lMatch[i].GetItem() == null) {
                if (p_lMatch[i].GetPieceValue() > 0) {
                    if (p_lMatch[i].GetCurrentSpeed() > _fMaxCurrentSpeed) {
                        _oMaxCurrentSpeed = p_lMatch[i];
                        _fMaxCurrentSpeed = p_lMatch[i].GetCurrentSpeed();
                    }
                }
            }
        }
        if (_oMaxCurrentSpeed == null) {
            _oMaxCurrentSpeed = p_lMatch[0];
        }
        return _oMaxCurrentSpeed;
    }

    private string GetCreatingItemType(List<TileController> p_lMatch) {
        Dictionary<int, int> _dRowCounts = new Dictionary<int, int>();
        Dictionary<int, int> _dColumnCounts = new Dictionary<int, int>();
        
        for (int i = 0; i < p_lMatch.Count; i++) {
            if (_dRowCounts.ContainsKey(p_lMatch[i].GetPosition().y)) {
                _dRowCounts[p_lMatch[i].GetPosition().y]++;
            }
            else {
                _dRowCounts[p_lMatch[i].GetPosition().y] = 1;
            }


            if (_dColumnCounts.ContainsKey(p_lMatch[i].GetPosition().x)) {
                _dColumnCounts[p_lMatch[i].GetPosition().x]++;
            }
            else {
                _dColumnCounts[p_lMatch[i].GetPosition().x] = 1;
            }
        }

        int _nSameRowCount = 0;
        int _nSameColumnCount = 0;

        foreach (var _oPair in _dRowCounts) {
            if (_oPair.Value > 1) {
                _nSameRowCount += _oPair.Value;
            }
        }

        foreach (var _oPair in _dColumnCounts) {
            if (_oPair.Value > 1) {
                _nSameColumnCount += _oPair.Value;
            }
        }

        if (_nSameRowCount >= 5 || _nSameColumnCount >= 5) {
            return "rainbow";
        }
        else if (_nSameRowCount == 0) {
            return "clear_row";
        }
        else if (_nSameColumnCount == 0) {
            return "clear_column";
        }
        else {
            return "bomb";
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

    public void OnCreateItemStart() {
        m_nNumberCreatingItem++;
    }

    public void OnCreateItemDone() {
        m_nNumberCreatingItem--;
    }

    public void OnActiveItemStart() {
        m_nNumberActivingItem++;
    }

    public void OnActiveItemDone() {
        m_nNumberActivingItem--;
    }

    private void OnPointerDown(Vector3 p_v3PointerPosition) {
        if (IsMoving() == true || m_bIsUsingBoosterHammer == true || m_bMainHandleThread == true) {
            return;
        }
        
        Vector2Int _v2iSlotPosition = GetTilePosition(p_v3PointerPosition);
        TileController _oTile = GetTileAt(_v2iSlotPosition);
        if (_oTile != null) {
            if (_oTile.IsMoveable() == false) {
                return;
            }
            if (m_oSelectedTile == null) {
                m_oSelectedTile = _oTile;
            }
            else {
                if (_oTile.IsNextTo(m_oSelectedTile) == true) {
                    _oTile.SwapWith(m_oSelectedTile);
                    m_oSwapingTilePair = new TilePair(_oTile, m_oSelectedTile);
                    m_oState = STATE.SWAP;
                    m_oSelectedTile = null;
                }
                else {
                    if (_oTile.IsSamePosition(m_oSelectedTile) == true) {
                        if (true) {
                            if (m_oSelectedTile.IsSamePosition(_oTile) == true) {
                                UseBoosterHammer(_oTile);
                                m_oSelectedTile = null;
                            }
                        }
                    }
                    else {
                        m_oSelectedTile = _oTile;
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

        Vector2Int _v2iTilePosition = GetTilePosition(p_v3PointerPosition);
        TileController _oTile = GetTileAt(_v2iTilePosition);
        if (_oTile != null) {
            if (_oTile.IsMoveable() == false) {
                return;
            }
            if (_oTile.IsNextTo(m_oSelectedTile) == true) {
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

    public bool IsCollectingPiece() {
        if (m_nNumberCollectingPiece > 0) {
            return true;
        }
        return false;
    }

    public bool IsActivingItem() {
        if (m_nNumberActivingItem > 0) {
            return true;
        }
        return false;
    }

    public bool IsCreatingItem() {
        if (m_nNumberCreatingItem > 0) {
            return true;
        }
        return false;
    }

    public void LoadLevel(LevelModel p_oLevelModel) {
        m_oLevelModel = p_oLevelModel;

        float _fCameraSize = Mathf.Max(m_oLevelModel.size.x + 0.2f, 5.0f);
        CameraController.Instance.SetCameraSize(_fCameraSize);

        ClearChild(s_tfTileContainer);
        s_tfFieldStable.localScale = new Vector3(m_oLevelModel.size.x + 0.05f, m_oLevelModel.size.y + 0.05f, 1.0f);
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
                _oTile.CreatePiece(_oPieceModel);
            }
        }

        for (int i = 0; i < m_oLevelModel.init_item.Count; i++) {
            ItemModel _oItemModel = m_oLevelModel.init_item[i];
            TileController _oTile = GetTileAt(_oItemModel.position);
            if (_oTile != null) {
                _oTile.CreateItem(_oItemModel);
            }
        }

        for (int i = 0; i < m_oLevelModel.init_obstacle.Count; i++) {
            ObstacleModel _oObstacleModel = m_oLevelModel.init_obstacle[i];
            TileController _oTile = GetTileAt(_oObstacleModel.position);
            if (_oTile != null) {
                _oTile.CreateObstacle(_oObstacleModel);
            }
        }
    }

    public void AddActiveItem(int p_nPiece, ItemController p_oItem) {
        for (int i = 0; i < m_lActivingItem.Count; i++) {
            if (m_lActivingItem[i].g_oItem.GetItemModel().position == p_oItem.GetItemModel().position) {
                return;
            }
        }
        ActivingItem _oActivingItem = new ActivingItem(p_oItem, p_nPiece);
        m_lActivingItem.Add(_oActivingItem);
    }

    public void AddActiveItemTwice(int p_nPiece, ItemController p_oItem) {
        for (int i = 0; i < m_lActivingItemTwice.Count; i++) {
            if (m_lActivingItemTwice[i].g_oItem.GetItemModel().position == p_oItem.GetItemModel().position) {
                return;
            }
        }
        ActivingItem _oActivingItem = new ActivingItem(p_oItem, p_nPiece);
        m_lActivingItemTwice.Add(_oActivingItem);
    }

    public void ActiveItem(ItemModel p_oItemModel) {
        Vector2Int _v2iPosition = p_oItemModel.position;
        int _nPiece = p_oItemModel.piece;
        if (p_oItemModel.type.Equals("clear_row") == true) {
            StartCoroutine(ActiveItemClearRowIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("clear_column") == true) {
            StartCoroutine(ActiveItemClearColumnIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("bomb") == true) {
            StartCoroutine(ActiveItemBombIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("super_bomb") == true) {
            StartCoroutine(ActiveItemSuperBombIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("super_clear") == true) {
            StartCoroutine(ActiveItemSuperClearIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("clear_row_column") == true) {
            StartCoroutine(ActiveItemClearRowColumnIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("rainbow") == true) {
            StartCoroutine(ActiveItemRainbowIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("super_rainbow") == true) {
            StartCoroutine(ActiveItemSuperRainbowIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("bomb_rainbow") == true) {
            StartCoroutine(ActiveItemBombRainbowIE(_nPiece, _v2iPosition));
        }
        else if (p_oItemModel.type.Equals("clear_rainbow") == true) {
            StartCoroutine(ActiveItemClearRainbowIE(_nPiece, _v2iPosition));
        }
    }

    private IEnumerator ActiveItemClearRowIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        yield return new WaitForSeconds(0.1f);
        bool _bIsDone = false;
        int _nOffset = 1;
        while (_bIsDone == false) {
            Vector2Int _v2iLeft = new Vector2Int(p_v2iPosition.x - _nOffset, p_v2iPosition.y);
            Vector2Int _v2iRight = new Vector2Int(p_v2iPosition.x + _nOffset, p_v2iPosition.y);

            TileController _oTileLeft = GetTileAt(_v2iLeft);
            TileController _oTileRight = GetTileAt(_v2iRight);

            if (_oTileLeft == null && _oTileRight == null) {
                _bIsDone = true;
            }
            else {
                if (_oTileLeft != null) {
                    _oTileLeft.TakeDamage(p_nPiece, 1);
                }
                if (_oTileRight != null) {
                    _oTileRight.TakeDamage(p_nPiece, 1);
                }
                _nOffset++;
            }
            yield return new WaitForSeconds(0.05f);
        }
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemClearColumnIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        bool _bIsDone = false;
        int _nOffset = 1;
        while (_bIsDone == false) {
            Vector2Int _v2iUp = new Vector2Int(p_v2iPosition.x, p_v2iPosition.y - _nOffset);
            Vector2Int _v2iDown = new Vector2Int(p_v2iPosition.x, p_v2iPosition.y + _nOffset);

            TileController _oTileUp = GetTileAt(_v2iUp);
            TileController _oTileDown = GetTileAt(_v2iDown);

            if (_oTileDown == null && _oTileDown == null) {
                _bIsDone = true;
            }
            else {
                if (_oTileUp != null) {
                    _oTileUp.TakeDamage(p_nPiece, 1);
                }
                if (_oTileDown != null) {
                    _oTileDown.TakeDamage(p_nPiece, 1);
                }
                _nOffset++;
            }
            yield return new WaitForSeconds(0.05f);
        }
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemBombIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        yield return new WaitForSeconds(0.05f);
        List<Vector2Int> _lOffset = new List<Vector2Int>() {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
        };
        for (int i = 0; i < _lOffset.Count; i++) {
            Vector2Int _v2iPosition = p_v2iPosition + _lOffset[i];
            TileController _oTile = GetTileAt(_v2iPosition);
            if (_oTile != null) {
                _oTile.TakeDamage(p_nPiece, 1);
            }
        }
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemSuperBombIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        yield return new WaitForSeconds(0.05f);
        bool _bIsDone = false;
        int _nOffset = 1;
        List<TileController> _lTileTookDamage = new List<TileController>();
        while (_bIsDone == false && _nOffset <= 2) {
            List<Vector2Int> _lEffectedPosition = new List<Vector2Int>();
            for (int x = p_v2iPosition.x - _nOffset; x <= p_v2iPosition.x + _nOffset; x++) {
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y + _nOffset));
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y - _nOffset));
            }
            for (int y = p_v2iPosition.y - _nOffset + 1; y < p_v2iPosition.y + _nOffset; y++) {
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x + _nOffset, y));
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x - _nOffset, y));
            }

            _bIsDone = true;
            for (int i = 0; i < _lEffectedPosition.Count; i++) {
                TileController _oTile = GetTileAt(_lEffectedPosition[i]);
                if (_oTile != null) {
                    if (_lTileTookDamage.Contains(_oTile) == false) {
                        _oTile.TakeDamage(p_nPiece, 1);
                        _lTileTookDamage.Add(_oTile);
                    }
                    _bIsDone = false;
                }
            }
            if (_bIsDone == false) {
                _nOffset++;
            }
            yield return new WaitForSeconds(0.1f);
        }
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemSuperClearIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        bool _bIsDone = false;
        int _nOffset = 1;
        List<TileController> _lTileTookDamage = new List<TileController>();
        while (_bIsDone == false) {
            List<Vector2Int> _lEffectedPosition = new List<Vector2Int>();
            Vector2Int _v2iLeft = new Vector2Int(p_v2iPosition.x - _nOffset, p_v2iPosition.y); _lEffectedPosition.Add(_v2iLeft);
            Vector2Int _v2iLeftUp = _v2iLeft + Vector2Int.up; _lEffectedPosition.Add(_v2iLeftUp);
            Vector2Int _v2iLeftDown = _v2iLeft + Vector2Int.down; _lEffectedPosition.Add(_v2iLeftDown);

            Vector2Int _v2iRight = new Vector2Int(p_v2iPosition.x + _nOffset, p_v2iPosition.y); _lEffectedPosition.Add(_v2iRight);
            Vector2Int _v2iRightUp = _v2iRight + Vector2Int.up; _lEffectedPosition.Add(_v2iRightUp);
            Vector2Int _v2iRightDown = _v2iRight + Vector2Int.down; _lEffectedPosition.Add(_v2iRightDown);

            Vector2Int _v2iUp = new Vector2Int(p_v2iPosition.x, p_v2iPosition.y - _nOffset); _lEffectedPosition.Add(_v2iUp);
            Vector2Int _v2iUpLeft = _v2iUp + Vector2Int.left; _lEffectedPosition.Add(_v2iUpLeft);
            Vector2Int _v2iUpRight = _v2iUp + Vector2Int.right; _lEffectedPosition.Add(_v2iUpRight);

            Vector2Int _v2iDown = new Vector2Int(p_v2iPosition.x, p_v2iPosition.y + _nOffset); _lEffectedPosition.Add(_v2iDown);
            Vector2Int _v2iDownLeft = _v2iDown + Vector2Int.left; _lEffectedPosition.Add(_v2iDownLeft);
            Vector2Int _v2iDownRight = _v2iDown + Vector2Int.right; _lEffectedPosition.Add(_v2iDownRight);

            _bIsDone = true;
            for (int i = 0; i < _lEffectedPosition.Count; i++) {
                TileController _oTile = GetTileAt(_lEffectedPosition[i]);
                if (_oTile != null) {
                    if (_lTileTookDamage.Contains(_oTile) == false) {
                        _oTile.TakeDamage(p_nPiece, 1);
                        _lTileTookDamage.Add(_oTile);
                        _bIsDone = false;
                    }
                }
            }
            if (_bIsDone == false) {
                _nOffset++;
            }

            yield return new WaitForSeconds(0.05f);
        }
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemClearRowColumnIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        bool _bIsDone = false;
        int _nOffset = 1;
        List<TileController> _lTileTookDamage = new List<TileController>();
        while (_bIsDone == false) {
            List<Vector2Int> _lEffectedPosition = new List<Vector2Int>();
            Vector2Int _v2iLeft = new Vector2Int(p_v2iPosition.x - _nOffset, p_v2iPosition.y); _lEffectedPosition.Add(_v2iLeft);
            Vector2Int _v2iRight = new Vector2Int(p_v2iPosition.x + _nOffset, p_v2iPosition.y); _lEffectedPosition.Add(_v2iRight);
            Vector2Int _v2iUp = new Vector2Int(p_v2iPosition.x, p_v2iPosition.y - _nOffset); _lEffectedPosition.Add(_v2iUp);
            Vector2Int _v2iDown = new Vector2Int(p_v2iPosition.x, p_v2iPosition.y + _nOffset); _lEffectedPosition.Add(_v2iDown);

            _bIsDone = true;
            for (int i = 0; i < _lEffectedPosition.Count; i++) {
                TileController _oTile = GetTileAt(_lEffectedPosition[i]);
                if (_oTile != null) {
                    if (_lTileTookDamage.Contains(_oTile) == false) {
                        _oTile.TakeDamage(p_nPiece, 1);
                        _lTileTookDamage.Add(_oTile);
                        _bIsDone = false;
                    }
                }
            }
            if (_bIsDone == false) {
                _nOffset++;
            }

            yield return new WaitForSeconds(0.05f);
        }
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemRainbowIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        bool _bIsDone = false;
        int _nOffset = 1;
        List<TileController> _lTileTookDamage = new List<TileController>();
        while (_bIsDone == false) {
            List<Vector2Int> _lEffectedPosition = new List<Vector2Int>();
            for (int x = p_v2iPosition.x - _nOffset; x <= p_v2iPosition.x + _nOffset; x++) {
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y + _nOffset));
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y - _nOffset));
            }
            for (int y = p_v2iPosition.y - _nOffset + 1; y < p_v2iPosition.y + _nOffset; y++) {
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x + _nOffset, y));
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x - _nOffset, y));
            }

            _bIsDone = true;
            for (int i = 0; i < _lEffectedPosition.Count; i++) {
                TileController _oTile = GetTileAt(_lEffectedPosition[i]);
                if (_oTile != null) {
                    if (_oTile.GetPieceValue() == p_nPiece) {
                        if (_lTileTookDamage.Contains(_oTile) == false) {
                            _oTile.TakeDamage(p_nPiece, 1);
                            _lTileTookDamage.Add(_oTile);
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                    _bIsDone = false;
                }
            }
            if (_bIsDone == false) {
                _nOffset++;
            }
        }
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemSuperRainbowIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        bool _bIsDone = false;
        int _nOffset = 1;
        List<TileController> _lTileTookDamage = new List<TileController>();
        while (_bIsDone == false) {
            List<Vector2Int> _lEffectedPosition = new List<Vector2Int>();
            for (int x = p_v2iPosition.x - _nOffset; x <= p_v2iPosition.x + _nOffset; x++) {
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y + _nOffset));
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y - _nOffset));
            }
            for (int y = p_v2iPosition.y - _nOffset + 1; y < p_v2iPosition.y + _nOffset; y++) {
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x + _nOffset, y));
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x - _nOffset, y));
            }

            _bIsDone = true;
            for (int i = 0; i < _lEffectedPosition.Count; i++) {
                TileController _oTile = GetTileAt(_lEffectedPosition[i]);
                if (_oTile != null) {
                    if (_lTileTookDamage.Contains(_oTile) == false) {
                        _oTile.TakeDamage(p_nPiece, 1);
                        _lTileTookDamage.Add(_oTile);
                    }
                    _bIsDone = false;
                }
            }
            if (_bIsDone == false) {
                _nOffset++;
            }
            yield return new WaitForSeconds(0.05f);
        }
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemBombRainbowIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        bool _bIsDone = false;
        int _nOffset = 1;
        List<TileController> _lTileUpgraded = new List<TileController>();
        while (_bIsDone == false) {
            List<Vector2Int> _lEffectedPosition = new List<Vector2Int>();
            for (int x = p_v2iPosition.x - _nOffset; x <= p_v2iPosition.x + _nOffset; x++) {
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y + _nOffset));
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y - _nOffset));
            }
            for (int y = p_v2iPosition.y - _nOffset + 1; y < p_v2iPosition.y + _nOffset; y++) {
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x + _nOffset, y));
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x - _nOffset, y));
            }

            _bIsDone = true;
            for (int i = 0; i < _lEffectedPosition.Count; i++) {
                TileController _oTile = GetTileAt(_lEffectedPosition[i]);
                if (_oTile != null) {
                    if (_oTile.GetPiece() != null && _oTile.GetPieceValue() == p_nPiece) {
                        if (_lTileUpgraded.Contains(_oTile) == false) {
                            _oTile.UpgradeItem(p_nPiece, "bomb");
                            _lTileUpgraded.Add(_oTile);
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                    _bIsDone = false;
                }
            }
            if (_bIsDone == false) {
                _nOffset++;
            }
        }

        for (int i = 0; i < _lTileUpgraded.Count; i++) {
            ItemController _oItem = _lTileUpgraded[i].GetItem();
            if (_oItem != null) {
                _lTileUpgraded[i].GetItem().AutoActive(_oItem.GetItemModel().piece, i * 0.1f);
            }
        }
        yield return new WaitForSeconds((_lTileUpgraded.Count + 1) * 0.1f);
        OnActiveItemDone();
    }

    private IEnumerator ActiveItemClearRainbowIE(int p_nPiece, Vector2Int p_v2iPosition) {
        OnActiveItemStart();
        bool _bIsDone = false;
        int _nOffset = 1;
        List<TileController> _lTileUpgraded = new List<TileController>();
        while (_bIsDone == false) {
            List<Vector2Int> _lEffectedPosition = new List<Vector2Int>();
            for (int x = p_v2iPosition.x - _nOffset; x <= p_v2iPosition.x + _nOffset; x++) {
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y + _nOffset));
                _lEffectedPosition.Add(new Vector2Int(x, p_v2iPosition.y - _nOffset));
            }
            for (int y = p_v2iPosition.y - _nOffset + 1; y < p_v2iPosition.y + _nOffset; y++) {
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x + _nOffset, y));
                _lEffectedPosition.Add(new Vector2Int(p_v2iPosition.x - _nOffset, y));
            }

            _bIsDone = true;
            for (int i = 0; i < _lEffectedPosition.Count; i++) {
                TileController _oTile = GetTileAt(_lEffectedPosition[i]);
                if (_oTile != null) {
                    if (_oTile.GetPiece() != null && _oTile.GetPieceValue() == p_nPiece) {
                        if (_lTileUpgraded.Contains(_oTile) == false) {
                            string _sType = Random.Range(int.MinValue, int.MaxValue) > 0 ? "clear_row" : "clear_column";
                            _oTile.UpgradeItem(p_nPiece, _sType);
                            _lTileUpgraded.Add(_oTile);
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                    _bIsDone = false;
                }
            }
            if (_bIsDone == false) {
                _nOffset++;
            }
        }
        for (int i = 0; i < _lTileUpgraded.Count; i++) {
            ItemController _oItem = _lTileUpgraded[i].GetItem();
            if (_oItem != null) {
                _lTileUpgraded[i].GetItem().AutoActive(_oItem.GetItemModel().piece, i * 0.1f);
            }
        }
        yield return new WaitForSeconds((_lTileUpgraded.Count + 1) * 0.1f);
        OnActiveItemDone();
    }

    private void UseBoosterHammer(TileController p_oTile) {
        StartCoroutine(UseBoosterHammerIE(p_oTile));
    }

    private IEnumerator UseBoosterHammerIE(TileController p_oTile) {
        m_bIsUsingBoosterHammer = true;
        yield return new WaitForSeconds(0.2f);
        m_bIsUsingBoosterHammer = false;
        if (p_oTile.IsNull() == false) {
            p_oTile.TakeDamage(0, 1);
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

    private List<TileController> GetAllNeighber(TileController p_oTileController) {
        List<TileController> _lNeighberTile = new List<TileController>();
        List<Vector2Int> _lOffset = new List<Vector2Int>() {
            Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up
        };

        Vector2Int _v2iPosition = p_oTileController.GetPosition();
        for (int i = 0; i < _lOffset.Count; i++) {
            TileController _oNeighberTile = GetTileAt(_v2iPosition + _lOffset[i]);
            if (_oNeighberTile != null) {
                _lNeighberTile.Add(_oNeighberTile);
            }
        }

        return _lNeighberTile;
    }

    private Vector2Int GetTilePosition(Vector3 p_v3Position) {
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
