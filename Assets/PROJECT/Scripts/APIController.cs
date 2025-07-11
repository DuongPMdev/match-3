using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[Serializable]
public class TelegramAuthResponse {
    public bool success;
    public TelegramData data;
}

[System.Serializable]
public class ProfileResponse {
    public bool success;
    public ProfileData data;
}

[Serializable]
public class ProfileData {
    public int next_life_in;
    public TelegramUser user;
}

[Serializable]
public class TelegramData {
    public string session_token;
    public TelegramUser user;
    public long expires_at;
}

[Serializable]
public class TelegramUser {
    public int id;
    public long telegram_id;
    public string username;
    public string first_name;
    public string last_name;
    public int level;
    public int total_score;
    public int coins;
    public int lives;
    public string last_life_refill;
    public string created_at;
    public string updated_at;
}

public class APIController : MonoBehaviour {

    #region Singleton
    public static APIController Instance;
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    #endregion

    [System.Serializable]
    public class InitData {
        public string init_data;
    }

    #region Variables
    private string m_sBaseURL = "https://apesmash-api.onrender.com/api/v1";
    private string m_sSessionToken;
    private TelegramUser m_oTelegramUser;
    #endregion

    #region Functions
    private void Start() {
        m_sSessionToken = PlayerPrefs.GetString("SessionToken", "");
        if (string.IsNullOrEmpty(m_sSessionToken) == false) {
            GetUserStats();
            GetUserProfile();
            GetLeaderboardGlobal();
        }
        else {
            TelegramAuth();
        }
    }

    public TelegramUser GetTelegramUser() {
        return m_oTelegramUser;
    }

    private void TelegramAuth() {
        StartCoroutine(TelegramAuthIE());
    }

    private IEnumerator TelegramAuthIE() {
        string url = m_sBaseURL + "/auth/telegram";
        
        InitData payload = new InitData {
            init_data = "user=%7B%22id%22%3A1894903459%2C%22first_name%22%3A%22Ph%E1%BA%A1m%22%2C%22last_name%22%3A%22%C4%90%C6%B0%C6%A1ng%22%2C%22username%22%3A%22duongpm13dev%22%2C%22language_code%22%3A%22vi%22%2C%22allows_write_to_pm%22%3Atrue%2C%22photo_url%22%3A%22https%3A%5C%2F%5C%2Ft.me%5C%2Fi%5C%2Fuserpic%5C%2F320%5C%2FAMvs2B34t-DSmX2Us40xZyNg_UF1tO9hTTVPPefnoj8.svg%22%7D&chat_instance=-6659888500702264886&chat_type=sender&auth_date=1752222835&signature=2YXWbrUy5hiYYDDzDGyGOy7YX3csSJHHoOHYvnbF68oMa1p_WkI-WD-kB7KCojOeAeeLSPqtdnOvAPlyCkkUBg&hash=8acb4d345815e2c1aa4af6c2d56ba2fbe9620d4605b94cac3443847713665ad6"
        };
        string _sInitData = DeepLinkController.Instance.GetInitData();
        if (string.IsNullOrEmpty(_sInitData) == false) {
            payload = new InitData { init_data = _sInitData };
        }

        string jsonData = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            TelegramAuthResponse _oTelegramAuthResponse = JsonUtility.FromJson<TelegramAuthResponse>(request.downloadHandler.text);
            m_sSessionToken = _oTelegramAuthResponse.data.session_token;
            PlayerPrefs.SetString("SessionToken", m_sSessionToken);
            GetUserStats();
            GetUserProfile();
            GetLeaderboardGlobal();
        }
        else {

        }
    }

    private void GetUserStats() {
        StartCoroutine(GetUserStatsIE());
    }

    private IEnumerator GetUserStatsIE() {
        string url = m_sBaseURL + "/user/stats";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {m_sSessionToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            string json = request.downloadHandler.text;
            Debug.Log(json);
        }
        else {
            TelegramAuth();
        }
    }

    private void GetUserProfile() {
        StartCoroutine(GetUserProfileIE());
    }

    private IEnumerator GetUserProfileIE() {
        string url = m_sBaseURL + "/user/profile";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {m_sSessionToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            string json = request.downloadHandler.text;
            Debug.Log(json);
            ProfileResponse _oProfileResponse = JsonUtility.FromJson<ProfileResponse>(json);
            m_oTelegramUser = _oProfileResponse.data.user;
            IntroSceneController.Instance.OnLoadedUserData();
        }
        else {
            TelegramAuth();
        }
    }

    private void GetLeaderboardGlobal() {
        StartCoroutine(GetLeaderboardGlobalIE());
    }

    private IEnumerator GetLeaderboardGlobalIE() {
        string url = m_sBaseURL + "/leaderboard/global";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {m_sSessionToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            string json = request.downloadHandler.text;
            Debug.Log(json);
        }
        else {
            TelegramAuth();
        }
    }
    #endregion

}
