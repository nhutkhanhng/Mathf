using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[System.Serializable]
public class DataAccount
{
    public string email;
    public string password;
}

[System.Serializable]
public class RequestUpdateData
{
    public int user_id;
    public int points;
}
[System.Serializable]
public class SignInRespone
{
    public string access_token;
    public string token_type;
    public string expires_in;
    public string id;
    public string name;
    // public string points;
}

[System.Serializable]
public class UserInfo
{
    public int id;
    public string name;
    public int points;
}

[System.Serializable]
public class SessionInfo
{
    public string AccessToken;
    public string TokenType;
    public string ExpiresIn;
}
public class GameMgr : Singleton<GameMgr>
{

    // public
    public static Action EVENT_RESET_INGAME;
    public GameState gameState = GameState.None;


    // DI
    [Inject] private GameView _gameView;
    [Inject] protected ViewUserInfo _userInfoView;
    [Inject] protected ViewInGame _viewInGame;
    // properties
    public bool IsInGameState => gameState == GameState.InGame;


    protected int _totalCoin = 0;
    public int TotalCoin { get { return _totalCoin; } set { _totalCoin = value; } }

    protected int CoihnColleceted = 0;
    protected string Authorization => $"{sessionInfo.TokenType} {sessionInfo.AccessToken}";

    protected const string URL = @"https://admin.mysuperkids.io";
    public const string SignInMethod = URL + "/api/v1/auth/sign-in";
    public const string URI_SIGNIN= URL + "/api/v1/auth/sign-in";

    protected const string URI_UPDATECOIN = @"/api/v1/accounts/update-coins";

    protected string URI_UpdateCoin => URL + URI_UPDATECOIN;

    public UserInfo userInfo = new UserInfo();
    public SessionInfo sessionInfo = new SessionInfo();

    public void SetSignInInfo(string signInRespone)
    {
        SignInRespone res = JsonUtility.FromJson<SignInRespone>(signInRespone) as SignInRespone;

        this.userInfo.id = Int32.Parse(res.id);
        this.userInfo.name = res.name;
        // this.userInfo.points = Int32.Parse(res.points);

        sessionInfo.AccessToken = res.access_token;
        sessionInfo.TokenType = res.token_type;
        sessionInfo.ExpiresIn = res.expires_in;


        RestClient.DefaultRequestHeaders["Authorization"] = Authorization;

        Collected(0, InitGame);

        // InitGame();
    }
    
    public void Collected(int coin, System.Action callback =null)
    {
        var _getInfoRquest = new RequestHelper
        {
            Uri = URI_UpdateCoin,
            Headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {sessionInfo.AccessToken}" },
                {"Accept", "application/json"  },
                {"Content-Type", "application/json" },
            },
            Body = new RequestUpdateData()
            {
                user_id = this.userInfo.id,
                points = coin,
            }
        };

