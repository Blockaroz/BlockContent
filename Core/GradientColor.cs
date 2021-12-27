using Microsoft.Xna.Framework;
using Terraria;

public struct GradientColor
{
    private Color[] _color;
    private float _fadeSpeed;
    private float _timeOnColor;

    public GradientColor(Color[] colors, float timePerColor = 1, float fadeSpeed = 1)
    {
        _color = colors;
        _fadeSpeed = fadeSpeed * 60;
        _timeOnColor = timePerColor * 60;
        if (_fadeSpeed > _timeOnColor)
            _fadeSpeed = _timeOnColor;
    }

    public Color Value
    {
        get
        {
            float count = Main.GlobalTimeWrappedHourly * 60;
            float t = count % _timeOnColor / _fadeSpeed;
            int index = (int)((count / _timeOnColor) % _color.Length);
            return Color.Lerp(_color[index], _color[(index + 1) % _color.Length], t);
        }
    }
}
