using HarmonyLib;
using Multiplayer.API;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerSecurityDoorsExpandedPatch.Source.Mods;

/// <summary>
///     Multiplayer Patch for Security and Blast Doors Expanded by LadySylveon,
///     Last Update: 3 Oct @ 9:30pm 2026
///     <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=3777106218" />
///     Lambda ordinals verified against IL (non-capturing <c>b__{id}_{ordinal}</c> methods
///     declared directly on the parent type, see References/SecurityDoorsExpanded.dll):
///     <list type="bullet">
///         <item>CompDockingPoint.CompGetGizmosExtra: b__3_0 (isActive), b__3_1 (toggle isDockingPoint)</item>
///         <item>Building_VacDoor.GetGizmos: b__50_0 (isActive), b__50_1 (toggle failSecure)</item>
///         <item>Building_VehicleDoor.GetGizmos: b__30_0 (DoorTrySet open/close/cancel)</item>
///         <item>CompVacDoor.CompGetGizmosExtra: b__31_0 (god-mode CompleteInstall or AddDesignation)</item>
///         <item>CompVacCheckpoint.CompGetGizmosExtra: b__13_0 (isActive), b__13_1 (toggle checkpoint)</item>
///     </list>
///     Designations (SDE_InstallVacBarrier, SDE_OpenVehicleDoor, SDE_CloseVehicleDoor) are synced
///     by Multiplayer core (DesignationManager.Add/RemoveDesignation). JobDriver/WorkGiver completion
///     (Notify_ManualOrderComplete, CompleteInstall via work) runs deterministically in simulation on
///     all clients, so only the direct state-changing gizmo actions need syncing here.
/// </summary>
[MpCompatFor("Jarocks.SecurityDoorsExpanded")]
public class SecurityDoorsExpanded
{
    private const string LogPrefix = "[Multiplayer Security and Blast Doors Expanded Patch]";

    public SecurityDoorsExpanded(ModContentPack content)
    {
        LongEventHandler.ExecuteWhenFinished(LatePatch);
    }

    private static void LatePatch()
    {
        try
        {
            Log.Message($"{LogPrefix} Initializing...");

            // Each type is resolved and registered independently so one missing
            // type (e.g. after a mod update) does not skip the remaining patches.
            var compDockingPointType = ResolveType("SecurityDoorsExpanded.CompDockingPoint");
            var buildingVacDoorType = ResolveType("SecurityDoorsExpanded.Building_VacDoor");
            var buildingVehicleDoorType = ResolveType("SecurityDoorsExpanded.Building_VehicleDoor");
            var compVacDoorType = ResolveType("SecurityDoorsExpanded.CompVacDoor");
            var compVacCheckpointType = ResolveType("SecurityDoorsExpanded.CompVacCheckpoint");

            // Toggles CompDockingPoint.isDockingPoint (used by the gravship docking overlay).
            RegisterGizmoLambda(compDockingPointType, "CompGetGizmosExtra", 1);

            // Toggles Building_VacDoor.failSecure (+ reachability ClearCache).
            RegisterGizmoLambda(buildingVacDoorType, "GetGizmos", 1);

            // Vehicle door open/close/cancel button. Calls private DoorTrySet(!holdOpenInt),
            // which either sets holdOpenInt directly (powered) or adds/removes the
            // SDE_OpenVehicleDoor/SDE_CloseVehicleDoor designation (unpowered).
            // Without this, powered open/close desyncs immediately.
            RegisterGizmoLambda(buildingVehicleDoorType, "GetGizmos", 0);

            // Vac barrier install button. In god-mode calls CompleteInstall directly,
            // otherwise adds the SDE_InstallVacBarrier designation (also synced by MP core).
            RegisterGizmoLambda(compVacDoorType, "CompGetGizmosExtra", 0);

            // Kept for backwards compatibility: public cleanup helper for the vac barrier
            // install (refunds panels, clears progress). Currently has no in-mod callers,
            // but syncing it is harmless if external callers use it from UI.
            RegisterSyncMethod(compVacDoorType, "CancelInstall");

            // Toggles CompVacCheckpoint.checkpointEnabled (+ RefreshVacuum, ClearCache).
            RegisterGizmoLambda(compVacCheckpointType, "CompGetGizmosExtra", 1);

            Log.Message($"{LogPrefix} Initialized.");
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} Initialization failed: {exception}");
        }
    }

    private static Type ResolveType(string typeName)
    {
        var resolvedType = AccessTools.TypeByName(typeName);

        if (resolvedType == null) Log.Error($"{LogPrefix} Could not find {typeName}.");

        return resolvedType;
    }

    private static void RegisterGizmoLambda(Type parentType, string parentMethod, int lambdaOrdinal)
    {
        if (parentType == null) return;

        try
        {
            MpCompat.RegisterLambdaMethod(parentType, parentMethod, lambdaOrdinal);
        }
        catch (Exception exception)
        {
            Log.Error(
                $"{LogPrefix} Failed to sync {parentType.FullName}:{parentMethod} lambda {lambdaOrdinal}: {exception}");
        }
    }

    private static void RegisterSyncMethod(Type parentType, string methodName)
    {
        if (parentType == null) return;

        try
        {
            MP.RegisterSyncMethod(parentType, methodName);
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} Failed to sync {parentType.FullName}:{methodName}: {exception}");
        }
    }
}