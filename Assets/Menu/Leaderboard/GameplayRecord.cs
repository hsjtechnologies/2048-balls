using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayRecord : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _rankText;

    // [SerializeField]
    // private Image _rankIcon;

    // [SerializeField]
    // private Sprite[] _rankIcons;

    [Space]
    [SerializeField]
    private TextMeshProUGUI _playerName;

    // [SerializeField]
    // private TextMeshProUGUI _levelReached;

    [SerializeField]
    private TextMeshProUGUI _score;

    public void SetRecord(int rank, string player, int score) //int level
    {
        SetRank(rank);
        _playerName.text = player;
        // _levelReached.text = level.ToString();
        _score.text = score.ToString();
    }

    private void SetRank(int rank)
    {
        _rankText.text = rank.ToString();
        // if (rank <= 3)
        // {
        //     _rankIcon.gameObject.SetActive(true);
        //     _rankIcon.sprite = _rankIcons[rank - 1];
        // }
        // else
        // {
        //     _rankText.text = rank.ToString();
        // }
    }
}
