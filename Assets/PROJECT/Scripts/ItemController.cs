using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    #region Views
    [Header("Views")]
    [SerializeField]
    private SpriteRenderer s_oItemSpriteRenderer;
    [SerializeField]
    private AnimationCurve s_oSpawnScale;
    #endregion

    #region Variables
    private ItemModel m_oItemModel;
    private TileController m_oTile;

    private float m_fStartSpeed;
    private float m_fAcceleration;
    private float m_fMaxSpeed;
    private float m_fCurrentSpeed;

    private bool m_bIsMoving;
    private bool m_bIsActiveAfterMove;
    private bool m_bIsAutoActive;
    private int m_nActivedTime;
    #endregion

    #region Functions
    private void Awake() {
        m_fStartSpeed = GameSceneController.Instance.GetTileStartSpeed();
        m_fAcceleration = GameSceneController.Instance.GetTileAcceleration();
        m_fMaxSpeed = GameSceneController.Instance.GetTileMaxSpeed();
        m_fCurrentSpeed = m_fStartSpeed;
        m_bIsMoving = false;
        m_bIsActiveAfterMove = false;
        m_bIsAutoActive = false;
        m_nActivedTime = 0;
    }

    public void SetItemModel(ItemModel p_oItemModel) {
        m_oItemModel = p_oItemModel;

        Sprite _oItemSprite = ThemeController.Instance.GetItemSprite(p_oItemModel.piece, p_oItemModel.type);
        s_oItemSpriteRenderer.sprite = _oItemSprite;

        StartCoroutine(SpawnIE());
    }

    public void UpgradeItem(string p_sType) {
        m_oItemModel.type = p_sType;
        m_bIsActiveAfterMove = true;
    }

    public void AutoActive(int p_nPiece, float p_fDelay) {
        m_bIsAutoActive = true;
        StartCoroutine(AutoActiveIE(p_nPiece, p_fDelay));
    }

    private IEnumerator AutoActiveIE(int p_nPiece, float p_fDelay) {
        yield return new WaitForSeconds(p_fDelay);

        m_nActivedTime++;
        if (m_oItemModel.type.Equals("rainbow") == true) {
            m_oItemModel.piece = p_nPiece;
        }
        LevelController.Instance.ActiveItem(m_oItemModel);
        bool _bIsDestroy = true;
        if (m_oItemModel.type.Equals("bomb") == true || m_oItemModel.type.Equals("super_bomb") == true) {
            _bIsDestroy = false;
            m_bIsAutoActive = false;
            LevelController.Instance.AddActiveItemTwice(m_oItemModel.piece, this);
        }
        if (_bIsDestroy == true) {
            m_oTile.RemoveItem();
            Destroy(gameObject);
        }
    }

    public void SetPiece(int p_nPiece) {
        m_oItemModel.piece = p_nPiece;
        m_bIsActiveAfterMove = true;
    }

    public ItemModel GetItemModel() {
        return m_oItemModel;
    }

    private IEnumerator SpawnIE() {
        LevelController.Instance.OnCreateItemStart();

        float _fDuration = 0.1f;
        float _fElapsedTime = 0.0f;
        while (_fElapsedTime < _fDuration) {
            float _fProceed = _fElapsedTime / _fDuration;
            float _fScale = s_oSpawnScale.Evaluate(_fProceed);
            transform.localScale = Vector3.one * _fScale;

            _fElapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;
        LevelController.Instance.OnCreateItemDone();
    }

    public void SetTile(TileController p_oTile) {
        m_oTile = p_oTile;
        m_oItemModel.position = m_oTile.GetPosition();
        if (transform.localPosition.magnitude > 0.1f) {
            StartCoroutine(MoveToZeroLocalPositionIE());
        }
        else {
            transform.localPosition = Vector3.zero;
        }
    }

    public void ResetSpeed() {
        m_fCurrentSpeed = m_fStartSpeed;
    }

    public float GetCurrentSpeed() {
        return m_fCurrentSpeed;
    }

    private IEnumerator MoveToZeroLocalPositionIE() {
        m_bIsMoving = true;
        LevelController.Instance.OnMoveItemStart();

        Vector3 _v3TargetPosition = Vector3.zero;
        Vector3 _v3CurrentPosition = transform.localPosition;

        float _fDistance = Vector3.Distance(_v3CurrentPosition, _v3TargetPosition);
        while (_fDistance > 0.001f) {
            m_fCurrentSpeed = Mathf.Min(m_fCurrentSpeed + m_fAcceleration * Time.deltaTime, m_fMaxSpeed);

            Vector3 _v3NextPosition = Vector3.MoveTowards(_v3CurrentPosition, _v3TargetPosition, Mathf.Min(_fDistance, m_fCurrentSpeed * Time.deltaTime));
            transform.localPosition = _v3NextPosition;
            _v3CurrentPosition = _v3NextPosition;

            _fDistance = Vector3.Distance(_v3CurrentPosition, _v3TargetPosition);

            yield return null;
        }

        transform.localPosition = Vector3.zero;
        m_oTile.CleanItem();
        if (m_bIsActiveAfterMove == true) {
            LevelController.Instance.AddActiveItem(m_oItemModel.piece, this);
        }
        LevelController.Instance.OnMoveItemDone();
        m_bIsMoving = false;
    }

    public bool IsMoving() {
        return m_bIsMoving;
    }

    public void Active(int p_nPiece) {
        if (m_bIsAutoActive == true) {
            return;
        }
        if (m_nActivedTime > 0) {
            return;
        }
        m_nActivedTime++;
        if (m_oItemModel.type.Equals("rainbow") == true) {
            m_oItemModel.piece = p_nPiece;
        }
        LevelController.Instance.ActiveItem(m_oItemModel);
        bool _bIsDestroy = true;
        if (m_oItemModel.type.Equals("bomb") == true || m_oItemModel.type.Equals("super_bomb") == true) {
            _bIsDestroy = false;
            s_oItemSpriteRenderer.GetComponent<FrequenceScaleObject>().enabled = true;
            LevelController.Instance.AddActiveItemTwice(m_oItemModel.piece, this);
        }
        if (_bIsDestroy == true) {
            m_oTile.RemoveItem();
            Destroy(gameObject);
        }
    }

    public void ActiveTwice(int p_nPiece) {
        if (m_bIsAutoActive == true) {
            return;
        }
        if (m_nActivedTime != 1) {
            return;
        }
        m_nActivedTime++;
        if (m_oItemModel.type.Equals("bomb") == true || m_oItemModel.type.Equals("super_bomb") == true) {
            LevelController.Instance.ActiveItem(m_oItemModel);
            m_oTile.RemoveItem();
            Destroy(gameObject);
        }
    }
    #endregion

}
