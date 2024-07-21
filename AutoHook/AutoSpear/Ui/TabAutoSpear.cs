using AutoHook.Classes;
using AutoHook.Configurations;
using AutoHook.Enums;
using AutoHook.IPC;
using AutoHook.Resources.Localization;
using AutoHook.Spearfishing;
using AutoHook.Ui;
using AutoHook.Utils;
using Dalamud.Interface.Colors;
using ECommons.GameHelpers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoHook.AutoSpear.Ui;

public class TabAutoSpear : BaseTab
{
    public override string TabName => "Auto Spear";
    public override bool Enabled => NavmeshIPC.IsEnabled;
    public override OpenWindow Type => OpenWindow.AutoSpear;

    private readonly AutoSpearPresets _autoSpear = Service.Configuration.AutoSpearConfig;
    private readonly SpearFishingPresets _autoGig = Service.Configuration.AutoGigConfig;
    
    public override void Draw()
    {
        if (ImGui.BeginChild(@"as_cfg1", new Vector2(0, 0), true))
        {
            if(!Player.Available)
            {
                ImGui.Text("Please login to a character befoe continuing");
                ImGui.EndChild();
                return;
            }

            if (_autoSpear.SelectedPreset is AutoSpearConfig selectedPreset)
            {
                ImGui.Text($"Status: {AutoHook.Plugin.AutoSpear.Status}");

                DrawUtil.SpacingSeparator();

                if (ImGui.Button("Add Node"))
                {
                    selectedPreset.AddNode(Player.Position);
                }

                ImGui.SameLine();
                ImGui.Text($"X: {Player.Position.X} Y: {Player.Position.Y} Z:{Player.Position.Z}");

                DrawUtil.SpacingSeparator();

                selectedPreset.DrawOptions();
            }

            ImGui.EndChild();
        }
    }

    public override void DrawHeader()
    {
        var local = AutoHook.Plugin.AutoSpear.Enabled;
        if(ImGui.Checkbox("Enabled", ref local))
        {
            AutoHook.Plugin.AutoSpear.Enabled = local;
        }
        DrawUtil.DrawComboSelectorPreset(_autoSpear);
        ImGui.SameLine();
        DrawUtil.DrawAddNewPresetButton(_autoSpear);
        ImGui.SameLine();
        DrawUtil.DrawDeletePresetButton(_autoSpear);
    }
}