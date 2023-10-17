using UnoEngine;

namespace Domain;
//cardDeck class
public class CardDeck
{
    public ECardDeckType DeckType { get; set; } 
    //list of gamecards as an attribute
    public List<GameCard> Cards { get; set; }
    public int Size { get; set; } 
    //cardDeck constructor
    //108 cards for original deckType and 112 for modern deckType
    public CardDeck(int size, ECardDeckType deckType)
    {   //define the amount of cardPacks in deck
        Size = size;
        DeckType = deckType;
        //create a list of gamecards
        Cards = new List<GameCard>();
        //for loop until the amount of cardPacks are added to deck
        for (int j = 0; j < size; j++)
        {
            //for each defined card color
        foreach (ECardColor color in Enum.GetValues(typeof(ECardColor)))
        {//if it is a defined color card and not a wild card
            if (color != ECardColor.Wild)
            {//for each defined cardvalue
                foreach (ECardValues value in Enum.GetValues(typeof(ECardValues)))
                {
                    switch (value)//evaluate next statements and pair colors with values
                    {//number cards
                        case ECardValues.One:
                        case ECardValues.Two:
                        case ECardValues.Three:
                        case ECardValues.Four:
                        case ECardValues.Five:
                        case ECardValues.Six:
                        case ECardValues.Seven:
                        case ECardValues.Eight:
                        case ECardValues.Nine:
                            Cards.Add(new GameCard()//two pairs of numbercards
                            {
                                CardColor = color,
                                CardValue = value,
                                Score = (int)value//score is equal to the value
                            });
                            Cards.Add(new GameCard()
                            {
                                CardColor = color,
                                CardValue = value,
                                Score = (int)value
                            });
                            break;
                        //action cards
                        case ECardValues.Skip:
                        case ECardValues.Reverse:
                        case ECardValues.DrawTwo:
                            Cards.Add(new GameCard()//two pairs of colored actionCards
                            {
                                CardColor = color,
                                CardValue = value,
                                Score = 20//for skip, reverse and drawtwo the score is equal to 20
                            });
                            Cards.Add(new GameCard()
                            {
                                CardColor = color,
                                CardValue = value,
                                Score = 20
                            });
                            break;
                        //add one zero card with a score of 0
                        case ECardValues.Zero:
                            Cards.Add((new GameCard()
                            {
                                CardColor = color,
                                CardValue = value,
                                Score = 0
                            }));
                            break;
                    }
                }
            }
            else //wild cards, color == wild
            {
                for (int i = 1; i <= 4; i++)
                {//add 4 regular wild cards with a score of 50
                    Cards.Add(new GameCard()
                    {
                        CardColor = color,
                        CardValue = ECardValues.Wild,
                        Score = 50
                    });
                }
                for (int i = 1; i <= 4; i++)
                {//add 4 drawFourWildCards with a score of 50
                    Cards.Add(new GameCard()
                    {
                        CardColor = color,
                        CardValue = ECardValues.WildDrawFour,
                        Score = 50
                    });
                }
                //addition 4 wild cards if modern deckType
                if (DeckType == ECardDeckType.Modern)
                {
                    for (int i = 1; i <= 3; i++)
                    {//add 3 drawFourWildCustomizable with a score of 40
                        Cards.Add(new GameCard()
                        {
                            CardColor = color,
                            CardValue = ECardValues.WildCustomizable,
                            Score = 40
                        });
                    }
                    //and add 1 wildShuffleHands gamecard with a value of 40
                    Cards.Add(new GameCard()
                    {
                        CardColor = color,
                        CardValue = ECardValues.WildShuffleHands,
                        Score = 40
                    });
                }
            }
        }
        }
    }
    //deck shuffling method (Knuth-Fisher-Yates)
    public void Shuffle()
    {
        Random r = new Random();//new random engine
        List<GameCard> unShuffledCards = Cards;
        //Step 1: For each unShuffled item in the collection
        for (int n = unShuffledCards.Count - 1; n > 0; --n)
        {
            //Step 2: Randomly pick an item which has not been shuffled
            int k = r.Next(n + 1);
            //Step 3: Swap the selected item with the last "unstruck" card in the collection
            
            //GameCard temp = unShuffledCards[n];
            //unShuffledCards[n] = unShuffledCards[k];
            //unShuffledCards[k] = temp;
            //<swap via deconstruction>
            (unShuffledCards[n], unShuffledCards[k]) = (unShuffledCards[k], unShuffledCards[n]);
        }
    }
    //method to draw cards from deck, returns a list of cards
    public List<GameCard> Draw(int count)
    {
        //Enum.Take() - Returns a specified number of contiguous elements from the start of a sequence
        var cardsDrawn = Cards.Take(count).ToList();
        //remove cards frow previous list
        Cards.RemoveAll(cards => cardsDrawn.Contains(cards));
        return cardsDrawn;
    }

}