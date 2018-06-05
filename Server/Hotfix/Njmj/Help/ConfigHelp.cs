using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    public class ConfigHelp
    {
        public static T Get<T>(int id)
        {
            ConfigComponent configComponent = Game.Scene.GetComponent<ConfigComponent>();
            IConfig config = configComponent.Get(typeof(T), id);
            return (T) config;
        }
    }
}