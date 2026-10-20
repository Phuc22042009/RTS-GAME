using System;
using UnityEngine;

public class UnitPerception
{
    private readonly Unit owner;

    public Unit CurrentTarget { get; private set; }

    public event Action<Unit> OnTargetAcquired;
    public event Action OnTargetLost;

    public UnitPerception(Unit owner)
    {
        this.owner = owner;
    }

    public Unit FindNearestEnemy()
    {
        Unit nearest = null;
        float minDistance = float.MaxValue;
        Vector2 ownerPos = owner.transform.position;

        foreach (Unit candidate in UnitRegistry.UnitList)
        {
            if (!IsValidEnemy(candidate))
                continue;

            float distance = Vector2.Distance(ownerPos, candidate.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = candidate;
            }
        }

        SetTarget(nearest);
        return nearest;
    }

    public bool IsValidEnemy(Unit candidate)
    {
        if (candidate == null || candidate == owner)
            return false;

        if (!candidate.gameObject.activeInHierarchy)
            return false;

        if (candidate.Stats.TeamID == owner.Stats.TeamID)
            return false;

        if (candidate.Health != null && candidate.Health.IsDead)
            return false;

        if (candidate.Stats.IsDead())
            return false;

        return true;
    }

    public bool IsTargetInAttackRange(Unit target, float range)
    {
        if (!IsValidEnemy(target))
            return false;

        float dist = Vector2.Distance(owner.transform.position, target.transform.position);
        return dist <= range;
    }

    public void SetTarget(Unit newTarget)
    {
        if (CurrentTarget != newTarget)
        {
            CurrentTarget = newTarget;
            if (CurrentTarget != null)
                OnTargetAcquired?.Invoke(CurrentTarget);
            else
                OnTargetLost?.Invoke();
        }
    }

    public void ClearTarget()
    {
        SetTarget(null);
    }
}
