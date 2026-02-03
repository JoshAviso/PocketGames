
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatHolderComponent : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private List<StatContainer> _statStartValues;
    [SerializeField] private StatHolder _holder;

    void Awake()
    {
        if(!StatsManager.AddStatHolder(_name, out _holder)) { Destroy(this); return; }
        foreach(var startStat in _statStartValues)
            _holder.SetStat(startStat.StatName, startStat.Val);
    }
}
