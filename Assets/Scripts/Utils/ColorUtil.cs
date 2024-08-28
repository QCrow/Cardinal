using UnityEngine;

public static class ColorUtil
{
    public static Color GetColorFromBuildingColor(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.RED:
                return new Color(0.922f, 0.282f, 0.51f);
            case ColorType.BLUE:
                return new Color(0.22f, 0.48f, 0.84f);
            case ColorType.YELLOW:
                return new Color(0.945f, 0.749f, 0.361f);
            default:
                return new Color(1, 1, 1, 1);
        }
    }
}