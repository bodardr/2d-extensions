using System;
using System.Collections;
using UnityEngine;

public static class CoroutineExtensions
{
    public static IEnumerator Then(this IEnumerator coroutine, IEnumerator then)
    {
        yield return coroutine;
        yield return then;
    }

    public static IEnumerator Then(this IEnumerator coroutine, Action then)
    {
        yield return coroutine;
        yield return then.ToCoroutine();
    }

    public static IEnumerator ToCoroutine(this Action action)
    {
        action();
        yield break;
    }

    public static IEnumerator Wait(this IEnumerator coroutine, float secondsToWait)
    {
        yield return coroutine;
        yield return new WaitForSeconds(secondsToWait);
    }
}