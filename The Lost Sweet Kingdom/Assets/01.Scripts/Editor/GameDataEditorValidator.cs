#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GameDataEditorValidator
{
    private const string SuccessMarker = "[GameData] Excel/AssetResource validation OK";

    static GameDataEditorValidator()
    {
        EditorApplication.delayCall += ValidateAfterReload;
    }

    [MenuItem("Tools/Game Data/Validate Excel and AssetResource")]
    public static void Validate()
    {
        GameDataRepository.LoadAll();
        GameDataRepository.ValidateRequiredAssets();
        Debug.Log(SuccessMarker);
    }

    private static void ValidateAfterReload()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += ValidateAfterReload;
            return;
        }

        try
        {
            Validate();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
#endif
