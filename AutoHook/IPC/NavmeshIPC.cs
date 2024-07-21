using Dalamud.Plugin.Ipc;
using ECommons.Reflection;
using ECommons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace AutoHook.IPC;

internal class NavmeshIPC
{
    internal static readonly string Name = "vnavmesh";
    internal static bool IsEnabled => DalamudReflector.TryGetDalamudPlugin(Name, out var _, false, true);

    private static ICallGateSubscriber<bool>? _navIsReady;
    private static ICallGateSubscriber<float>? _navBuildProgress;
    private static ICallGateSubscriber<Vector3, float, float, Vector3?>? _queryMeshNearestPoint;
    private static ICallGateSubscriber<Vector3, bool, float, Vector3?>? _queryMeshPointOnFloor;
    private static ICallGateSubscriber<object>? _pathStop;
    private static ICallGateSubscriber<bool>? _pathIsRunning;
    private static ICallGateSubscriber<Vector3, bool, bool>? _pathfindAndMoveTo;

    internal static void Init()
    {
        if (IsEnabled)
        {
            try
            {
                _navIsReady = Service.PluginInterface.GetIpcSubscriber<bool>($"{Name}.Nav.IsReady");
                _navBuildProgress = Service.PluginInterface.GetIpcSubscriber<float>($"{Name}.Nav.BuildProgress");

                _queryMeshNearestPoint = Service.PluginInterface.GetIpcSubscriber<Vector3, float, float, Vector3?>($"{Name}.Query.Mesh.NearestPoint");
                _queryMeshPointOnFloor = Service.PluginInterface.GetIpcSubscriber<Vector3, bool, float, Vector3?>($"{Name}.Query.Mesh.PointOnFloor");

                _pathStop = Service.PluginInterface.GetIpcSubscriber<object>($"{Name}.Path.Stop");
                _pathIsRunning = Service.PluginInterface.GetIpcSubscriber<bool>($"{Name}.Path.IsRunning");

                _pathfindAndMoveTo = Service.PluginInterface.GetIpcSubscriber<Vector3, bool, bool>($"{Name}.SimpleMove.PathfindAndMoveTo");
            }
            catch (Exception ex) { ex.Log(); }
        }
    }

    internal static T? Execute<T>(Func<T> func)
    {
        if (IsEnabled)
        {
            try
            {
                if (func != null)
                    return func();
            }
            catch (Exception ex) { ex.Log(); }
        }

        return default;
    }

    internal static void Execute(Action action)
    {
        if (IsEnabled)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex) { ex.Log(); }
        }
    }

    internal static void Execute<T>(Action<T> action, T param)
    {
        if (IsEnabled)
        {
            try
            {
                action?.Invoke(param);
            }
            catch (Exception ex) { ex.Log(); }
        }
    }

    internal static void Execute<T1, T2>(Action<T1, T2> action, T1 p1, T2 p2)
    {
        if (IsEnabled)
        {
            try
            {
                action?.Invoke(p1, p2);
            }
            catch (Exception ex) { ex.Log(); }
        }
    }

    internal static bool IsReady() => Execute(() => _navIsReady!.InvokeFunc());
    internal static float BuildProgress() => Execute(() => _navBuildProgress!.InvokeFunc());
    internal static Vector3? QueryMeshNearestPoint(Vector3 pos, float halfExtentXZ, float halfExtentY) => Execute(() => _queryMeshNearestPoint!.InvokeFunc(pos, halfExtentXZ, halfExtentY));
    internal static Vector3? QueryMeshPointOnFloor(Vector3 pos, float halfExtentXZ) => Execute(() => _queryMeshPointOnFloor!.InvokeFunc(pos, false, halfExtentXZ));
    internal static void Stop() => Execute(_pathStop!.InvokeAction);
    internal static bool IsRunning() => Execute(() => _pathIsRunning!.InvokeFunc());
    internal static void PathfindAndMoveTo(Vector3 pos, bool fly) => Execute(() => _pathfindAndMoveTo!.InvokeFunc(pos, fly));
}
