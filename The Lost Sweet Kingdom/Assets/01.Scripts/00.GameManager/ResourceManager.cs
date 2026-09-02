#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ResourceManager
{

    static public T Load<T>(string assetName) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName("assetresource", assetName);
        foreach (var item in assetPaths)
        {
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(item);
            if (asset != null)
            {
                return asset;
            }
        }

        // 데이터 키와 프리팹 파일명의 대소문자가 달라도 Editor/빌드에서 동일하게 조회한다.
        string[] candidateGuids = AssetDatabase.FindAssets(assetName, new[] { "Assets/AssetResource" });
        foreach (string guid in candidateGuids)
        {
            string candidatePath = AssetDatabase.GUIDToAssetPath(guid);
            string candidateName = System.IO.Path.GetFileNameWithoutExtension(candidatePath);
            if (!string.Equals(candidateName, assetName, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            T asset = AssetDatabase.LoadAssetAtPath<T>(candidatePath);
            if (asset != null)
            {
                return asset;
            }
        }
#else
        T addressableAsset = AddressablesManager.Get<T>(assetName);
        if (addressableAsset != null)
        {
            return addressableAsset;
        }
#endif

        return Resources.Load<T>(assetName);
    }

    static public T LoadInstantiate<T>(string assetName, Transform transform = null) where T : UnityEngine.Object
    {
        var resouce = Load<T>(assetName);

        if (resouce != null)
        {
            if (transform == null)
                return GameObject.Instantiate(resouce);

            return GameObject.Instantiate(resouce, transform);
        }

        return null;
    }

}
