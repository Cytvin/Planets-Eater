using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private EndGameView _endGameView;
    private IReadOnlyList<Planet> _planets;
    private Player _player;
    private Player _ai;
    private bool _isGameEnd = false;

    public void Init(Player player, Player ai, IReadOnlyList<Planet> planets)
    {
        _endGameView.Init();
        _player = player;
        _ai = ai;
        _planets = planets;
    }

    private void Update()
    {
        if (_isGameEnd)
        {
            return;
        }

        if (_player.PlanetsCount == _planets.Count)
        {
            OnPlayerVictory();
        }

        if (_ai.PlanetsCount == _planets.Count)
        {
            OnAIVictory();
        }

        if (_ai.PlanetsCount == 0)
        {
            OnPlayerVictory();
        }

        if (_player.PlanetsCount == 0)
        {
            OnAIVictory();
        }
    }

    private void OnPlayerVictory()
    {
        _isGameEnd = true;
        _endGameView.EnableOnVictory();
    }

    private void OnAIVictory()
    {
        _isGameEnd = true;
        _endGameView.EnableOnDefeat();
    }
}
