using UnityEngine;

namespace Core.Singleton
{
    public abstract class BaseSingleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; protected set; }

        private void Awake()
        {
            CreateSingleton();
            PostAwake();
        }

        protected abstract void CreateSingleton();

        protected virtual void PostAwake()
        {
        }
    }

}