using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTileController : MonoBehaviour {

    #region Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private GameObject s_goPrefabPiece;
    [SerializeField]
    private GameObject s_goPrefabObstacle;
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private Transform s_tfPieceContainer;
    [SerializeField]
    private Transform s_tfObstacleContainer;
    #endregion

    #region Variables
    private Vector2Int m_v2iPosition;
    #endregion

    #region Functions
    public void SetPosition(Vector2Int p_v2iPosition) {
        m_v2iPosition = p_v2iPosition;
        GetComponent<RectTransform>().anchoredPosition = m_v2iPosition * 100;
    }

    public void SetPieceModel(PieceModel p_oPieceModel) {
        GameObject _goPiece = Instantiate(s_goPrefabPiece, s_tfPieceContainer.position, Quaternion.identity, s_tfPieceContainer);
        _goPiece.GetComponent<ToolPieceController>().SetPieceModel(p_oPieceModel);
    }

    public void SetObstacleModel(ObstacleModel p_oObstacleModel) {
        GameObject _goObstacle = Instantiate(s_goPrefabObstacle, s_tfObstacleContainer.position, Quaternion.identity, s_tfObstacleContainer);
    }
    #endregion

    #region OnClickButtons
    public void OnClickButtonSelectTile() {
        ToolSceneController.Instance.SelectTile(m_v2iPosition);
    }
    #endregion

}
