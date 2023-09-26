using System.Collections.Generic;
using UnityEngine;

namespace com.NW84P
{
    public static class WaitForSecondsCache
    {
        private static readonly Dictionary<float, WaitForSeconds> _cache = new();

        public static WaitForSeconds Get(float seconds)
        {
            if (!_cache.TryGetValue(seconds, out var waitForSeconds))
            {
                waitForSeconds = new WaitForSeconds(seconds);
                _cache.Add(seconds, waitForSeconds);
            }

            return waitForSeconds;
        }
    }
}
