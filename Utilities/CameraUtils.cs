using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

public class CameraUtils : ModSystem
{
    public override void ModifyScreenPosition()
    {
        if (_shakeTime > 0)
            Screenshake(_shakeIntensity, _shakeDuration);
    }

    private static float _shakeIntensity = 0;
    private static float _shakeDuration = 0;
    private static float _shakeTime = 0;
    public static void Screenshake(float intensity, float duration)
    {
        _shakeIntensity = intensity;
        _shakeDuration = duration;

        if (_shakeTime < _shakeDuration)
            _shakeTime++;

        if (_shakeTime > _shakeDuration)
        {
            _shakeIntensity = 0;
            _shakeDuration = 0;
            _shakeTime = 0;
            return;
        }

        Vector2 shakeVector = new Vector2(Main.rand.NextFloat(-_shakeIntensity, _shakeIntensity), Main.rand.NextFloat(-_shakeIntensity, _shakeIntensity));
        Main.screenPosition.X += shakeVector.X;
        Main.screenPosition.Y += shakeVector.Y;
    }
}
