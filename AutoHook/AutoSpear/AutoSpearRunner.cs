using AutoHook.IPC;
using AutoHook.Spearfishing.Struct;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.Automation.NeoTaskManager;
using ECommons.Automation.UIInput;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoHook.AutoSpear;
public partial class AutoSpearRunner : IDisposable
{
    public string Status = string.Empty;
    public bool Enabled { get; set; } = false;
    private static TaskManager TaskManager => Service.TaskManager;

    private List<PositionWithTarget>? Steps = null;
    private int _currentStep = 0;
    private bool Navigating = false;
    public AutoSpearRunner()
    {
        Service.Framework.Update += HandleTick;
    }

    public void Dispose()
    {
        Service.Framework.Update -= HandleTick;
    }

    private void HandleTick(IFramework framework)
    {
        if (EzThrottler.Throttle("DoAutoSpear", 1000))
        {
            DoAutoSpear();
        }
        else if (!Enabled)
        {
            Status = "Not running...";
        }
        else
        {
            Status = "Throttled...";
        }
    }

    public void DoAutoSpear()
    {
        if (!Enabled)
        {
            if (Steps != null) Steps = null;

            return;
        }

        try
        {
            if (!NavmeshIPC.IsReady() && Enabled)
            {
                Status = "Waiting for Navmesh...";
                return;
            }
        }
        catch
        {
            Status = "vnavmesh communication failed";
            return;
        }

        if (Service.Configuration.AutoSpearConfig.SelectedPreset is not AutoSpearConfig preset)
        {
            Status = "Preset broken?? you shouldn't see this";
            return;
        }

        BuildSteps(preset);

        if (Steps == null) return; // Should never happen

        ProcessStep(Steps[_currentStep]);
    }

    private void ProcessStep(PositionWithTarget data)
    {
        if (Navigating) return;
        if (SpearFishing()) return;

        var distance = Vector3.Distance(Player.Position, data.Position);

        if (distance > 2.5f)
        {
            TaskManager.Enqueue(async () => await Move(data.Position));
        }
        else if (distance < 2.5f)
        {
            if (data.TargetId == null)
            {
                Svc.Log.Error("Please setup target ids before enabling");
                Enabled = false;
                return;
            }

            var obj = ScanForObject(data.TargetId.Value);
            if (obj == null)
            {
                NextStep(); // if object doesnt exist just move on
                return;
            }

            distance = Vector3.Distance(Player.Position, obj.Position);
            if (distance > 2.5f && distance < 20f) // ensure we aren't seeing a distant node
            {
                TaskManager.Enqueue(async () => await Move(obj.Position));
            }
            else if (distance < 2.5f)
            {
                if (Service.Condition[ConditionFlag.Mounted])
                {
                    TaskManager.Enqueue(Dismount);
                    TaskManager.EnqueueDelay(2500);
                }

                TaskManager.Enqueue(() => InteractWithObject(obj));
            }
        }
        else
        {
            NextStep();
        }
    }

    private void NextStep()
    {
        _currentStep++; // Move to next step;

        if (_currentStep > Steps!.Count)
        {
            _currentStep = 0;
        }
    }

    private unsafe bool SpearFishing()
    {
        var addon = (SpearfishWindow*)Service.GameGui.GetAddonByName(@"SpearFishing");
        return addon != null && addon->Base.WindowNode != null;
    }

    private async Task Move(Vector3 position)
    {
        MountUp();

        Svc.Log.Debug("Pathing to default: {0} {1} {2}", position.X, position.Y, position.Z);
        NavmeshIPC.PathfindAndMoveTo(position, true);
        Navigating = true;

        while (NavmeshIPC.IsRunning())
        {
            await Task.Delay(1000);
        }

        Navigating = false;
    }

    private void BuildSteps(AutoSpearConfig preset)
    {
        Steps = preset.Nodes;
    }

    private unsafe void InteractWithObject(IGameObject obj)
    {
        var system = TargetSystem.Instance();
        system->OpenObjectInteraction((FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)obj.Address);
    }

    private IGameObject? ScanForObject(uint target)
    {
        var matches = Service.ObjectTable.Where(x => x.DataId == target && x.IsTargetable);

        if (!matches.Any()) return null;

        if (matches.Count() > 1)
        {
            return matches.First(); // TODO: Return closest to player
        }

        return matches.Single();
    }

    private unsafe void MountUp()
    {
        if (Service.Condition[ConditionFlag.Mounted])
            return;

        if (EzThrottler.Throttle("MountUp", 3))
        {
            NavmeshIPC.Stop();
            var am = ActionManager.Instance();
            am->UseAction(ActionType.Mount, 23);
        }
    }

    private unsafe void Dismount()
    {
        if (!Service.Condition[ConditionFlag.Mounted])
            return;

        var am = ActionManager.Instance();
        am->UseAction(ActionType.Mount, 0);
    }
}
