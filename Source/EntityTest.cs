using PangordsEngine;

namespace PangordsEngine.Source
{
    class EntityTest : Entity
    {
        protected override void OnStart()
        {
            AddComponent<Light>();
        }
    }
}
