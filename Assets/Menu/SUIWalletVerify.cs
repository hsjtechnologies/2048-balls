using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using TMPro;
using PlayfabRequests;
using System.Threading.Tasks;
using Core.DataModels;
using Cysharp.Threading.Tasks;

public class SUIWalletVerify : MonoBehaviour
{
    [SerializeField]
    private GameObject SuiWalletBG;
    [SerializeField]
    private GameObject SuiInputs;
    [SerializeField]
    private GameObject Spinner;
    [SerializeField]
    private TMP_Text ErrorText;
    [SerializeField]
    private GameObject SavingText;
    [SerializeField]
    private TMP_Text SuiWalletText;

    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private string prefix = "Wallet: ";
    [SerializeField] private string suffix = "";
    [SerializeField] private bool shortenAddress = true;
    [SerializeField] private int startChars = 6; // Characters to show at start (e.g., "0x8f17")
    [SerializeField] private int endChars = 4;  // Characters to show at end (e.g., "7248")
    [SerializeField] private Color validColor = Color.green;

    private async void Start()
    {
        Spinner.SetActive(true);

        await UniTask.WaitUntil(() => !string.IsNullOrWhiteSpace(PlayfabManager.Instance.PlayfabId) && !string.IsNullOrEmpty(PlayfabManager.Instance.PlayfabId));
        var (loadState, playerSuiWalletAddr) = await PlayfabManager.Instance.GetWalletAddr();

        if (loadState == LoaderEnum.Loaded)
        {
            Debug.Log("PLAYER WALLET DATA ALREADY EXISTS");
            SuiWalletBG.SetActive(false);
            targetText.color = validColor;
            string displayText = FormatAddressForDisplay(playerSuiWalletAddr);
            targetText.text = prefix + displayText + suffix;
            GameManager.Instance.CompleteSUILogin();
            PlayfabManager.Instance.PlayerSuiWalletAddr = playerSuiWalletAddr;
        }
        else
        {
            Debug.Log("PLAYER WALLET DATA EMPTY");
            SuiWalletBG.SetActive(true);
            SuiInputs.SetActive(true);
        }
        Spinner.SetActive(false);
    }

    private static bool IsValidSuiAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return false;

        // Basic pattern: starts with 0x and followed by exactly 64 hex characters
        string pattern = @"^0x[a-fA-F0-9]{64}$";
        return Regex.IsMatch(address, pattern);
    }

    private string FormatAddressForDisplay(string address)
    {
        if (string.IsNullOrEmpty(address))
            return address;
        
        // If shortening is disabled or address is too short, return as is
        if (!shortenAddress || address.Length < (startChars + endChars))
            return address;
        
        // Extract start and end parts
        string startPart = address.Substring(0, startChars);
        string endPart = address.Substring(address.Length - endChars);
        
        // Return formatted address with ellipsis
        return startPart + "..." + endPart;
    }

    // public async Task VerifyText()
    // {
    //     if (string.IsNullOrEmpty(SuiWalletText.text))
    //     {
    //         ErrorText.text = "Input is empty!";
    //         ErrorText.gameObject.SetActive(true);
    //         return;
    //     }
    //     if (string.IsNullOrWhiteSpace(SuiWalletText.text))
    //     {
    //         ErrorText.text = "Input cannat be spaces!";
    //         ErrorText.gameObject.SetActive(true);
    //         return;
    //     }
    //     if (IsValidSuiAddress(SuiWalletText.text))
    //     {
    //         ErrorText.gameObject.SetActive(false);
    //         SuiInputs.SetActive(false);
    //         SavingText.SetActive(true);
    //         Spinner.SetActive(true);

    //         var (loadState, message) = await PlayfabManager.Instance.SavePlayerWallet(SuiWalletText.text);

    //         if (loadState != LoaderEnum.Loaded)
    //         {
    //             ErrorText.text = message;
    //             ErrorText.gameObject.SetActive(true);
    //         }
    //         else
    //         {
    //             PlayfabManager.Instance.PlayerSuiWalletAddr = SuiWalletText.text;
    //             SuiWalletBG.SetActive(false);
    //         }

    //         Spinner.SetActive(false);
    //     }
    //     else
    //     {
    //         ErrorText.text = "Input is not a valid SUI Address!";
    //         ErrorText.gameObject.SetActive(true);
    //     }
    // }

}
