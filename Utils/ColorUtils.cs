using UnityEngine;

namespace YouAskedForIt.Utils
{
    internal static class ColorUtils
    {
        public static Color FromFixedColor(FixedColor fixedColor)
        {
            switch (fixedColor)
            {
                case FixedColor.Red:
                    return new Color(0.6f, 0.2f, 0.2f, 1f);
                case FixedColor.Green:
                    return new Color(0.2f, 0.6f, 0.2f, 1f);
                case FixedColor.Blue:
                    return new Color(0.2f, 0.2f, 0.6f, 1f);
                case FixedColor.Yellow:
                    return new Color(0.6f, 0.6f, 0.2f, 1f);
                case FixedColor.Cyan:
                    return new Color(0.2f, 0.6f, 0.6f, 1f);
                case FixedColor.Magenta:
                    return new Color(0.6f, 0.2f, 0.6f, 1f);
                case FixedColor.White:
                    return new Color(0.8f, 0.8f, 0.8f, 1f);
                case FixedColor.Gray:
                    return new Color(0.5f, 0.5f, 0.5f, 1f);
                case FixedColor.Black:
                default:
                    return new Color(0.2f, 0.2f, 0.2f, 1f);
            }
        }
    }
}