        RestClient.Put(_getInfoRquest).Then(_updateCoinRes =>
        {
            Debug.Log(_updateCoinRes.Text);

            var result = MiniJSON.Json.Deserialize(_updateCoinRes.Text) as Dictionary<string, object>;
            object _data;
            int _coinReceiveFromServer = TotalCoin;
            if (result.TryGetValue("data", out _data))
            {
                bool _parsed = int.TryParse(_data.ToString(), out _coinReceiveFromServer);
                if (_parsed)
                {
                    if (_coinReceiveFromServer >= TotalCoin)
                    {
                        TotalCoin = _coinReceiveFromServer;
                        userInfo.points = _coinReceiveFromServer;
                    }

                }
            }

            callback?.Invoke();
        }).Catch(err => Debug.LogError("Error " + err.Message));

    }

    public void SignIN(System.Action callback = null)
    {
        var _currentRquest = new RequestHelper
        {
            Uri = SignInMethod,
            Body = new DataAccount
            {
                email = @"ngan.nguyen@eastplayers.io",
                password = @"eastplayers@0407"
            },
        };

        RestClient.Post(_currentRquest).Then(res => {

            // And later we can clear the default query string params for all requests
            RestClient.ClearDefaultParams();

            var result = MiniJSON.Json.Deserialize(res.Text) as Dictionary<string, object>;

            object _accountInfo;
            if (result.TryGetValue("data", out _accountInfo))
            {

                string _r = MiniJSON.Json.Serialize(result["data"]);
                Debug.LogError(_r);

                SetSignInInfo(_r);

                //Dictionary<string, object> _dataAccount = (Dictionary<string, object>)(_accountInfo);

                //if (_dataAccount.ContainsKey("access_token"))
                //{
                //    sessionInfo.AccessToken = _dataAccount["access_token"].ToString();
                //}

                //if (_dataAccount.ContainsKey("token_type"))
                //{
                //    sessionInfo.TokenType = _dataAccount["token_type"].ToString();
                //}

                //if (_dataAccount.ContainsKey("token_type"))
                //{
                //    sessionInfo.ExpiresIn = _dataAccount["expires_in"].ToString();
                //}

                //if (_dataAccount.ContainsKey("id"))
                //{
                //    Int32.TryParse(_dataAccount["id"].ToString(), out userInfo.id);
                //}

                //if (_dataAccount.ContainsKey("name"))
                //{
                //    userInfo.name = _dataAccount["name"].ToString();
                //}

                RestClient.DefaultRequestHeaders["Authorization"] = Authorization;

                callback?.Invoke();
            }
        }).Catch(err => Debug.LogError("Error " + err.Message));
    }
    #region UNITY
    private void Start()
    {
#if UNITY_EDITOR
        SignIN();
#else

#endif
    }

    // private void Update()
    // {
    // }
    #endregion



    private void InitGame()
    {
        _userInfoView.Receive(this.userInfo);

        CoihnColleceted = 0;
        SetState(gameState);
        SoundMgr.PlayMusic(SoundMgr.MUSIC_BACKGROUND);
    }


    public void PlayGame()
    {
        CoihnColleceted = 0;
        SetState(GameState.InGame);
        GameScene.Instance.InitScene();
    }


    public void ReplayGame()
    {
        CoihnColleceted = 0;
        GameScene.Instance.ResetGame();
        SetState(GameState.InGame);
        SoundMgr.PlayMusic(SoundMgr.MUSIC_BACKGROUND);
    }


    public void GameOver()
    {
        SetState(GameState.GameOver);
        SoundMgr.Instance.PlaySFX(SoundMgr.SFX_GAMEOVER);
    }


    public void SetState(GameState newState)
    {
        gameState = newState;

        switch (gameState)
        {
            case GameState.Loading: _gameView.SetStateView("Loading"); break;
            // case GameState.Info: _gameView.SetStateView("Info"); break;
            case GameState.Menu: _gameView.SetStateView("Menu"); break;
            case GameState.InGame: _gameView.SetStateView("InGame"); break;
            case GameState.GameOver: _gameView.SetStateView("GameOver"); break;
            case GameState.Setting: _gameView.SetStateView("Setting"); break;
            case GameState.None: _gameView.SetStateView("None"); break;
        }
    }




}



// God bless my code to be bug free 
//
//                       _oo0oo_
//                      o8888888o
//                      88" . "88
//                      (| -_- |)
//                      0\  =  /0
//                    ___/`---'\___
//                  .' \\|     |// '.
//                 / \\|||  :  |||// \
//                / _||||| -:- |||||- \
//               |   | \\\  -  /// |   |
//               | \_|  ''\---/''  |_/ |
//               \  .-\__  '-'  ___/-. /
//             ___'. .'  /--.--\  `. .'___
//          ."" '<  `.___\_<|>_/___.' >' "".
//         | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//         \  \ `_.   \_ __\ /__ _/   .-` /  /
//     =====`-.____`.___ \_____/___.-`___.-'=====
//                       `=---='
//
//
//     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
//               佛祖保佑         永无BUG
//