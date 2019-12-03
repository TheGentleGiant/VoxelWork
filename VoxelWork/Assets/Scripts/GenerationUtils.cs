using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationUtils
{
    private static int maxHeight = 150;
    private static float increment = 0.01f;
    private static int octaves = 4;
    private static float persistence = 0.5f;
    private static int repeat = 2;
    

    public static int GenerateHeight(float x, float z)
    {
        float _height = Map(0, maxHeight, 0, 1, BrownianMotion(x * increment , z *increment, octaves, persistence));
        return (int) _height;
    }
    public static int GenerateStoneHeight(float x, float z)
    {
        float _height = Map(0, maxHeight-10, 0, 1, BrownianMotion(x * increment * 2, z *increment*2, octaves+1, persistence));
        return (int) _height;
    }

    static float Map(float newMin, float newMax, float originalMin, float originalMax, float value)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originalMin, originalMax, value));
    }

    public static float BrownianMotion(float x, float z, int octave, float persistence)
    {
        float _total = 0f;
        float _frequency = 1f;
        float _amplitude = 1f;
        float maxValue = 0;
        for (int i = 0; i < octave; i++)
        {
            //_total += (float)Perlin(x * _frequency, 0, z * _frequency) * _amplitude;
            _total += Mathf.PerlinNoise(x *_frequency, z*_frequency) * _amplitude;
            maxValue += _amplitude;
            _amplitude *= persistence;
            _frequency *= 2;
        }

        return _total / maxValue;
    }

    public static float BrownianMotion3D(float x, float y, float z)
    {
        float XY = BrownianMotion(x * increment * 10, y * increment*10, 3,  0.5f);
        float YZ = BrownianMotion(y * increment *10, z * increment*10, 3,  0.5f);
        float XZ = BrownianMotion(z * increment*10, x * increment*10, 3,  0.5f);

        float YX = BrownianMotion(y * increment*10, x * increment*10, 3, 0.5f);
        float ZY = BrownianMotion(z * increment*10, y * increment*10, 3, 0.5f);
        float ZX = BrownianMotion(z * increment*10, x * increment*10, 3, 0.5f);

        return (XY + YZ + XZ + YX + ZY + ZX) / 6.0f;


    }
    
    
    
    
    
    
    
    /*PERLIN NOISE CALCULATIONS*/
    public static double Perlin(float x, float y, float z)
    {
        
        if (repeat >0)
        {
            x = x % repeat;
            y = y % repeat;
            z = z % repeat;
        }
        int xi = (int) x & 255;
        int yi = (int) y & 255;
        int zi = (int) z & 255;
        double xf = x - (int) x;
        double yf = y - (int) y;
        double zf = z - (int) z;

        double u = Fade(xf);
        double v = Fade(yf);
        double w = Fade(zf);

        int aaa, aba, aab, abb, baa, bba, bab, bbb;
        aaa = p[p[p[xi] + yi] + zi];
        aba = p[p[p[xi] + Increment(yi)] + zi];
        aab = p[p[p[xi] + yi] + Increment(zi)];
        abb = p[p[p[xi] + Increment(yi)] + Increment(zi)];
        baa = p[p[p[Increment(xi)] + yi] + zi];
        bba = p[p[p[Increment(xi)] + Increment(yi)] + zi];
        bab = p[p[p[Increment(xi)] + yi] + Increment(zi)];
        bbb = p[p[p[Increment(xi)] + Increment(yi)] + Increment(zi)];

        double x1, x2, y1, y2;
        x1 = Lerp(Gradient(aaa, xf, yf, zf), Gradient(baa, xf - 1, yf, zf), u);
        x2 = Lerp(Gradient(aba, xf, yf - 1, zf), Gradient(bba, xf - 1, yf - 1, zf), u);

        y1 = Lerp(x1, x2, v);

        x1 = Lerp(Gradient(aab, xf, yf, zf - 1), Gradient(bab, xf - 1, yf, zf - 1), u);
        x2 = Lerp(Gradient(abb, xf, yf - 1, zf - 1), Gradient(bbb, xf - 1, yf - 1, zf - 1), u);

        y2 = Lerp(x1, x2, w);

        return (Lerp(y1, y2, w) + 1) / 2;
    }
    
    private static readonly int[] permutation = { 151,160,137,91,90,15,                 // Hash lookup table as defined by Ken Perlin.  This is a randomly
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,    // arranged array of all numbers from 0-255 inclusive.
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
    };

    private static int[] p;

    public static void perlin()
    {
        p = new int[512];
        for (int x = 0; x < 512; x++)
        {
            p[x] = permutation[x % 256];
        }
    }
    // Fade function as defined by Ken Perlin.  This eases coordinate values
    // so that they will ease towards integral values.  This ends up smoothing
    // the final output.
    // // 6t^5 - 15t^4 + 10t^3
    public static double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    public static int Increment(int num)
    {
        num++;
        if (repeat > 0)
        {
            num %= repeat;
        }
        return num;
    }

    public static double Gradient(int hash, double x, double y, double z)
    {
        switch (hash & 0xF)
        {
            case 0x0: return x + y;
            case 0x1: return -x + y;
            case 0x2: return x - y;
            case 0x3: return -x - y;
            case 0x4: return x + z;
            case 0x5: return -x + z;
            case 0x6: return x - z;
            case 0x7: return -x - z;
            case 0x8: return y + z;
            case 0x9: return -y + z;
            case 0xA: return y - z;
            case 0xB: return -y - z;
            case 0xC: return y + x;
            case 0xD: return -y + z;
            case 0xE: return y - x;
            case 0xF: return -y - z;
            default: return 0;
        }
    }

    public static double Lerp(double a, double b, double x)
    {
        return a + x * (b - a);
    }

}
