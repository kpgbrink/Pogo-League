using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ObjTeamList
{
    [SerializeField]
    bool removeIfOnTeam;
    [SerializeField]
    bool removeIfNotOnTeam;
    [SerializeField]
    Transform[] transformList;
    [SerializeField]
    Transform[] TransformList { get; set; }

    public void Init()
    {
        if (TransformList != null) return;
        TransformList = (Transform[]) transformList.Clone();
    }

    public IEnumerable<Transform> FilteredTransformList(int? team)
    {
        return transformList.ToList().Where(t =>
        {
            var objTeam = t.GetComponent<ObjTeam>();
            if (objTeam != null && team != null)
            {
                if (objTeam.TeamNumber == team && removeIfOnTeam) return false;
                if (objTeam.TeamNumber != team && removeIfNotOnTeam) return false;
            }
            return true;
        }).ToArray();
    }
}
