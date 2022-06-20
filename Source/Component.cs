using System;

namespace PangordsEngine
{
    class Component
    {
        public Component Transform { get; }
        public Component Light { get; }
        public Component Camera { get; }

        /// <summary>
        /// Executes the <see cref="Component"/>'s main behavior.
        /// </summary>
        public virtual void Initialize() { }
    }
}
