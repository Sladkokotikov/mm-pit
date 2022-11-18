using System;

public static class Extensions
{


    public static T With<T>(this T source, Action<T> change)
    {
        change(source);
        return source;
    }
}