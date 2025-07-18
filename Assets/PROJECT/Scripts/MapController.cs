using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour {

    #region Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private List<GameObject> s_lMapBackground;
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
        int _nLevel = PlayerPrefsController.Instance.GetUserModel().max_level;
        if (_nLevel > 36) {
            _nLevel = Random.Range(0, int.MaxValue) % 36 + 1;
        }
        TextAsset _oTextAsset = Resources.Load<TextAsset>("Level " + _nLevel);
        if (_oTextAsset != null) {
            string _sJSONData = _oTextAsset.text;
            LevelModel _oLevelModel = JsonUtility.FromJson<LevelModel>(_sJSONData);
            int _nMap = _oLevelModel.map;
            for (int i = 0; i < s_lMapBackground.Count; i++) {
                if (s_lMapBackground[i] != null) {
                    s_lMapBackground[i].SetActive(i == _nMap);
                }
            }
        }
        s_tfButtonLevelContainer.parent.parent.GetComponent<ScrollRect>().horizontalNormalizedPosition = (PlayerPrefsController.Instance.GetUserModel().max_level - 1) * 1.0f / 100;
    }
    #endregion

}
