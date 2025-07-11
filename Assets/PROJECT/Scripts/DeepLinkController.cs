using System;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class LaunchParams {
    public string initData;
}

public class DeepLinkController : MonoBehaviour {
    
	#region Singleton
	public static DeepLinkController Instance;
	private void Awake() {
		Instance = this;

    }
    #endregion

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string GetLaunchParams();

#else
    public static string GetLaunchParams() {
        return "";
    }
#endif

    #region Variables
    #endregion

    #region Functions
    public string GetInitData() {
        try {
            string _sLaunchParams = GetLaunchParams();
            LaunchParams _oLaunchParams = JsonUtility.FromJson<LaunchParams>(_sLaunchParams);
            return _oLaunchParams.initData;
        }
        catch (Exception p_oException) {
            return "";
        }
    }
    #endregion

}
