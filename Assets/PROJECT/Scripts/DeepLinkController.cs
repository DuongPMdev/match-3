using System;
using System.Runtime.InteropServices;
using UnityEngine;

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
    public string GetData() {
        string _sAbsoluteURL = Application.absoluteURL;
        string _sAbsoluteUri = "";
        if (string.IsNullOrEmpty(_sAbsoluteURL) == false) {
            Uri _oUri = new Uri(_sAbsoluteURL);
            _sAbsoluteUri = _oUri.AbsoluteUri;
        }

        string _sLaunchParams = GetLaunchParams();

        return _sAbsoluteUri + "\n\n ===== \n\n" + _sLaunchParams;
    }
    #endregion

}
