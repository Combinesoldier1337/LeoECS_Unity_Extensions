using Client;
using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateViewer : MonoBehaviour
{
    [SerializeField]
    private GameState _gameState = null;
    private EcsWorld _world = null;    
    public void Init(GameState state, EcsWorld world)
    {
        _gameState = state;
        _world = world;
    }
}
