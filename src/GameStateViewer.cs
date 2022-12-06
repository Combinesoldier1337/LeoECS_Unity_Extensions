using Leopotam.EcsLite;
using UnityEngine;
using System.Reflection;
namespace Client
{
    public class GameStateViewer : MonoBehaviour
    {
        [SerializeField]
        private GameState _gameState = null;
        public void Init(GameState state)
        {
            _gameState = state;
        }
    }
}

    
