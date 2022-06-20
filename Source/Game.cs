using System;
using System.Collections.Generic;

namespace PangordsEngine
{
    class Game
    {
        public static List<Entity> entities = new();
        
        public static Entity SpawnEntity<T>() where T : Entity, new()
        {
            Entity entity = new Entity();
            entities.Add(entity);
            return entity;
        }

        // Is called when the game starts
        public static void Start()
        {

        }

        // Is called each frame
        public static void Update()
        {
            
        }

        // Is called when a window is first displayed
        public static void OnWindowDisplay()
        {
            Console.WriteLine("Window displayed!");
        }
    }
}
