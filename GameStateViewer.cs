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
    public void Init(GameState state)
    {
        _gameState = state;
    }
}
