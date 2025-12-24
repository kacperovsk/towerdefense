using UnityEditor;
using UnityEngine;

public class ClearPlayerPrefsBeforeBuild
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs cleared!");
    }
}
