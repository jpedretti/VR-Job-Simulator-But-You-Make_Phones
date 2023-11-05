using UnityEngine;

namespace com.NW84P
{
    public static class PlayerPrefsExtensions
    {
        public static void SetBool(string key, bool value)
            => PlayerPrefs.SetInt(key, value ? 1 : 0);

        public static bool GetBool(string key, bool defaultValue = false)
            => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }
}
