using UnityEngine;

[System.Serializable]
public struct DamageInfo
{
    public float amount;
    public GameObject source;
    public bool isCritical;

    public DamageInfo(float amount, GameObject source = null, bool isCritical = false)
    {
        this.amount = amount;
        this.source = source;
        this.isCritical = isCritical;
    }
}
