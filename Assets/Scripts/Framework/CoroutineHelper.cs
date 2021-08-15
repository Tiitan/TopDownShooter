using System.Collections;
using UnityEngine;

namespace Framework
{
    static class CoroutineHelper
    {
        /// <summary>
        /// Makes a coroutine that invokes the coroutine after x seconds.
        /// </summary>
        /// <param name="coroutine">The coroutine to be invoked</param>
        /// <param name="seconds">the amount of time awaited before invoking the coroutine.</param>
        public static IEnumerator After(this IEnumerator coroutine, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            yield return coroutine;
        }
    }
}
