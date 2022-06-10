using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PangordsEngine
{
    public delegate void Start();
    public delegate void Update();
    public delegate void OnWindowDisplay();

    class CoreBehavior
    {
        public event Start Start;

        protected virtual void OnStart()
        {
            Start?.Invoke();
        }
    }
}
