using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour {
    
    #region Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private GameObject s_goPrefabPiece;
    [SerializeField]
    private GameObject s_goPrefabItem;
    [SerializeField]
    private GameObject s_goPrefabObstacle;
    [SerializeField]
    private GameObject s_goPrefabTakeDamageVFX;
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Transform s_tfPieceContainer;
    [SerializeField]
    private Transform s_tfItemContainer;
    [SerializeField]
    private Transform s_tfObstacleContainer;
    [SerializeField]
    private Transform s_tfVFXContainer;
    #endregion

    #region Variables
    private Vector2Int m_v2iPosition;
    private PieceController m_oPiece;
    private ItemController m_oItem;
    private ObstacleController m_oObstacle;
    #endregion

    #region Functions
    public void SetPosition(Vector2Int p_v2iPosition) {
        m_v2iPosition = p_v2iPosition;
        transform.localPosition = new Vector3(m_v2iPosition.x, m_v2iPosition.y, 0.0f);
    }

    public Vector2Int GetPosition() {
        return m_v2iPosition;
    }

    public int GetPieceValue() {
        if (m_oPiece != null) {
            return m_oPiece.GetPieceModel().piece;
        }
        if (m_oItem != null) {
            return m_oItem.GetItemModel().piece;
        }
        return -1;
    }

    public void CreatePiece(PieceModel p_oPieceModel, bool p_bUpper = false) {
        GameObject _goPiece = Instantiate(s_goPrefabPiece, s_tfPieceContainer.position, Quaternion.identity, s_tfPieceContainer);
        if (p_bUpper == true) {
            _goPiece.transform.localPosition = Vector3.up;
        }
        _goPiece.GetComponent<PieceController>().SetPieceModel(p_oPieceModel);
        _goPiece.GetComponent<PieceController>().SetTile(this);
        m_oPiece = _goPiece.GetComponent<PieceController>();
    }

    public void SetPiece(PieceController p_oPiece) {
        m_oPiece = p_oPiece;
        if (m_oPiece != null) {
            m_oPiece.transform.parent = s_tfPieceContainer;
            m_oPiece.SetTile(this);
        }
    }

    public PieceController GetPiece() {
        return m_oPiece;
    }

    public void CreateItem(ItemModel p_oItemModel) {
        GameObject _goItem = Instantiate(s_goPrefabItem, s_tfItemContainer.position, Quaternion.identity, s_tfItemContainer);
        _goItem.GetComponent<ItemController>().SetItemModel(p_oItemModel);
        _goItem.GetComponent<ItemController>().SetTile(this);
        m_oItem = _goItem.GetComponent<ItemController>();
    }

    public void SetItem(ItemController p_oItem) {
        m_oItem = p_oItem;
        if (m_oItem != null) {
            m_oItem.transform.parent = s_tfItemContainer;
            m_oItem.SetTile(this);
        }
    }

    public void UpgradeItem(int p_nPiece, string p_sType) {
        if (m_oItem != null) {
            m_oItem.SetPiece(p_nPiece);
            m_oItem.UpgradeItem(p_sType);
        }
        else {
            if (m_oPiece != null) {
                ItemModel _oItemModel = new ItemModel(m_v2iPosition, p_nPiece, p_sType);
                CreateItem(_oItemModel);
                DestroyPiece();
            }
        }
    }

    public ItemController GetItem() {
        return m_oItem;
    }

    public void CreateObstacle(ObstacleModel p_oObstacleModel) {
        GameObject _goObstacle = Instantiate(s_goPrefabObstacle, s_tfObstacleContainer.position, Quaternion.identity, s_tfObstacleContainer);
        _goObstacle.GetComponent<ObstacleController>().SetObstacleModel(p_oObstacleModel);
        _goObstacle.GetComponent<ObstacleController>().SetTile(this);
        m_oObstacle = _goObstacle.GetComponent<ObstacleController>();
    }

    public void SetObstacle(ObstacleController p_oObstacle) {
        m_oObstacle = p_oObstacle;
        if (m_oObstacle != null) {
            m_oObstacle.transform.parent = s_tfObstacleContainer;
            m_oObstacle.SetTile(this);
        }
    }

    public ObstacleController GetObstacle() {
        return m_oObstacle;
    }

    public bool IsMoving() {
        if (m_oObstacle != null) {
            return m_oObstacle.IsMoving();
        }
        if (m_oItem != null) {
            return m_oItem.IsMoving();
        }
        if (m_oPiece != null) {
            return m_oPiece.IsMoving();
        }
        return false;
    }

    public bool IsNull() {
        if (m_oObstacle != null) {
            if (m_oObstacle.IsNull() == true) {
                return true;
            }
        }
        return false;
    }

    public bool IsMoveable() {
        if (IsEmpty() == true) {
            return false;
        }
        if (m_oObstacle != null) {
            if (m_oObstacle.IsNull() == true) {
                return false;
            }
        }
        return true;
    }

    public bool IsEmpty() {
        if (m_oObstacle != null) {
            return false;
        }
        else if (m_oItem != null) {
            return false;
        }
        else if (m_oPiece != null) {
            return false;
        }
        return true;
    }

    public bool IsMatch(TileController p_oTile) {
        if (m_oPiece != null && p_oTile.GetPiece() != null) {
            if (m_oPiece.GetPieceModel().piece == p_oTile.GetPiece().GetPieceModel().piece) {
                return true;
            }
        }
        if (m_oPiece != null && p_oTile.GetItem() != null) {
            if (m_oPiece.GetPieceModel().piece == p_oTile.GetItem().GetItemModel().piece) {
                return true;
            }
        }
        if (m_oItem != null && p_oTile.GetPiece() != null) {
            if (m_oItem.GetItemModel().piece == p_oTile.GetPiece().GetPieceModel().piece) {
                return true;
            }
        }
        if (m_oItem != null && p_oTile.GetItem() != null) {
            if (m_oItem.GetItemModel().piece == p_oTile.GetItem().GetItemModel().piece) {
                return true;
            }
        }
        return false;
    }

    public void ShiftDown(TileController p_oTile) {
        if (m_oObstacle != null) {
            p_oTile.SetObstacle(m_oObstacle);
            m_oObstacle = null;
        }
        if (m_oItem != null) {
            p_oTile.SetItem(m_oItem);
            m_oItem = null;
        }
        if (m_oPiece != null) {
            p_oTile.SetPiece(m_oPiece);
            m_oPiece = null;
        }
    }

    public void SwapWith(TileController p_oTile) {
        int _nNumberItem = 0;
        int _nNumberRainbowItem = 0;
        int _nNumberBombItem = 0;
        int _nPiece = 0;
        if (m_oItem != null) {
            _nNumberItem++;
            if (m_oItem.GetItemModel().type.Equals("rainbow") == true) {
                _nNumberRainbowItem++;
            }
            else {
                _nPiece = m_oItem.GetItemModel().piece;
                if (m_oItem.GetItemModel().type.Equals("bomb") == true) {
                    _nNumberBombItem++;
                }
            }
        }
        if (p_oTile.GetItem() != null) {
            _nNumberItem++;
            if (p_oTile.GetItem().GetItemModel().type.Equals("rainbow") == true) {
                _nNumberRainbowItem++;
            }
            else {
                _nPiece = p_oTile.GetItem().GetItemModel().piece;
                if (p_oTile.GetItem().GetItemModel().type.Equals("bomb") == true) {
                    _nNumberBombItem++;
                }
            }
        }

        if (_nNumberItem == 2) {
            SetItem(p_oTile.GetItem());
            if (_nNumberRainbowItem == 2) {
                UpgradeItem(_nPiece, "super_rainbow");
            }
            else if (_nNumberRainbowItem == 1) {
                if (_nNumberBombItem == 1) {
                    UpgradeItem(_nPiece, "bomb_rainbow");
                }
                else {
                    UpgradeItem(_nPiece, "clear_rainbow");
                }
            }
            else if (_nNumberBombItem == 2) {
                UpgradeItem(_nPiece, "super_bomb");
            }
            else if (_nNumberBombItem == 1) {
                UpgradeItem(_nPiece, "super_clear");
            }
            else if (_nNumberBombItem == 0) {
                UpgradeItem(_nPiece, "clear_row_column");
            }
            p_oTile.RemoveItem();
        }
        else {
            ObstacleController _oTempObstacle = m_oObstacle;
            ItemController _oTempItem = m_oItem;
            PieceController _oTempPiece = m_oPiece;

            SetObstacle(p_oTile.GetObstacle());
            SetItem(p_oTile.GetItem());
            SetPiece(p_oTile.GetPiece());

            p_oTile.SetObstacle(_oTempObstacle);
            p_oTile.SetItem(_oTempItem);
            p_oTile.SetPiece(_oTempPiece);
            
            if (_nNumberRainbowItem == 1) {
                if (m_oItem != null && p_oTile.GetPiece() != null) {
                    m_oItem.SetPiece(p_oTile.GetPiece().GetPieceModel().piece);
                }
                if (m_oPiece != null && p_oTile.GetItem() != null) {
                    p_oTile.GetItem().SetPiece(m_oPiece.GetPieceModel().piece);
                }
            }
        }
    }

    public void FillUp(PieceModel p_oPieceModel) {
        CreatePiece(p_oPieceModel, true);
    }

    public void ResetSpeed() {
        if (m_oObstacle != null) {
            m_oObstacle.ResetSpeed();
        }
        if (m_oItem != null) {
            m_oItem.ResetSpeed();
        }
        if (m_oPiece != null) {
            m_oPiece.ResetSpeed();
        }
    }

    public float GetCurrentSpeed() {
        if (m_oObstacle != null) {
            return m_oObstacle.GetCurrentSpeed();
        }
        if (m_oItem != null) {
            return m_oItem.GetCurrentSpeed();
        }
        if (m_oPiece != null) {
            return m_oPiece.GetCurrentSpeed();
        }
        return 0.0f;
    }

    public void Match() {
        if (m_oItem != null) {
            m_oItem.Active(m_oItem.GetItemModel().piece);
        }
        if (m_oPiece != null) {
            m_oPiece.Collect();
        }
    }

    public void TakeDamage(int p_nPiece, int p_nDamage) {
        GameObject _goTakeDamageVFX = Instantiate(s_goPrefabTakeDamageVFX, s_tfVFXContainer.position, Quaternion.identity, s_tfVFXContainer);
        Destroy(_goTakeDamageVFX, 0.3f);

        if (m_oObstacle != null) {
            m_oObstacle.TakeDamage(p_nDamage);
        }
        else if (m_oItem != null) {
            m_oItem.Active(p_nPiece);
        }
        else if (m_oPiece != null) {
            m_oPiece.Collect();
        }
    }

    public void RemovePiece() {
        m_oPiece = null;
    }

    public void DestroyPiece() {
        Destroy(m_oPiece.gameObject);
        m_oPiece = null;
    }

    public void RemoveItem() {
        m_oItem = null;
    }

    public void CleanItem() {
        if (s_tfItemContainer.childCount > 1) {
            Destroy(s_tfItemContainer.GetChild(0).gameObject);
        }
    }

    public void DestroyItem() {
        Destroy(m_oItem.gameObject);
        m_oItem = null;
    }

    public bool IsSamePosition(TileController p_oTile) {
        if (p_oTile != null && (p_oTile.GetPosition() - m_v2iPosition).magnitude == 0) {
            return true;
        }
        return false;
    }

    public bool IsNextTo(TileController p_oTile) {
        if ((p_oTile.GetPosition() - m_v2iPosition).magnitude == 1) {
            return true;
        }
        return false;
    }
    #endregion

}
