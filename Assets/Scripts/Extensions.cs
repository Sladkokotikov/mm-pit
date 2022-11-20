using System;
using System.Collections;
using UnityEngine;

public static class Extensions
{


    public static T With<T>(this T source, Action<T> change)
    {
        change(source);
        return source;
    }

    public static IEnumerator Delay(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);
        action();
    }
}