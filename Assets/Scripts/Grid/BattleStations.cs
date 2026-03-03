
using System.Collections.Generic;
using UnityEngine;

public interface IStationUnit
{
    public enum MoveMode { Normal, Offscreen_To_Station, Station_To_Offscreen }
    public void Move(Transform from, Transform to, MoveMode move_mode = MoveMode.Normal);
}

public class BattleStations : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Transform> _offscreenStations = new();
    [SerializeField] private List<Transform> _battleStations = new();

    private Dictionary<uint, IStationUnit> _activeUnits = new();
    private List<IStationUnit> _reserveUnits = new();

    /// <param name="station_index">If a negative number is put, the unit is added to the reserve</param>
    /// <returns>True if adding unit was successful, otherwise, false</returns>
    public bool AddUnit(IStationUnit unit, int station_index, bool allow_overwrite = true, uint offscreen_start = 0)
    {
        if(unit == null) return false;
        if(station_index >= _offscreenStations.Count) return false;
        if(station_index < 0)
        {
            _reserveUnits.Add(unit);
            return true;
        }

        if(GetUnit((uint) station_index, out var otherUnit)){
            if(!allow_overwrite) return false;
            _reserveUnits.Add(otherUnit);
        }

        _activeUnits[(uint) station_index] = unit;

        if(offscreen_start >= _offscreenStations.Count) offscreen_start = 0;
        Transform offscreenStation = null;
        if(_offscreenStations.Count > 0) 
            offscreenStation = _offscreenStations[(int) offscreen_start];

        unit.Move(offscreenStation, _battleStations[station_index]);
        otherUnit?.Move(_battleStations[station_index], offscreenStation);

        return true;
    }

    /// <returns>True if there was a unit to remove, otherwise, false</returns>
    public bool RemoveUnit(uint station_index, out IStationUnit unit, bool auto_reserve = true, uint offscreen_end = 0)
    {
        if(!GetUnit(station_index, out unit)) return false;
        
        _activeUnits[station_index] = default;
        if(auto_reserve) _reserveUnits.Add(unit);

        if(offscreen_end >= _offscreenStations.Count) offscreen_end = 0;
        Transform offscreenStation = null;
        if(_offscreenStations.Count > 0) 
            offscreenStation = _offscreenStations[(int) offscreen_end];
        
        unit.Move(_battleStations[(int) station_index], offscreenStation, IStationUnit.MoveMode.Station_To_Offscreen);

        return true;
    }

    /// <returns>True if there was a unit to remove, otherwise, false</returns>
    public bool RemoveReserveUnit(uint index, out IStationUnit unit)
    {
        unit = default;
        if(index >= _reserveUnits.Count) return false;
        bool res = GetUnit(index, out unit, true);
        _reserveUnits.RemoveAt((int) index);
        return res;   
    }

    /// <returns>True if move is valid, otherwise, false</returns>
    public bool MoveUnit(uint unit_index, uint end_station, out IStationUnit unit, bool from_reserve = false, bool allow_switching = true)
    {
        unit = default;
        if(end_station >= _battleStations.Count) return false;
        if (!from_reserve && !GetUnit(unit_index, out unit)) return false;
        else {
            if(unit_index >= _reserveUnits.Count) return false;
            unit = _reserveUnits[(int) unit_index];
        }
        if(unit == null) return false;

        if (GetUnit(end_station, out var otherUnit) && !allow_switching) return false;

        if(!from_reserve) _activeUnits[unit_index] = otherUnit;
        else _reserveUnits.RemoveAt((int) unit_index);

        _activeUnits[end_station] = unit;

        unit?.Move(_battleStations[(int) unit_index], _battleStations[(int) end_station]);
        otherUnit?.Move(_battleStations[(int) end_station], _battleStations[(int) unit_index]);

        return true;
    }
    
    /// <returns>True if a unit exists at station, otherwise, false</returns>
    public bool GetUnit(uint index, out IStationUnit unit, bool fromReserve = false)
    {
        unit = default;
        if (!fromReserve){
            if(index >= _battleStations.Count) return false;
            if(!_activeUnits.ContainsKey(index)) return false;
            unit = _activeUnits[index];
        } else {
            if(index >= _reserveUnits.Count) return false;
            unit = _reserveUnits[(int)index];
        }

        return unit != null;
    }

}
