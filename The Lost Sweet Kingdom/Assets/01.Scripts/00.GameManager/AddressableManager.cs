using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public enum AddressableLabel
{
    Default,
}

public static class AddressablesManager
{
    private static readonly Dictionary<AddressableLabel, List<AsyncOperationHandle>> _loadedAssets = new();
    private static readonly Dictionary<AddressableLabel, UniTask> _loadingTasks = new();
    private static readonly Dictionary<string, List<UnityEngine.Object>> _cachedAssets =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly List<string> _sceneLoadOrder = new();

    public static bool IsInit { get; private set; }

    public static async UniTask Init(Action action = null)
    {
        IsInit = false;
        await LoadByLabel(AddressableLabel.Default);
        IsInit = true;

        action?.Invoke();
    }

#if UNITY_EDITOR
    public static UniTask LoadByLabel(AddressableLabel label)
    {
        return UniTask.CompletedTask;
    }
#else
    public static async UniTask LoadByLabel(AddressableLabel label)
    {
        if (_loadedAssets.ContainsKey(label)) return;

        bool ownsTask = false;
        if (!_loadingTasks.TryGetValue(label, out UniTask loadTask))
        {
            loadTask = LoadByLabelInternal(label).Preserve();
            _loadingTasks[label] = loadTask;
            ownsTask = true;
        }

        try
        {
            await loadTask;
        }
        finally
        {
            if (ownsTask)
            {
                _loadingTasks.Remove(label);
            }
        }
    }
#endif

    private static async UniTask LoadByLabelInternal(AddressableLabel label)
    {
        List<UnityEngine.Object> requestAssets = new();
        List<AsyncOperationHandle> requestHandles = new();
        AsyncOperationHandle<IList<UnityEngine.Object>> assetHandle =
            Addressables.LoadAssetsAsync<UnityEngine.Object>(
                label.ToString(),
                asset => CacheAsset(asset, requestAssets));
        AsyncOperationHandle<IList<Sprite>> spriteHandle =
            Addressables.LoadAssetsAsync<Sprite>(
                label.ToString(),
                asset => CacheAsset(asset, requestAssets));

        requestHandles.Add(assetHandle);
        requestHandles.Add(spriteHandle);

        try
        {
            await UniTask.WhenAll(assetHandle.ToUniTask(), spriteHandle.ToUniTask());
        }
        catch
        {
            RemoveCachedAssets(requestAssets);
            ReleaseHandles(requestHandles);
            throw;
        }

        foreach (AsyncOperationHandle handle in requestHandles)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                continue;
            }

