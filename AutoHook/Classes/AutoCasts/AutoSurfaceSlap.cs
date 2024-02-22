﻿using AutoHook.Data;
using AutoHook.Resources.Localization;
using AutoHook.Utils;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace AutoHook.Classes.AutoCasts;

public class AutoSurfaceSlap : BaseActionCast
{
    
    public override bool DoesCancelMooch() => true;
    
    public AutoSurfaceSlap() : base(UIStrings.Surface_Slap, Data.IDs.Actions.SurfaceSlap, ActionType.Action)
    {
        
        HelpText = UIStrings.OverridesIdenticalCast;
    }

    public override string GetName()
        => Name = UIStrings.UseSurfaceSlap;

    public override bool CastCondition()
    {
        if (PlayerResources.HasStatus(IDs.Status.IdenticalCast) || PlayerResources.HasStatus(IDs.Status.SurfaceSlap))
            return false;

        return true;
    }

    protected override DrawOptionsDelegate DrawOptions => () =>
    {
        if (DrawUtil.Checkbox(UIStrings.Dont_Cancel_Mooch, ref DontCancelMooch,
                UIStrings.IdenticalCast_HelpText, true))
        {
            Service.Save();
        }
    };

    public override int Priority { get; set; } = 15;
    public override bool IsExcludedPriority { get; set; } = false;
}