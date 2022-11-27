using UnityEngine;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEditor;

namespace Client
{
    [System.Serializable]
    public class GameState
    {
        public EcsWorld EcsWorld;
        //public Saves Saves = new Saves();
        public int InputEntity;
        public int PlayerEntity;
        public int InterfaceEntity;

        public GameState(EcsWorld ecsWorld)
        {
            //Saves.InitSave();
            EcsWorld = ecsWorld;
        }
    }
}
