using System;

namespace AutoHook.Spearfishing.Enums;

public enum SpearfishSize : byte
{
    All = 0,
    Small   = 1,
    Average = 2,
    Large   = 3,
    Unknown = 255,
}

public static class SpearFishSizeExtensions
{
    public static string ToName(this SpearfishSize size)
        => size switch
        {
            SpearfishSize.All => "All",
            SpearfishSize.Small   => "Small",
            SpearfishSize.Average => "Average",
            SpearfishSize.Large   => "Large",
            
            _                     => throw new ArgumentOutOfRangeException(nameof(size), size, null),
        };
}