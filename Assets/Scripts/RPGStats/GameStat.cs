
using System;
using UnityEngine;

[Serializable] public class StatContainer
{
    public string StatName = "Default";
    public int Val = 0;
}
[Serializable] public class GameStat
{
    public string Name = "Default";
    public int MinValue = 0;
    public int MaxValue = -1;
    public int StartValue = 0;
    public GameStatModCalc CustomModCalc = null;

    /// <returns>Default : if MaxValue > 0 - 0.0 to 1.0 as normalized. If MaxValue <= 0 - returns (Stat - 10 - Min) / 2 (round down).</returns>
    public virtual float GetMod(int _currentValue){ 
        return CustomModCalc == null ? 
            (MaxValue <= 0 ?
                Mathf.Floor((_currentValue - MinValue - 10) * 0.5f) : 
                (_currentValue - (float)MinValue) / MaxValue) : 
            CustomModCalc.CalculateMod(this, _currentValue);
    }
}

[CreateAssetMenu(fileName = "New GameStatModCalc", menuName = "Scriptables/GameStats/GameStatModCalc", order = 0)]
public abstract class GameStatModCalc : ScriptableObject {
    public abstract float CalculateMod(GameStat _stat, int _currentValue);}
