using System.Collections.Generic;
using UnityEngine;

public class BindSavedData : MonoBehaviour
{
    [Header("Prefabs")]
    public BindSingleUIElementData entryUIViewerPrefab;
    
    [Header("UI Containers")]
    public Transform UiContainer;


    public void ListSavedData()
    {
        MatchData data = SaveLoadManager.Instance.LoadAllMatches();

        if (data == null)
            return;

        foreach (Transform t in UiContainer)
            Destroy(t.gameObject);

        data.matches.ForEach(m =>
        {
            BindSingleUIElementData uiEntry = Instantiate(entryUIViewerPrefab, UiContainer);
            uiEntry.BindData(m);
        });
    }

}
