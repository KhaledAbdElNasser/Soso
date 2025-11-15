using TMPro;
using UnityEngine;

public class BindSingleUIElementData : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI matchesText;

    public void BindData(MatchRecord data)
    {
        if (data == null) return;

        movesText.text = data.movesAtMatch.ToString();
        timerText.text = Mathf.FloorToInt(data.time) + "s";
        matchesText.text = data.matches;

    }
}
