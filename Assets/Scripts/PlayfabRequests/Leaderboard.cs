using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Core.DataModels;
using Cysharp.Threading.Tasks;

namespace PlayfabRequests
{
    public class Leaderboard
    {
        public async UniTask<(LoaderEnum state, string message)> SaveScore(int _score, int _level)
        {
            LoaderEnum loadState = LoaderEnum.Loading;
            string message = "Loading";

            PlayFabClientAPI.ExecuteCloudScript
            (
                new ExecuteCloudScriptRequest
                {
                    FunctionName = "UpdatePlayerStats",
                    FunctionParameter = new
                    {
                        score = _score,
                        level = _level
                    }
                },
                response =>
                {
                    message = "Successfull LeaderBoard data sent";
                    Debug.Log(message);
                    loadState = LoaderEnum.Loaded;
                },
                error =>
                {
                    message = "LeaderBoard update failed: " + error.ErrorMessage;
                    Debug.LogError(message);
                    loadState = LoaderEnum.Failed;
                }
            );

            await UniTask.WaitUntilValueChanged(this, x => loadState);
            return (loadState, message);
        }

        public async UniTask<GetLeaderboardResult> Get(string stat, int startPos, bool showStats)
        {
            GetLeaderboardResult _result = null;
            LoaderEnum loadState = LoaderEnum.Loading;
            Debug.Log("STAT: " + stat);
            Debug.Log("STARTPOS: " + startPos);
            Debug.Log("SHOWSTATS: " + showStats);
            PlayFabClientAPI.GetLeaderboard
            (
                new GetLeaderboardRequest
                {
                    StatisticName = stat,
                    StartPosition = startPos,
                    MaxResultsCount = 100,
                    ProfileConstraints = new PlayerProfileViewConstraints {
                        ShowDisplayName = true,
                    }
                },
                response =>
                {
                    if (response.Leaderboard.Count > 0)
                    {
                        _result = response;
                    }
                    else
                    {
                        Debug.LogWarning("Empty Leaderboard results");
                    }

                    loadState = LoaderEnum.Loaded;
                },
                error =>
                {
                    loadState = LoaderEnum.Failed;
                    Debug.LogError("Error fetching Leaderboard: " + error.ErrorMessage);
                }
            );

            await UniTask.WaitUntilValueChanged(this, x => loadState);
            return _result;
        }
    }
}
