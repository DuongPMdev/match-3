using UnityEngine;

namespace NGenAssets {
    
    public class NGenAssets {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RuntimeInit() {
            Debug.Log("NGen Assets Runtime Initialize");
            if (SettingsManager.Instance == null) {
                GameObject _goSettingsManager = new("SettingsManager");
                _goSettingsManager.AddComponent<SettingsManager>();
            }
        }

    }

}