            Exception operationException = handle.OperationException;
            RemoveCachedAssets(requestAssets);
            ReleaseHandles(requestHandles);
            throw new InvalidOperationException(
                $"Addressables label '{label}' 로드에 실패했습니다.",
                operationException);
        }

        _loadedAssets[label] = requestHandles;
    }


    public static async UniTask LoadByLabelAsync(AddressableLabel label, Action onComplete = null)
    {
        await LoadByLabel(label);
        onComplete?.Invoke();
    }

    public static bool IsLabelLoaded(AddressableLabel label)
    {
#if UNITY_EDITOR
        return IsInit;
#else
        return _loadedAssets.ContainsKey(label);
#endif
    }

    public static Dictionary<string, List<UnityEngine.Object>> GetAllLoadedAssets()
    {
        Dictionary<string, List<UnityEngine.Object>> copiedAssets =
            new(_cachedAssets.Comparer);

        foreach (KeyValuePair<string, List<UnityEngine.Object>> pair in _cachedAssets)
        {
            copiedAssets[pair.Key] = new List<UnityEngine.Object>(pair.Value);
        }

        return copiedAssets;
    }

    public static void UnloadByLabel(AddressableLabel label)
    {
#if UNITY_EDITOR
        return;
#else
        if (!_loadedAssets.ContainsKey(label)) return;

        foreach (var handle in _loadedAssets[label])
        {
            RemoveCachedAssets(handle);
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
        _loadedAssets.Remove(label);
#endif
    }

    public static T Get<T>(string name) where T : class
    {
        if (!_cachedAssets.TryGetValue(name, out var assets))
        {
            Debug.LogWarning($"'{name}'이(가) 캐시에 없음");
            return null;
        }

        for (int i = assets.Count - 1; i >= 0; i--)
        {
            if (assets[i] == null)
            {
                assets.RemoveAt(i);
            }
        }

        foreach (UnityEngine.Object asset in assets)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)) && asset is GameObject gameObject)
            {
                var component = gameObject.GetComponent<T>();
                if (component != null)
                    return component;
            }

            if (asset is T typedAsset)
                return typedAsset;
        }

        Debug.LogWarning($"'{name}'은 있지만, {typeof(T)} 타입은 캐시에 없음");
        return null;
    }

    private static void CacheAsset(
        UnityEngine.Object asset,
        ICollection<UnityEngine.Object> requestAssets)
    {
        if (asset == null)
        {
            return;
        }

        if (!_cachedAssets.TryGetValue(asset.name, out List<UnityEngine.Object> assets))
        {
            assets = new List<UnityEngine.Object>();
            _cachedAssets[asset.name] = assets;
        }

        assets.Add(asset);
        requestAssets.Add(asset);
    }

    private static void ReleaseHandles(IEnumerable<AsyncOperationHandle> handles)
    {
        foreach (AsyncOperationHandle handle in handles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }

    private static void RemoveCachedAssets(AsyncOperationHandle handle)
    {
        if (!handle.IsValid() ||
            handle.Result is not IEnumerable<UnityEngine.Object> loadedAssets)
        {
            return;
        }

        RemoveCachedAssets(loadedAssets);
    }

    private static void RemoveCachedAssets(
        IEnumerable<UnityEngine.Object> loadedAssets)
    {
        foreach (UnityEngine.Object asset in loadedAssets)
        {
            if (asset == null ||
                !_cachedAssets.TryGetValue(asset.name, out List<UnityEngine.Object> assets))
            {
                continue;
            }

            assets.Remove(asset);
            if (assets.Count == 0)
            {
                _cachedAssets.Remove(asset.name);
            }
        }
    }

    public static async UniTask LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action onComplete = null)
    {
        if (mode == LoadSceneMode.Single)
        {
            await UnloadAllScenes();
        }
        else if (_loadedScenes.ContainsKey(sceneName))
        {
            onComplete?.Invoke();
            return;
        }

        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName, mode);
        try
        {
            await handle.ToUniTask();
        }
        catch
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
            throw;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Exception operationException = handle.OperationException;
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
            throw new InvalidOperationException(
                $"Addressables scene '{sceneName}' 로드에 실패했습니다.",
                operationException);
        }

        _loadedScenes[sceneName] = handle;
        _sceneLoadOrder.Add(sceneName);
        onComplete?.Invoke();
    }

    public static async UniTask UnloadScene(Action onComplete = null)
    {
        if (_sceneLoadOrder.Count == 0)
        {
            return;
        }

        string sceneName = _sceneLoadOrder[_sceneLoadOrder.Count - 1];
        if (await UnloadSceneInternal(sceneName))
        {
            onComplete?.Invoke();
        }
    }

    public static async UniTask UnloadScene(string sceneName, Action onComplete = null)
    {
        if (await UnloadSceneInternal(sceneName))
        {
            onComplete?.Invoke();
        }
    }

    private static async UniTask UnloadAllScenes()
    {
        List<string> sceneNames = new(_sceneLoadOrder);
        for (int i = sceneNames.Count - 1; i >= 0; i--)
        {
            await UnloadSceneInternal(sceneNames[i]);
        }
    }

    private static async UniTask<bool> UnloadSceneInternal(string sceneName)
    {
        if (!_loadedScenes.TryGetValue(sceneName, out AsyncOperationHandle<SceneInstance> handle))
        {
            return false;
        }

        await Addressables.UnloadSceneAsync(handle).ToUniTask();
        _loadedScenes.Remove(sceneName);
        _sceneLoadOrder.Remove(sceneName);
        return true;
    }
}
