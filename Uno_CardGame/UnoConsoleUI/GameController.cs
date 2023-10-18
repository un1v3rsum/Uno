using System.ComponentModel.Design;
using Domain;
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
            var choice = "";
            if (_gameEngine.State.TurnResult == ETurnResult.GameStart)
            {
                Console.Clear();
                Console.WriteLine("< NEW GAME >");
                _gameEngine.showDiscardPile();
                _gameEngine.State.TurnResult = ETurnResult.OnGoing;
            }
            
            if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Skip)
            {
                Console.WriteLine("Last card played was SKIP.");
                Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].NickName } " 
                                  + $"misses his turn! ");
                
                _gameEngine.nextPlayer();
            }
            if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Reverse)
            {
                Console.WriteLine("Last card played was REVERSE. Direction will be set to counterclockwise!");
                
                _gameEngine.setGameDirection();
                _gameEngine.nextPlayer();

            }
            if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.DrawTwo)
            {
                Console.WriteLine("Last card played was DRAW-TWO.");
                Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].NickName } " 
                                  + $" takes 2 cards and misses his turn! ");
                
                _gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].
                    PlayerHand.AddRange(_gameEngine.State.CardDeck.Draw(2));
                
                _gameEngine.nextPlayer();

            }
            if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Wild)
            {
                Console.WriteLine("Last card played was a WILD.");
                Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].NickName } " 
                                  + $"declares the first color! ");
                Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].NickName } " 
                                  + $"cards on hand: ");
                
                _gameEngine.ShowPlayerHand();
                Console.Write("Choose any card: ");
                choice = Console.ReadLine();
                //Player to dealer's left declares the first color to be matched and takes the first turn
            }
            
            Console.WriteLine("active player no: " + _gameEngine.State.ActivePlayerNo);
            Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].NickName } " 
                              + $"cards on hand: ");
            
            _gameEngine.ShowPlayerHand();
            Console.Write("Choose your card: ");
            choice = Console.ReadLine();
            Console.WriteLine("validity of card: " + _gameEngine.checkValidity(_gameEngine.State.DiscardedCards.First(),
                _gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].PlayerHand[int.Parse(choice)]));
            if (choice is "0" or "1" or "2" or "3" or "4" or "5" or "6"  
                &&  _gameEngine.checkValidity(_gameEngine.State.DiscardedCards.First(),
                    _gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].PlayerHand[int.Parse(choice)])==true)
            {
                _gameEngine.playCard(choice);
                _gameEngine.showDiscardPile();
                _gameEngine.ShowPlayerHand();

                //check validity
                //remove from hand to discardPile
                //activeplayer++
                //modify EturnResult    
            }
            if (choice == "d")
            {
                _gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].PlayerHand.
                    AddRange(_gameEngine.State.CardDeck.Draw(1));
                _gameEngine.ShowPlayerHand();
                _gameEngine.nextPlayer();
                _gameEngine.ShowPlayerHand();
                //get card from drawpile to playerHand
                //check if it can be played
                //if played then remove from hand to discardpile
                //activeplayer++
                //modify EturnResult    
            }
            else
            {
                Console.WriteLine("choice is not acceptable!");
            }
            
            

        }

        return null;
    }
}