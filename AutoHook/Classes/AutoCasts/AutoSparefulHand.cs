﻿using AutoHook.Resources.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace AutoHook.Classes.AutoCasts;

public class AutoSparefulHand : BaseActionCast
{
    public AutoSparefulHand(string name, uint id, ActionType actionType = ActionType.Action) : base(name, id,
        actionType)
    {
    }

    public override string GetName()
        => Name = UIStrings.SparefulHand;

    public override bool CastCondition()
    {
        return true;
    }

    public override bool IsExcludedPriority { get; set; }
}