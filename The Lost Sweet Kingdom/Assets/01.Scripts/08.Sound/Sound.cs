using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace System.Sound
{
    public class Sound
    {
        private static ObjectPool<SoundObject> _objectPool = null;
        private static ObjectPool<SoundObject> ObjectPool
        {
            get
            {
                if (_objectPool == null)
                {
                    _objectPool = new ObjectPool<SoundObject>(
                        createFunc: () =>
                        {
                            GameObject go = UnityEngine.Object.Instantiate(
                                UnityEngine.Resources.Load<GameObject>("ScriptableObject/Sound/SoundObject"),
                                SoundManager.Instance.transform);
                            return go.GetComponent<SoundObject>();
                        },
                        actionOnGet: obj => obj.gameObject.SetActive(true),
                        actionOnRelease: obj => obj.gameObject.SetActive(false),
                        actionOnDestroy: obj => UnityEngine.Object.Destroy(obj.gameObject),
                        collectionCheck: false,
                        defaultCapacity: 10
                    );
                }
                return _objectPool;
            }
        }

        public static SoundObject Play(string soundId, bool loop = false)
        {
            SoundObject soundObject = ObjectPool.Get();
            soundObject.Initialize();
            soundObject.SetSoundSourceByName(soundId);
            soundObject.SetLoop(loop);
            soundObject.PlayWithCallback(new SoundPlayCallback(soundObject));
            return soundObject;
        }

        public static void Stop(SoundObject soundObject)
        {
            soundObject?.Stop();
            ObjectPool.Release(soundObject);
        }

        private class SoundPlayCallback : ICallback
        {
            private SoundObject _soundObject;

            public SoundPlayCallback(SoundObject soundObject)
            {
                _soundObject = soundObject;
            }

            public void OnProcessCompleted()
            {
                ObjectPool.Release(_soundObject);
            }
        }
    }

    public interface ICallback
    {
        void OnProcessCompleted();
    }
}
