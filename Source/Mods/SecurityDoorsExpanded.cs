using HarmonyLib;
using Multiplayer.API;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerSecurityDoorsExpandedPatch.Source.Mods;

/// <summary>
///     Multiplayer Patch for Security Doors Expanded by LadySylveon, Last Update: 12 Sep @ 5:01am 2026
///     https://steamcommunity.com/sharedfiles/filedetails/?id=3777106218
/// </summary>
[MpCompatFor("Jarocks.SecurityDoorsExpanded")]
public class SecurityDoorsExpandedPatch
{
    public SecurityDoorsExpandedPatch(ModContentPack _content)
    {
        LongEventHandler.ExecuteWhenFinished(LatePatch);
    }

    private static void LatePatch()
    {
        Log.Message("[Multiplayer Security Doors Expanded Patch] Initializing...");

        var _compDockingPoint = GetCompDockingPoint();
        if (_compDockingPoint == null)
            return;

        var _buildingVacDoor = GetBuilding_VacDoor();
        if (_buildingVacDoor == null)
            return;

        var _compVacDoorType = GetCompVacDoorType();
        if (_compVacDoorType == null)
            return;

        var _compVacCheckpointType = GetCompVacCheckpointType();
        if (_compVacCheckpointType == null)
            return;

        MpCompat.RegisterLambdaMethod(_compDockingPoint, "CompGetGizmosExtra", 1);
        MpCompat.RegisterLambdaMethod(_buildingVacDoor, "GetGizmos", 1);
        MpCompat.RegisterLambdaMethod(_compVacDoorType, "CompGetGizmosExtra", 0);
        MP.RegisterSyncMethod(_compVacDoorType, "CancelInstall");
        MpCompat.RegisterLambdaMethod(_compVacCheckpointType, "CompGetGizmosExtra", 1);

        Log.Message("[Multiplayer Security Doors Expanded Patch] Initialized.");
    }

    #region Getters

    private static Type GetCompDockingPoint()
    {
        var _compDockingPoint = AccessTools.TypeByName("SecurityDoorsExpanded.CompDockingPoint");

        if (_compDockingPoint != null) return _compDockingPoint;

        Log.Error("[Multiplayer Security Doors Expanded Patch] Could not find " +
                  "SecurityDoorsExpanded.CompDockingPoint.");
        return null;
    }

    private static Type GetBuilding_VacDoor()
    {
        var _buildingVacDoor = AccessTools.TypeByName("SecurityDoorsExpanded.Building_VacDoor");

        if (_buildingVacDoor != null) return _buildingVacDoor;

        Log.Error("[Multiplayer Security Doors Expanded Patch] Could not find " +
                  "SecurityDoorsExpanded.Building_VacDoor.");
        return null;
    }

    private static Type GetCompVacDoorType()
    {
        var _compVacDoorType = AccessTools.TypeByName("SecurityDoorsExpanded.CompVacDoor");

        if (_compVacDoorType != null) return _compVacDoorType;

        Log.Error("[Multiplayer Security Doors Expanded Patch] Could not find " +
                  "SecurityDoorsExpanded.CompVacDoor.");
        return null;
    }

    private static Type GetCompVacCheckpointType()
    {
        var _compVacCheckpointType = AccessTools.TypeByName("SecurityDoorsExpanded.CompVacCheckpoint");

        if (_compVacCheckpointType != null) return _compVacCheckpointType;

        Log.Error("[Multiplayer Security Doors Expanded Patch] Could not find " +
                  "SecurityDoorsExpanded.CompVacCheckpoint.");
        return null;
    }

    #endregion
}