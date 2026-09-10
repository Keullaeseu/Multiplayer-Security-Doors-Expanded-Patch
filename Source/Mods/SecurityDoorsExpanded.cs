using HarmonyLib;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerSecurityDoorsExpandedPatch.Source.Mods;

/// <summary>
///     Multiplayer Patch for Security Doors Expanded by LadySylveon, Last Update: 11 Aug @ 3:08am 2026
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

        var _compVacDoorType = GetCompVacDoorType();
        if (_compVacDoorType == null)
            return;

        var _compVacCheckpointType = GetCompVacCheckpointType();
        if (_compVacCheckpointType == null)
            return;

        MpCompat.RegisterLambdaMethod(_compVacDoorType, "CompGetGizmosExtra", 1, 2, 3);
        MpCompat.RegisterLambdaMethod(_compVacCheckpointType, "CompGetGizmosExtra", 1);

        Log.Message("[Multiplayer Security Doors Expanded Patch] initialized.");
    }

    #region Getters

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