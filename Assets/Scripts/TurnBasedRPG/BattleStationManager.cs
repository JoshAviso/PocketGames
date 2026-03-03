
using System.Collections.Generic;
using UnityEngine;

public interface IBattleUnit
{
    public enum MoveMode { Normal, Offscreen_To_Station, Station_To_Offscreen }
    public void Move(Transform from, Transform to, MoveMode move_mode = MoveMode.Normal);
}

public class BattleStationManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Transform> _offscreenStations = new();
    [SerializeField] private List<Transform> _battleStations = new();

    private Dictionary<uint, IBattleUnit> _activeUnits = new();
    private List<IBattleUnit> _reserveUnits = new();

    /// <param name="station_index">If a negative number is put, the unit is added to the reserve</param>
    /// <returns>True if adding unit was successful, otherwise, false</returns>
    public static bool AddUnit(IBattleUnit unit, int station_index, bool allow_overwrite = true, uint offscreen_start = 0)
    {
        if(unit == null) return false;
        if(station_index >= Instance._battleStations.Count) return false;
        if(station_index < 0)
        {
            Instance._reserveUnits.Add(unit);
            return true;
        }

        if(GetUnit((uint) station_index, out var otherUnit)){
            if(!allow_overwrite) return false;
            Instance._reserveUnits.Add(otherUnit);
        }

        Instance._activeUnits[(uint) station_index] = unit;

        if(offscreen_start >= Instance._offscreenStations.Count) offscreen_start = 0;
        Transform offscreenStation = null;
        if(Instance._offscreenStations.Count > 0) 
            offscreenStation = Instance._offscreenStations[(int) offscreen_start];

        unit.Move(offscreenStation, Instance._battleStations[station_index]);
        otherUnit?.Move(Instance._battleStations[station_index], offscreenStation);

        return true;
    }

    /// <returns>True if there was a unit to remove, otherwise, false</returns>
    public static bool RemoveUnit(uint station_index, out IBattleUnit unit, bool auto_reserve = true, uint offscreen_end = 0)
    {
        if(!GetUnit(station_index, out unit)) return false;
        
        Instance._activeUnits[station_index] = default;
        if(auto_reserve) Instance._reserveUnits.Add(unit);

        if(offscreen_end >= Instance._offscreenStations.Count) offscreen_end = 0;
        Transform offscreenStation = null;
        if(Instance._offscreenStations.Count > 0) 
            offscreenStation = Instance._offscreenStations[(int) offscreen_end];
        
        unit.Move(Instance._battleStations[(int) station_index], offscreenStation, IBattleUnit.MoveMode.Station_To_Offscreen);

        return true;
    }

    /// <returns>True if there was a unit to remove, otherwise, false</returns>
    public static bool RemoveReserveUnit(uint index, out IBattleUnit unit)
    {
        unit = default;
        if(index >= Instance._reserveUnits.Count) return false;
        bool res = GetUnit(index, out unit, true);
        Instance._reserveUnits.RemoveAt((int) index);
        return res;   
    }

    /// <returns>True if move is valid, otherwise, false</returns>
    public static bool MoveUnit(uint unit_index, uint end_station, out IBattleUnit unit, bool from_reserve = false, bool allow_switching = true)
    {
        unit = default;
        if(end_station >= Instance._battleStations.Count) return false;
        if (!from_reserve && !GetUnit(unit_index, out unit)) return false;
        else {
            if(unit_index >= Instance._reserveUnits.Count) return false;
            unit = Instance._reserveUnits[(int) unit_index];
        }
        if(unit == null) return false;

        if (GetUnit(end_station, out var otherUnit) && !allow_switching) return false;

        if(!from_reserve) Instance._activeUnits[unit_index] = otherUnit;
        else Instance._reserveUnits.RemoveAt((int) unit_index);

        Instance._activeUnits[end_station] = unit;

        unit?.Move(Instance._battleStations[(int) unit_index], Instance._battleStations[(int) end_station]);
        otherUnit?.Move(Instance._battleStations[(int) end_station], Instance._battleStations[(int) unit_index]);

        return true;
    }
    
    /// <returns>True if a unit exists at station, otherwise, false</returns>
    public static bool GetUnit(uint index, out IBattleUnit unit, bool fromReserve = false)
    {
        unit = default;
        if (!fromReserve){
            if(index >= Instance._battleStations.Count) return false;
            if(!Instance._activeUnits.ContainsKey(index)) return false;
            unit = Instance._activeUnits[index];
        } else {
            if(index >= Instance._reserveUnits.Count) return false;
            unit = Instance._reserveUnits[(int)index];
        }

        return unit != null;
    }

    public static BattleStationManager Instance { get; private set; }
    protected virtual void Awake(){
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
        }
    }

}
