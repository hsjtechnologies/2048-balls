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

        public async UniTask<(LoaderEnum state, string walletAddress)> GetPlayerWallet()
        {
            LoaderEnum loadState = LoaderEnum.Loading;
            string walletAddr = null;

            PlayFabClientAPI.ExecuteCloudScript
            (
                new ExecuteCloudScriptRequest
                {
                    FunctionName = "GetPlayerWallet",
                    GeneratePlayStreamEvent = true
                },
                response =>
                {
                    Debug.Log("SUCCESS FULLY RETREIVED WALLET DATA");
                    var serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
                    var result = serializer.DeserializeObject<GetUserDataResult>(response.FunctionResult.ToString());

                    if (result.Data.Count > 0 && result.Data.ContainsKey("SUIWallet"))
                    {
                        Debug.Log("PLAYER SUIWallet: " + result.Data["SUIWallet"].Value);
                        walletAddr = result.Data["SUIWallet"].Value;
                        loadState = LoaderEnum.Loaded;
                    }
                    else
                    {
                        Debug.LogWarning("PLAYER WALLET DATA EMPTY!");
                        loadState = LoaderEnum.Empty;
                    }
                },
                error => Debug.LogWarning("ERROR FETCHING PLAYER WALLET DATA: " + error.ErrorMessage)
            );

            await UniTask.WaitUntilValueChanged(this, x => loadState);
            return (loadState, walletAddr);
        }
    }
}
