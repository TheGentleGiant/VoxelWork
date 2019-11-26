using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationUtils
{
    private static int maxHeight = 150;
    private static float increment = 0.01f;
    private static float octaves = 4;
    private static float persistence = 0.5f;

    public static int GenerateHeight(float x, float y)
    {
        return 0;
    }

    static float Map(float newMin, float newMax, float originalMin, float originalMax, float value)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originalMin, originalMax, value));
    }

    static float BrownianMotion(float x, float y, int octave, float persistence)
    {
        float _total = 0f;
        float _frequency = 1f;
        float _amplitude = 1f;

        return 0f;
    }

    public static float PerlinNoise(float width, float height)
    {
        float[,] _values = new float[(int)width,(int)height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double noiseX = x / width - 0.5f;
                double noiseY = y / height - 0.5f;
                _values[y, x] = PerlinNoise((float)noiseX, (float)noiseY);
            }
        }
    
        return 0f;
        //return _values;
    }
}
