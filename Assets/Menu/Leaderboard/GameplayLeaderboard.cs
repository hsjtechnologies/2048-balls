using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayfabRequests;
using UnityEngine;
using UnityEngine.UI;

public class GameplayLeaderboard : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameplayRecords;

    [SerializeField]
    private GameplayRecord _gameplayRecordPrefab;

    [SerializeField]
    private GameObject _spinner;

    [SerializeField]
    private Button _btnNext;

    [SerializeField]
    private Button _btnPrev;

    private int _lbStartPos = 0;

    private void OnEnable()
    {
        GetLB(_lbStartPos);
    }

    private void OnDisable()
    {
        _lbStartPos = 0;
        ClearLeaderboard();
        _btnNext.interactable = true;
        _btnPrev.interactable = false;
    }

    private async void GetLB(int startPos)
    {
        ClearLeaderboard();
        _spinner.SetActive(true);
        PopulateLeaderboard(await PlayfabManager.Instance.GetLeaderBoard("Score_Max", startPos, true));
        _spinner.SetActive(false);
    }

    private void PopulateLeaderboard(GetLeaderboardResult result)
    {
        int rank = _lbStartPos == 0 ? 1 : (_lbStartPos + 1);


        if (result != null && result.Leaderboard.Count > 0)
        {
            foreach (PlayerLeaderboardEntry entry in result.Leaderboard)
            {
                GameplayRecord record = Instantiate(_gameplayRecordPrefab, _gameplayRecords.transform);

                List<StatisticModel> data = entry.Profile.Statistics;

                record.SetRecord(rank,
                                entry.DisplayName,
                                //  FindData(data, "Level_max", 0),
                                entry.StatValue
                                );

                rank++;
            }
        }
    }

    public void NextPage()
    {
        _lbStartPos += 100;
        Debug.Log($"_lbStartPos: {_lbStartPos}");

        GetLB(Mathf.Clamp(_lbStartPos, 0, 400));

        if(_lbStartPos >= 400)
        {
            _btnNext.interactable = false;
        }
        else
        {
            _btnNext.interactable = true;
        }

        if(_lbStartPos > 0)
        {
            _btnPrev.interactable = true;
        }
    }

    public void PrevPage()
    {
        _lbStartPos -= 100;
        Debug.Log($"_lbStartPos: {_lbStartPos}");

        GetLB(Mathf.Clamp(_lbStartPos, 0, 400));

        if(_lbStartPos < 400)
        {
            _btnNext.interactable = true;
        }

        if(_lbStartPos <= 0)
        {
            _btnPrev.interactable = false;
        }
    }

    private int FindData(List<StatisticModel> data, string name, int version)
    {
        StatisticModel fetchedData = data.Find(data => data.Name == name && data.Version == version);

        if (fetchedData != null)
        {
            return fetchedData.Value;
        }

        return 0;
    }

    private void ClearLeaderboard()
    {
        if (_gameplayRecords.transform.childCount <= 0)
        {
            return;
        }

        GameplayRecord[] records = _gameplayRecords.GetComponentsInChildren<GameplayRecord>();

        foreach (GameplayRecord record in records)
        {
            Destroy(record.gameObject);
        }
    }
}
