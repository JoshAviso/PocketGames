
using System;

public interface IBattleAction
{
    public string Name { get; protected set; }  
    public bool Perform();
    public bool WhileWaiting(float deltaTime);
    public bool Cleanup();
    public bool IsFinished { get; set; }
}

[Serializable] public class BattleManager
{   
    private bool _forceQuitBattle;
    public static bool BattleOngoing { get; private set; }
    public static bool ShouldBattleFinish =>
        !Instance._forceQuitBattle;

    public static bool BeginBattle()
    {
        if(BattleOngoing) return false;

        return Instance.SetupBattle();
    }

    public static bool EndCurrentBattle()
    {
        if(!BattleOngoing) return false;
        Instance._forceQuitBattle = true;
        return true;
    }

    private bool SetupBattle()
    {
        BattleOngoing = true;

        // Setup Code
        


        
        return true;
    }

    public static void Tick(float deltaTime)
    {
        if(!BattleOngoing) return;

        if(ShouldBattleFinish)
            Instance.CleanupBattle();

        // Tick Code

    }

    private void CleanupBattle()
    {
        // Cleanup Code


        BattleOngoing = false;
    }

    private static BattleManager _instance;
    public static BattleManager Instance {
        get {
            _instance ??= new();
            return _instance;
        }
        private set { _instance = value; }
    }
}
