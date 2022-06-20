using System.Collections.Generic;
using System;

namespace PangordsEngine
{
    class Entity
    {
        public static List<Component> components = new List<Component>()
        { 
            new Transform(),
        };

        internal static void Initialize()
        {

        }

        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }

        /// <summary>
        /// Adds a <see cref="Component"/> to the <see cref="Entity"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="Component"/>'s type.</typeparam>
        public T AddComponent<T>() where T : Component, new()
        {
            T t = new T { };
            components.Add(t);
            return (T)Convert.ChangeType(components[^1], typeof(T));
        }

        /// <summary>
        /// Gets an <see cref="Entity"/>'s component of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                {
                    return (T)Convert.ChangeType(components[i], typeof(T));
                }
            }

            throw new Exception($"There is no component of type {typeof(T)} in this entity!");
        }
    }
}