using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour {

    #region Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private GameObject s_goPrefabButtonLevel;
    [SerializeField]
    private GameObject s_goPrefabButtonLevelSeperate;
    #endregion

    #region Views
    [SerializeField]
    private Transform s_tfButtonLevelContainer;
    #endregion

    #region Functions
    private void Start() {
        int _nNumberLevel = 100;
        for (int i = 1; i <= _nNumberLevel; i++) {
            GameObject _goButtonLevel = Instantiate(s_goPrefabButtonLevel, Vector3.zero, Quaternion.identity, s_tfButtonLevelContainer);
            _goButtonLevel.GetComponent<ButtonLevelController>().SetLevel(i);
            if (i < _nNumberLevel) {
                Instantiate(s_goPrefabButtonLevelSeperate, Vector3.zero, Quaternion.identity, s_tfButtonLevelContainer);
            }
        }
        StartCoroutine(UpdatePositionIE());
    }

    private IEnumerator UpdatePositionIE() {
        yield return null;
        s_tfButtonLevelContainer.parent.parent.GetComponent<ScrollRect>().horizontalNormalizedPosition = PlayerPrefsController.Instance.GetUserModel().max_level * 1.0f / 100;
    }
    #endregion

}
