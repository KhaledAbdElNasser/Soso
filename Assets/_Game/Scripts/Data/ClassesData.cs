using System.Collections.Generic;
using System;
using UnityEngine;

public class ClassesData : MonoBehaviour
{
}

[Serializable]
public class MatchRecord
{
    public string matches;
    public int movesAtMatch;
    public float time;
}

[Serializable]
public class MatchData
{
    public List<MatchRecord> matches = new List<MatchRecord>();
}

