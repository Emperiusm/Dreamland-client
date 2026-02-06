using System.Collections;
using UnityEngine;

namespace Dreamland
{
    public static class UploadRetry
    {
        public static float[] GetBackoffDelays(int attempts)
        {
            var delays = new float[attempts];
            for (int i = 0; i < attempts; i += 1)
            {
                delays[i] = Mathf.Pow(2f, i);
            }
            return delays;
        }

        public static IEnumerator Run(int attempts, System.Func<IEnumerator> action)
        {
            var delays = GetBackoffDelays(attempts);
            for (int i = 0; i < attempts; i += 1)
            {
                yield return action();
                if (action == null)
                {
                    yield break;
                }
                if (i < attempts - 1)
                {
                    yield return new WaitForSeconds(delays[i]);
                }
            }
        }
    }
}
