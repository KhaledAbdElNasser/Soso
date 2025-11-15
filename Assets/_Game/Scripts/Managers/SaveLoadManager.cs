using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private const string MatchDataKey = "MatchData";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Save a new match
    public void SaveMatch(string match, int moves, float elapsedTime)
    {
        MatchData data = LoadAllMatches() ?? new MatchData();

        data.matches.Add(new MatchRecord
        {
            matches = match,
            movesAtMatch = moves,
            time = elapsedTime
        });

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(MatchDataKey, json);
        PlayerPrefs.Save();
    }

    // Load all matches
    public MatchData LoadAllMatches()
    {
        if (!PlayerPrefs.HasKey(MatchDataKey))
            return null;

        string json = PlayerPrefs.GetString(MatchDataKey);
        return JsonUtility.FromJson<MatchData>(json);
    }

    // Optional: clear all saved matches
    public void ClearAllMatches()
    {
        PlayerPrefs.DeleteKey(MatchDataKey);
        PlayerPrefs.Save();
    }
}
