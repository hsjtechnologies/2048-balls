using UnityEngine;

namespace Core.Singleton
{
   public abstract class NonPersistentSingleton<T> : BaseSingleton<T> where T : Component
    {
        protected override void CreateSingleton()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}