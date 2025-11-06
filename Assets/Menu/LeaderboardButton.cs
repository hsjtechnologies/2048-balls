using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LeaderboardButton : MonoBehaviour
{
    [SerializeField]
    private GameObject Leaderboard;
    [SerializeField]
    private TMP_Text ButtonText;
    [SerializeField]
    private string btnTextDefault = "Leaderboard";
    [SerializeField]
    private string btnTextOnOpen = "Close";

    private UnityEvent<bool> CallbackEvent;


    public void OnToggle()
    {
        ButtonText.text = Leaderboard.activeSelf ? btnTextOnOpen : btnTextDefault;
    }
}
