using System;
using System.Collections.Generic;

public class RandomService
{

    public static RandomService game = new RandomService();

    Random _random;

    public Random Random
    {
        get
        {
            if(null == _random )
            {
                Initialize(System.DateTime.Now.Millisecond);
            }
            return _random;
        }

        set
        {
            _random = value;
        }
    }

    public void Initialize(int seed)
    {
        Random = new Random(seed);
    }

    public bool Bool(float chance)
    {
        return Float() < chance;
    }

    public int Int()
    {
        Initialize(DateTime.Now.Millisecond);
        return Random.Next();
        
    }

    public int Int(int maxValue)
    {
        return Random.Next(maxValue);
    }

    public int Int(int minValue, int maxValue)
    {
        return Random.Next(minValue, maxValue);
    }

    public float Float()
    {
        return (float)Random.NextDouble();
    }

    public float Float(float minValue, float maxValue)
    {
        return minValue + (maxValue - minValue) * Float();
    }

    public T Element<T>(IList<T> elements)
    {
        return elements[Int(0, elements.Count)];
    }
}
