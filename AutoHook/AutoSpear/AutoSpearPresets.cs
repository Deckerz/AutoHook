using AutoHook.Classes;
using AutoHook.Configurations;
using Dalamud.Interface.Utility.Raii;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Schedulers;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static FFXIVClientStructs.ThisAssembly;

namespace AutoHook.AutoSpear;

public class AutoSpearPresets : BasePreset
{
    public List<AutoSpearConfig> Presets = new();
    public override List<BasePresetConfig> PresetList => Presets.Cast<BasePresetConfig>().ToList();

    public override void AddNewPreset(string presetName)
    {
        var newPreset = new AutoSpearConfig(presetName);
        Presets.Add(newPreset);
        SelectedGuid = newPreset.UniqueId.ToString();
        Service.Save();
    }

    public override void AddNewPreset(BasePresetConfig preset)
    {
        var json = JsonConvert.SerializeObject(preset);
        var copy = JsonConvert.DeserializeObject<AutoSpearConfig>(json);
        copy!.UniqueId = Guid.NewGuid();
        Presets.Add(copy);
        SelectedGuid = copy.UniqueId.ToString();
        Service.Save();
    }

    public override void RemovePreset(Guid value)
    {
        var preset = Presets.Find(p => p.UniqueId == value);
        if (preset == null)
            return;

        Presets.Remove(preset);
        Service.Save();
    }
}

public class AutoSpearConfig(string presetName) : BasePresetConfig(presetName)
{
    public Guid? Preset = null;
    public List<PositionWithTarget> Nodes = [];

    private readonly AutoSpearRunner AutoSpear = AutoHook.Plugin.AutoSpear;
    public void AddNode(Vector3 position)
    {
        new TickScheduler(() => Nodes.Add(new(position)));
    }

    public void RemoveNode(PositionWithTarget node)
    {
        new TickScheduler(() => Nodes.Remove(node));
    }

    public void SetPreset(Guid presetId)
    {
        Preset = presetId;
    }

    public override void DrawOptions()
    {
        if (Nodes == null || Nodes.Count == 0)
            return;

        foreach (var node in Nodes.ToList())
        {
            var position = node.Position;

            using var id = ImRaii.PushId(Guid.NewGuid().ToString());

            if (AutoSpear.Enabled) ImGui.BeginDisabled();

            ImGui.Text($"X: {position.X} Y: {position.Y} Z:{position.Z} - Target Node: {node.TargetId}");
            ImGui.SameLine();
            if (ImGui.Button("Set target"))
            {
                node.TargetId = Player.Object.TargetObject?.DataId;
            }
            ImGui.SameLine();
            if (ImGui.Button("X"))
            {
                RemoveNode(node);
                Service.Save();
            }

            if (AutoSpear.Enabled) ImGui.EndDisabled();
        }
    }

    public override void AddItem(BaseOption item)
    {
    }

    public override void RemoveItem(Guid value)
    {
    }
}

public class PositionWithTarget(Vector3 positon)
{
    public Vector3 Position { get; set; } = positon;
    public uint? TargetId { get; set; }
}