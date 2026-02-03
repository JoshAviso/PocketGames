using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class StatHolder
{
    public string Name;
    public Dictionary<string, int> Stats;
    [SerializeField] private List<StatContainer> StatReflector = new();

    public bool GetStat(string stat, out int val)
    {
        val = -1;
        if(!Stats.ContainsKey(stat)) return false;
        val = Stats[stat];
        UpdateReflector(stat, val);
        return true;
    }
    public bool SetStat(string stat, int val)
    {
        if(!Stats.ContainsKey(stat)) return false;
        Stats[stat] = val;
        UpdateReflector(stat, val);
        return true;
    }
    private void UpdateReflector(string stat, int val)
    {
        if(!Application.isEditor) return;

        StatContainer reflector = null;
        foreach(var reflect in StatReflector)
            if (reflect.StatName.Equals(stat))
            {
                reflector = reflect;
                break;
            }

        if(reflector == null)
        {
            reflector = new(){ StatName = stat };
            StatReflector.Add(reflector);
        }

        reflector.Val = val;
    }
}

public class StatsManager
{
    [SerializeField] private List<GameStat> _gameStats;

    private Dictionary<string, StatHolder> _statHolders = new();

    public static bool GetStat(string owner, string stat, out int val)
    {
        val = -1;
        if(!GetHolder(owner, out var holder)) return false;
        if (holder.GetStat(stat, out val)) return true;

        if(!StatExists(stat, out var gamestat)) return false;
        
        holder.Stats.Add(stat, gamestat.StartValue);
        val = gamestat.StartValue;
        return true;
    }
    public static bool SetStat(string owner, string stat, int val)
    {
        if(!GetHolder(owner, out var holder)) return false;
        if (holder.SetStat(stat, val)) return true;
        if(!StatExists(stat, out _)) return false;

        holder.Stats.Add(stat, val);
        return true;
    }

    public static bool AddStatHolder(string name, out StatHolder holder)
    {
        holder = null;
        if(String.IsNullOrEmpty(name)) return false;
        if(GetHolder(name, out _)) return false;

        holder = new(){ Name = name };
        foreach(var gamestat in Instance._gameStats)
            holder.Stats.Add(gamestat.Name, gamestat.StartValue);

        return true;
    }

    public static bool GetHolder(string name, out StatHolder holder)
    {
        holder = null;
        if(String.IsNullOrEmpty(name)) return false;
        if(!Instance._statHolders.ContainsKey(name)) return false;
        holder = Instance._statHolders[name];
        return true;
    }
    public static bool StatExists(string name, out GameStat stat)
    {
        stat = null;
        foreach(var gamestat in Instance._gameStats)
            if (name.Equals(gamestat.Name)) { stat = gamestat; return true; }
        return false;
    }

    public static List<string> GetStatList()
    {
        List<string> res = new();
        foreach(var stat in Instance._gameStats)
        {
            res.Add(stat.Name);
        }
        return res.Count <= 0 ? null : res;
    }

    // SINGLETON
    public static StatsManager Instance { 
        get {
            _instance ??= new();
            return _instance;
        }
    }
    private static StatsManager _instance = null;
}