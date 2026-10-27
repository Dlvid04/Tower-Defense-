using System;

public static class GameEvents
{
    public static event Action<TroopAgent> UnitKilled;      
    public static event Action<TroopAgent> UnitExitedMap;   

    public static void RaiseUnitKilled(TroopAgent troop)
    {
        if (UnitKilled != null)
        {
            UnitKilled(troop);
        }
    }

    public static void RaiseUnitExitedMap(TroopAgent troop)
    {
        if (UnitExitedMap != null)
        {
            UnitExitedMap(troop);
        }
    }
}