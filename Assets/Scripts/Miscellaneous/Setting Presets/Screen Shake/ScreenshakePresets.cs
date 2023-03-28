using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenshakePresets
{
    private static Dictionary<Intensity, float> IntensityValues = new Dictionary<Intensity, float>
    {
        { Intensity.ZeroPercent, 0f },
        { Intensity.TwentyFivePercent, 0.25f },
        { Intensity.FiftyPercent, 0.5f },
        { Intensity.SeventyFivePercent, 0.75f },
        { Intensity.OneHundredPercent, 1f },
        { Intensity.OneHundredAndFiftyPercent, 1.5f },
        { Intensity.TwoHundredPercent, 2f },
        { Intensity.TwoHundredAndFiftyPercent, 2.5f },
        { Intensity.ThreeHundredPercent, 3f },
        { Intensity.ThreeHundredAndFiftyPercent, 3.5f },
        { Intensity.FourHundredPercent, 4f },
        { Intensity.FourHundredAndFiftyPercent, 4.5f },
        { Intensity.FiveHundredPercent, 5f },
    };


    public static float RetrieveValue(Intensity Intensity) {
        return ((IntensityValues.ContainsKey(Intensity) ? IntensityValues[Intensity] : 1f));
    }



    public enum Intensity
    {
        ZeroPercent,
        TwentyFivePercent,
        FiftyPercent,
        SeventyFivePercent,
        OneHundredPercent,
        OneHundredAndFiftyPercent,
        TwoHundredPercent,
        TwoHundredAndFiftyPercent,
        ThreeHundredPercent,
        ThreeHundredAndFiftyPercent,
        FourHundredPercent,
        FourHundredAndFiftyPercent,
        FiveHundredPercent
    }
}