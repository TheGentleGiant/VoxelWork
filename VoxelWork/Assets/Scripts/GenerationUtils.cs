using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationUtils
{
    private static int maxHeight = 150;
    private static float increment = 0.005f;
    private static int octaves = 4;
    private static float persistence = 0.5f;
    private static int repeat = 0;
    

    public static int GenerateHeight(float x, float z)
    {
        float _height = Map(0, maxHeight, 0, 1, BrownianMotion(x * increment , z *increment, octaves, persistence));
        return (int) _height;
    }
    public static int GenerateStoneHeight(float x, float z)
    {
        float _height = Map(0, maxHeight-20, 0, 1, BrownianMotion(x * increment * 2, z *increment*2, octaves+1, persistence));
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
        float _offset = 32000f;
        for (int i = 0; i < octave; i++)
        {
            _total += Perlin((x+ _offset) * _frequency,(z + _offset) * _frequency, 0f) * _amplitude;
            //_total += Mathf.PerlinNoise(x *_frequency, z*_frequency) * _amplitude;
            maxValue += _amplitude;
            _amplitude *= persistence;
            _frequency *= 2;
        }

        return _total / maxValue;
    } 

    public static float BrownianMotion3D(float x, float y, float z)
    {
        float XY = BrownianMotion(x * increment*10, y * increment*10, 3,  0.5f);
        float YZ = BrownianMotion(y * increment*10, z * increment*10, 3,  0.5f);
        float XZ = BrownianMotion(z * increment*10, x * increment*10, 3,  0.5f);

        float YX = BrownianMotion(y * increment*10, x * increment*10, 3, 0.5f);
        float ZY = BrownianMotion(z * increment*10, y * increment*10, 3, 0.5f);
        float ZX = BrownianMotion(z * increment*10, x * increment*10, 3, 0.5f);

        return (XY + YZ + XZ + YX + ZY + ZX) / 6.0f;
    }
    public static float BrownianMotion3D(float x, float y, float z, float smoothing, int octaves)
    {
        float XY = BrownianMotion(x * smoothing, y * smoothing, octaves,  0.5f);
        float YZ = BrownianMotion(y * smoothing, z * smoothing, octaves,  0.5f);
        float XZ = BrownianMotion(z * smoothing, x * smoothing, octaves,  0.5f);

        float YX = BrownianMotion(y * smoothing, x * smoothing, octaves, 0.5f);
        float ZY = BrownianMotion(z * smoothing, y * smoothing, octaves, 0.5f);
        float ZX = BrownianMotion(z * smoothing, x * smoothing, octaves, 0.5f);

        return (XY + YZ + XZ + YX + ZY + ZX) / 6.0f;
    }

    /*public static float _BrownianMotion3D(float x, float y, float z, int octave, float persistence)
    {
        float _total = 0f;
        float _frequency = 1f;
        float _amplitude = 1f;
        float maxValue = 0;

        for (int i = 0; i < octave; i++)
        {
            _total += Perlin(x * _frequency * increment* 10 , y * _frequency * increment* 10, z * _frequency * increment* 10) * _amplitude;
            maxValue += _amplitude;
            _amplitude *= persistence;
            _frequency *= 2;
        }

        return _total / maxValue;
    }*/
    

    /*PERLIN NOISE CALCULATIONS*/
    
    public static float Perlin(float x, float y, float z)
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
        float xf = x - (int) x;
        float yf = y - (int) y;
        float zf = z - (int) z;

        float u = Fade(xf);
        float v = Fade(yf);
        float w = Fade(zf);

      /*OLD*/  
      // int aaa, aba, aab, abb, baa, bba, bab, bbb;
      // aaa = p[p[p[xi] + yi] + zi];
      // aba = p[p[p[xi] + Increment(yi)] + zi];
      // aab = p[p[p[xi] + yi] + Increment(zi)];
      // abb = p[p[p[xi] + Increment(yi)] + Increment(zi)];
      // baa = p[p[p[Increment(xi)] + yi] + zi];
      // bba = p[p[p[Increment(xi)] + Increment(yi)] + zi];
      // bab = p[p[p[Increment(xi)] + yi] + Increment(zi)];
      // bbb = p[p[p[Increment(xi)] + Increment(yi)] + Increment(zi)];
        int a = (p[xi] + yi);
        int aa = (p[a] + zi);                                             
        int ab =( p[a + 1] + zi);                                            
        int b = (p[xi + 1] + yi);
        int ba = (p[b] + zi);                                             
        int bb =( p[b + 1] + zi);                                            
                                                                                
                                                                                                                                                // This is where the "magic" happens.  We calculate a new set of p[] values and use that to get
                                                                                                                                                // our final gradient values.  Then, we interpolate between those gradients with the u value to get
        float x1, x2, y1, y2;                                                                                                                   // 4 x-values.  Next, we interpolate between the 4 x-values with v to get 2 y-values.  Finally,
        x1 = Lerp(_Gradient(p[aa], xf, yf, zf), _Gradient(p[ba], xf - 1, yf, zf), u);                              // we interpolate between the y-values to get a z-value.
        x2 = Lerp(_Gradient(p[ab], xf, yf - 1, zf), _Gradient(p[bb], xf - 1, yf - 1, zf), u);                      // When calculating the p[] values, remember that above, p[a+1] expands to p[xi]+yi+1 -- so you are
                                                                                                                                                // essentially adding 1 to yi.  Likewise, p[ab+1] expands to p[p[xi]+yi+1]+zi+1] -- so you are adding
        y1 = Lerp(x1, x2, v);                                                                                                         // to zi.  The other 3 parameters are your possible return values (see grad()), which are actually   
                                                                                                                                                // the vectors from the edges of the unit cube to the point in the unit cube itself.
        x1 = Lerp(_Gradient(p[aa +1], xf, yf, zf - 1), _Gradient(p[ba +1], xf - 1, yf, zf - 1), u);
        x2 = Lerp(_Gradient(p[ab +1], xf, yf - 1, zf - 1), _Gradient(p[bb + 1], xf - 1, yf - 1, zf - 1), u);

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
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180/*,151*/
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
    public static float Fade(float t)
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
    
    /*OLD GRADIENT FUNCTION
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
    }*/

    public static float _Gradient(int hash, float x, float y, float z)
    {
        int h = hash & 15;                              // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
        float u = h < 8 ? x : y;                        // If the most significant bit (MSB) of the hash is 0 then set u = x.  Otherwise y.
        float v;                                        // In Ken Perlin's original implementation this was another conditional operator (?:).
                                                        // expanded it for readability.
                                                        
        if (h < 4)                                      // If the first and second significant bits are 0 set v = y
        {                                               
            v = y;
        }
        else if (h ==12 ||  h == 14)                    // If the first and second significant bits are 1 set v = x
        {
            v = x;
        }
        else                                            // If the first and second significant bits are not equal (0/1, 1/0) set v = z
        {
            v = z;
        }

        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);

    }

    public static float Lerp(float a, float b, float x)
    {
        return a + x * (b - a);
    }

}
