using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Core.DataModels;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace PlayfabRequests
{
    public class Account
    {
        public async UniTask<(LoaderEnum state, string message, string SessionTicket)> InitializePlayfab(string username)
        {
            LoaderEnum loadState = LoaderEnum.Loading;
            string message = "Loading";
            string playfabId = null;

            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
                    CustomId = username + "Guest",
                    CreateAccount = true,
                    CustomTags = new Dictionary<string, string>
                    {
                        {"AccType", "AnonGuest"}
                    }
                },
                response =>
                {
                    playfabId = response.PlayFabId;
                    message = "Guest Login Success: " + playfabId;
                    Debug.Log(message);
                    loadState = LoaderEnum.Loaded;
                },
                error =>
                {
                    message = "GUEST LOGIN FAILED: " + error.ErrorMessage;
                    Debug.LogWarning(message);
                    loadState = LoaderEnum.Failed;
                }
            );

            await UniTask.WaitUntilValueChanged(this, x => loadState);
            return (loadState, message, playfabId);
        }

        public async UniTask<(LoaderEnum state, string message, string playfabId)> AccountCreate(string username, string password)
        {
            LoaderEnum loadState = LoaderEnum.Loading;
            string message = "Loading";
            string playfabId = null;

            Debug.Log("TitleId: " + PlayFabSettings.TitleId);
            Debug.Log("Username: " + username);
            Debug.Log("Password: " + password);

            PlayFabClientAPI.RegisterPlayFabUser(
                new RegisterPlayFabUserRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    Username = username,
                    Password = password,
                    RequireBothUsernameAndEmail = false,
                    CustomTags = new Dictionary<string, string>
                    {
                        {"guestLink", password}
                    }
                },
                response =>
                {
                    PlayFabClientAPI.UpdateUserTitleDisplayName
                    (
                        new UpdateUserTitleDisplayNameRequest
                        {
                            DisplayName = username
                        },
                        response => Debug.Log("USER TITLE DISPLAYNAME UPDATED"),
                        error => Debug.LogWarning("ERROR UPDATING USER TITLE DISPLAY NAME: " + error.ErrorMessage)
                    );

                    message = "New User Created + " + response.PlayFabId;
                    playfabId = response.PlayFabId;
                    Debug.Log(message);
                    loadState = LoaderEnum.Loaded;
                },
                error =>
                {
                    message = "SING UP ERROR: " + error.ErrorMessage;
                    Debug.LogWarning(message);
                    loadState = LoaderEnum.Failed;
                }
            );

            await UniTask.WaitUntilValueChanged(this, x => loadState);
            return (loadState, message, playfabId);
        }

        public async UniTask<(LoaderEnum state, string message, string playFabId, string username)> AccountLogIn(string username, string password)
        {
            LoaderEnum loadState = LoaderEnum.Loading;
            string message = "Loading";
            string playfabId = null;
            string _username = null;
            PlayFabClientAPI.LoginWithPlayFab
            (
                new LoginWithPlayFabRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    Username = username,
                    Password = password,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetUserAccountInfo = true
                    }
                },
                response =>
                {
                    playfabId = response.InfoResultPayload.AccountInfo.PlayFabId;
                    _username = response.InfoResultPayload.AccountInfo.Username;
                    message = "Successful Sign In";
                    loadState = LoaderEnum.Loaded;
                },
                error =>
                {
                    message = "SIGN In ERROR: " + error.ErrorDetails;
                    Debug.LogWarning(message);
                    loadState = LoaderEnum.Failed;
                }
            );

            await UniTask.WaitUntilValueChanged(this, x => loadState);
            return (loadState, message, playfabId, _username);
        }

    }
}
