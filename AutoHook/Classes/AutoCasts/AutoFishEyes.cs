﻿using AutoHook.Data;
using AutoHook.Resources.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace AutoHook.Classes.AutoCasts;

public class AutoFishEyes : BaseActionCast
{
    public override int Priority { get; set; } = 6;
    public override bool IsExcludedPriority { get; set; } = false;

    public AutoFishEyes() : base(UIStrings.Fish_Eyes, IDs.Actions.FishEyes, ActionType.Action)
    {
        DoesCancelMooch = true;
        HelpText = UIStrings.CancelsCurrentMooch;
    }

    public override string GetName()
        => Name = UIStrings.Fish_Eyes;

    public override bool CastCondition()
    {
        return true;
    }

    /*protected override DrawOptionsDelegate DrawOptions => () =>
    {

    };*/
}