using System;
using System.Collections.Generic;
using UnityEngine;

public static class UnitRegistry
{
    private static readonly List<Unit> unitList = new List<Unit>();

    public static IReadOnlyList<Unit> UnitList => unitList;

    public static event Action<Unit> OnUnitRegistered;
    public static event Action<Unit> OnUnitUnregistered;

    public static void Register(Unit unit)
    {
        if (unit != null && !unitList.Contains(unit))
        {
            unitList.Add(unit);
            OnUnitRegistered?.Invoke(unit);
        }
    }

    public static void Unregister(Unit unit)
    {
        if (unit != null && unitList.Contains(unit))
        {
            unitList.Remove(unit);
            OnUnitUnregistered?.Invoke(unit);
        }
    }

    public static List<Unit> GetAliveUnits(int teamId)
    {
        List<Unit> result = new List<Unit>();
        for (int i = 0; i < unitList.Count; i++)
        {
            Unit u = unitList[i];
            if (u != null && u.gameObject.activeInHierarchy && u.Stats.TeamID == teamId && !u.Stats.IsDead())
            {
                result.Add(u);
            }
        }
        return result;
    }

    public static void Clear()
    {
        unitList.Clear();
    }
}
