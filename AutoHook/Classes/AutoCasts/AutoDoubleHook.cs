﻿using AutoHook.Resources.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace AutoHook.Classes.AutoCasts;

public class AutoDoubleHook : BaseActionCast
{
    public override int Priority { get; set; } = 5;
    public override bool IsExcludedPriority { get; set; } = false;

    public AutoDoubleHook() : base(UIStrings.Double_Hook, Data.IDs.Actions.DoubleHook, ActionType.Action)
    {
    }

    public override string GetName()
        => Name = UIStrings.Double_Hook;

    public override bool CastCondition()
    {
        return true;
    }

    /*protected override DrawOptionsDelegate DrawOptions => () =>
    {

    };*/
}