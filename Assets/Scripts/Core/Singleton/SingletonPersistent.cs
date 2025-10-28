using UnityEngine;

namespace Core.Singleton
{
   public abstract class PersistentSingleton<T> : BaseSingleton<T> where T : Component
    {
        protected override void CreateSingleton()
        {
            if (Instance is null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ResetSingleton()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }
}