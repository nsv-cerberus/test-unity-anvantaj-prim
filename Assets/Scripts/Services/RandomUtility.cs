using System;

public static class RandomUtility
{
    private static readonly Random _random = new Random();

    public static int GetRandomInRange(int min, int max)
    {
        return (int)Math.Floor(_random.NextDouble() * (max - min + 1)) + min;
    }
}