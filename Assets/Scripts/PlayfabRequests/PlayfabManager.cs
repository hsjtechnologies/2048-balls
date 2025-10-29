using UnityEngine;
using Core.Singleton;
using Core.DataModels;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using PlayFab.ClientModels;

namespace PlayfabRequests
{
    public class PlayfabManager : MonoBehaviour // PersistentSingleton<PlayfabManager>
    {
        public static PlayfabManager Instance { get; private set; }

        private Account _account = new Account();
        private PlayerData _playerData = new PlayerData();
        private Leaderboard _leaderboard = new Leaderboard();

        private string _initPlayfabId = null;
        private string _playfabId = null;
        private string _playerSuiWalletAddr = null;
        public string InitPlayfabId { get => _initPlayfabId; }
        public string PlayfabId { get => _playfabId; }
        public string PlayerSuiWalletAddr { get => _playerSuiWalletAddr; set => _playerSuiWalletAddr = value; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            Debug.Log("Initialize PlayfabManager");
        }

        public async UniTask InitLogin(string username)
        {

            var (initState, initMsg, initPlayfabId) = await _account.InitializePlayfab(username);
            _initPlayfabId = initPlayfabId;

            if (initState == LoaderEnum.Loaded)
            {
                var (signInState, signInMsg, signInPlayfabId, signInUsername) = await _account.AccountLogIn(username, _initPlayfabId);
                if (signInState == LoaderEnum.Loaded)
                {
                    Debug.Log("Succesfull Logged in: " + signInPlayfabId);
                    _playfabId = signInPlayfabId;
                }
                else if (signInState == LoaderEnum.Failed)
                {
                    var (signUpState, signUpMsg, signUpPlayfabId) = await _account.AccountCreate(username, _initPlayfabId);
                    _playfabId = signUpPlayfabId;
                }
            }
        }

        public async UniTask<(LoaderEnum loadState, string playerSuiWalletAddr)> GetWalletAddr()
        {
            LoaderEnum loadState = LoaderEnum.Loading;
            (loadState, _playerSuiWalletAddr) = await _playerData.GetPlayerWallet();
            await UniTask.WaitUntil(() => loadState != LoaderEnum.Loading);
            return (loadState, _playerSuiWalletAddr);
        }

        public async void SignUp(string username, string password) => await _account.AccountCreate(username, password);
        public async void SignIn(string username, string password) => await _account.AccountLogIn(username, password);
        public async UniTask<(LoaderEnum state, string message)> SavePlayerWallet(string walletAddress) => await _playerData.SavePlayerWallet(walletAddress);
        public async void SaveScore(int score, int level) => await _leaderboard.SaveScore(score, level);
        public async UniTask<GetLeaderboardResult> GetLeaderBoard(string stat, int startPos, bool showStats = false) => await _leaderboard.Get(stat, startPos, showStats);
    }
}
