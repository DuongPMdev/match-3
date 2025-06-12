using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitSpriteToScreen : MonoBehaviour {

    private void Start() {
        StartCoroutine(FitToScreenIE());
    }
    
    private IEnumerator FitToScreenIE() {
        yield return new WaitForEndOfFrame();

        SpriteRenderer _oSpriteRenderer = GetComponent<SpriteRenderer>();

        if (_oSpriteRenderer == null || _oSpriteRenderer.sprite == null) {
            Debug.LogError("SpriteRenderer or Sprite is missing!");
        }

        float _fSpriteRendererWidth = _oSpriteRenderer.sprite.bounds.size.x;
        float _fSpriteRendererHeight = _oSpriteRenderer.sprite.bounds.size.y;

        float _fWorldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float _fWorldScreenWidth = _fWorldScreenHeight * 390.0f / 700.0f;

        float _fScale = Mathf.Min(_fWorldScreenWidth / _fSpriteRendererWidth, _fWorldScreenHeight / _fSpriteRendererHeight);

        transform.localScale = Vector3.one * _fScale;
    }

}
