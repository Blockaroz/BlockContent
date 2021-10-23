using Microsoft.Xna.Framework;
using Terraria;

public struct GradientColor
{
    private Color[] _color;
    private float _fadeSpeed;
    private float _totalTime;

    public GradientColor(Color[] colors, float totalTime = 1, float fadeSpeed = 1)
    {
        _color = colors;
        _fadeSpeed = fadeSpeed * 60;
        _totalTime = totalTime * 60;
        if (_fadeSpeed > _totalTime)
            _fadeSpeed = _totalTime;
    }

    public Color Value
    {
        get
        {
            float t = Main.GameUpdateCount % _totalTime / _fadeSpeed;
            int index = (int)((Main.GameUpdateCount / _totalTime) % _color.Length);
            return Color.Lerp(_color[index], _color[(index + 1) % _color.Length], t);
        }
    }
}
