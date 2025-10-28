using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Core.DataModels;
using Cysharp.Threading.Tasks;

namespace PlayfabRequests
{
    public class PlayerData
    {
        public async UniTask<(LoaderEnum state, string message)> SavePlayerWallet(string walletAddr)
        {
            LoaderEnum loadState = LoaderEnum.Loading;
            string message = "Loading";

            PlayFabClientAPI.ExecuteCloudScript
            (
                new ExecuteCloudScriptRequest
                {
                    FunctionName = "UpdatePlayerSuiWallet",
                    FunctionParameter = new
                    {
                        walletAddress = walletAddr,
                    },
                    GeneratePlayStreamEvent = true
                },
                response =>
                {
                    message = "Wallet Address Successfully Saved";
                    Debug.Log(message);
                    loadState = LoaderEnum.Failed;
                },
                error =>
                {
                    message = "Error saving wallet address: " + error.ErrorMessage;
                    Debug.LogWarning(message);
                    loadState = LoaderEnum.Failed;
                }
            );

            await UniTask.WaitUntilValueChanged(this, x => loadState);
            return (loadState, message);
        }
    }
}
