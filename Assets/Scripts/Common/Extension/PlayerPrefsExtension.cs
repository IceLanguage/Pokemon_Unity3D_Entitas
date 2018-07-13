using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

static class PlayerPrefsExtension
{
    public static void SetVector3(this PlayerPrefs playerPrefs, string key, Vector3 pos)
    {
        PlayerPrefs.SetFloat(key + ":x", pos.x);
        PlayerPrefs.SetFloat(key + ":y", pos.y);
        PlayerPrefs.SetFloat(key + ":z", pos.z);
    }
    public static Vector3 GetVector3(this PlayerPrefs playerPrefs,string key)
    {
        return new Vector3(
            PlayerPrefs.GetFloat(key + ":x"),
            PlayerPrefs.GetFloat(key + ":y"),
            PlayerPrefs.GetFloat(key + ":z"));
    }
}
