using UnoEngine;

namespace UnoConsoleUI;

public class GameController<TKey>
{
    private readonly GameEngine<TKey> _gameEngine;
    public GameController(GameEngine<TKey> gameEngine)
    {
        _gameEngine = gameEngine;
    }

    public string? MainLoop()
    {
        while (_gameEngine.IsGameOver() == false)
        {
            if (_gameEngine.IsItNewMove())
            {
                Console.Write($"Player {_gameEngine.State.ActivePlayerNo + 1} choose your move: ");
                var cards = Console.ReadLine();
                //_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo]
            }
        }

        return null;
    }
}