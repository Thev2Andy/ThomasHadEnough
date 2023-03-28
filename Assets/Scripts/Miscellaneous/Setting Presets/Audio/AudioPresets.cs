using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioPresets
{
    private static Dictionary<Volume, float> VolumeValues = new Dictionary<Volume, float>
    {
        { Volume.ZeroPercent, 0f },
        { Volume.FivePercent, 0.05f },
        { Volume.TenPercent, 0.1f },
        { Volume.FifteenPercent, 0.15f },
        { Volume.TwentyPercent, 0.2f },
        { Volume.TwentyFivePercent, 0.25f },
        { Volume.ThirtyPercent, 0.3f },
        { Volume.ThirtyFivePercent, 0.35f },
        { Volume.FourtyPercent, 0.4f },
        { Volume.FourtyFivePercent, 0.45f },
        { Volume.FiftyPercent, 0.5f },
        { Volume.FiftyFivePercent, 0.55f },
        { Volume.SixtyPercent, 0.6f },
        { Volume.SixtyFivePercent, 0.65f },
        { Volume.SeventyPercent, 0.7f },
        { Volume.SeventyFivePercent, 0.75f },
        { Volume.EightyPercent, 0.8f },
        { Volume.EightyFivePercent, 0.85f },
        { Volume.NinetyPercent, 0.9f },
        { Volume.NinetyFivePercent, 0.95f },
        { Volume.OneHundredPercent, 1f },
    };


    public static float RetrieveValue(Volume Volume) {
        return ((VolumeValues.ContainsKey(Volume) ? VolumeValues[Volume] : 1f));
    }



    public enum Volume
    {
        ZeroPercent,
        FivePercent,
        TenPercent,
        FifteenPercent,
        TwentyPercent,
        TwentyFivePercent,
        ThirtyPercent,
        ThirtyFivePercent,
        FourtyPercent,
        FourtyFivePercent,
        FiftyPercent,
        FiftyFivePercent,
        SixtyPercent,
        SixtyFivePercent,
        SeventyPercent,
        SeventyFivePercent,
        EightyPercent,
        EightyFivePercent,
        NinetyPercent,
        NinetyFivePercent,
        OneHundredPercent,
    }
}
