using CardGameWar.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameWar
{
    class Program
    {
        static void Main(string[] args)
        {
            int totalTurnCount = 0;
            int finiteGameCount = 0;
            for (int i = 0; i < 36; i++)
            {
                //задание: список игроков, минимум 2
                Game game = new Game("Олли", "Мартин");
                while (!game.IsEndOfGame())
                {
                    game.PlayTurn();
                }

                if (game.TurnCount < 36)
                {
                    totalTurnCount += game.TurnCount;
                    finiteGameCount++;
                }
            }

            double avgTurn = (double)totalTurnCount / (double)finiteGameCount;

            Console.WriteLine(finiteGameCount + " конец игры со средним количеством  " + Math.Round(avgTurn, 2) + " ходов за игру");

            Console.ReadLine();
        }
    }
    public enum Suit //масть
    {
        Clubs,
        Diamonds,
        Spades,
        Hearts
    }
    public static class Extensions
{
    public static void Enqueue(this Queue<Card> cards, Queue<Card> newCards)
    {
        foreach(var card in newCards)
        {
            cards.Enqueue(card);
        }
    }
}
    public class Card
    {
        public string DisplayName { get; set; }
        public Suit Suit { get; set; }
        public int Nominal { get; set; } //номинал
    }
}
namespace CardGameWar.Objects
{
    public class Player
    {
        public string Name { get; set; }
        public Queue<Card> Deck { get; set; }

        public Player(string name)
        {
            Name = name;
        }

        public Queue<Card> Deal(Queue<Card> cards)
        {
            Queue<Card> player1cards = new Queue<Card>();
            Queue<Card> player2cards = new Queue<Card>();

            int counter = 2;
            while (cards.Any())
            {
                if (counter % 2 == 0) //игрок получает новую карту
                {
                    player2cards.Enqueue(cards.Dequeue());
                }
                else
                {
                    player1cards.Enqueue(cards.Dequeue());
                }
                counter++;
            }

            Deck = player1cards;
            return player2cards;

        }

    }
    public static class DeckCreator
    {
        public static Queue<Card> CreateCards()
        {
            Queue<Card> cards = new Queue<Card>();
            for (int i = 2; i <= 14; i++)
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    cards.Enqueue(new Card()
                    {
                        Suit = suit,
                        Nominal = i,
                        DisplayName = GetShortName(i, suit)
                    });
                }
            }
            return Shuffle(cards); //перетасовка карт
        }

        private static Queue<Card> Shuffle(Queue<Card> cards)
        {
            //перетасовка карт
            List<Card> transformedCards = cards.ToList();
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int n = transformedCards.Count - 1; n > 0; --n)
            {
                //на этом шаге выбирают карту
                int j = rnd.Next(n + 1);


                Card temp = transformedCards[n];
                transformedCards[n] = transformedCards[j];
                transformedCards[j] = temp;
            }

            Queue<Card> shuffledCards = new Queue<Card>();
            foreach (var card in transformedCards)
            {
                shuffledCards.Enqueue(card);
            }

            return shuffledCards;
        }

        ////12345
        ////45312

        //Random rnd = new Random();
        //    for (int i = 0; i<Count; i++)
        //    {
        //        int index = rnd.Next(Count);
        //Card tmp = cards[i];
        //cards[i] = cards[index];
        //        cards[index] = tmp;
        //    }

        private static string GetShortName(int value, Suit suit)
        {
            string valueDisplay = "";
            if (value >= 2 && value <= 10)
            {
                valueDisplay = value.ToString();
            }
            else if (value == 11)
            {
                valueDisplay = "♠";
            }
            else if (value == 12)
            {
                valueDisplay = "♥";
            }
            else if (value == 13)
            {
                valueDisplay = "♦";
            }
            else if (value == 14)
            {
                valueDisplay = "♣";
            }

            return valueDisplay + Enum.GetName(typeof(Suit), suit)[0];
        }
    }
    public class Game
    {
        private Player Player1;
        private Player Player2;
        public int TurnCount;
        public Game(string player1name, string player2name)
        {
            Player1 = new Player(player1name);
            Player2 = new Player(player2name);

            var cards = DeckCreator.CreateCards(); //возвращение перетасованного набора карт

            var deck = Player1.Deal(cards); //первый игрок сохраняет свои карты, а второй возвращает в колоду
            //нашла алгоритм Deal, вроде работает правильно
            Player2.Deck = deck;
        }

        public bool IsEndOfGame()
        {
            if (!Player1.Deck.Any())
            {
                Console.WriteLine(Player1.Name + " закончились карты!  " + Player2.Name + " победитель!");
                Console.WriteLine("Повтор: " + TurnCount.ToString());
                return true;
            }
            else if (!Player2.Deck.Any())
            {
                Console.WriteLine(Player2.Name + " закончились карты!  " + Player1.Name + " победитель!");
                Console.WriteLine("Повтор: " + TurnCount.ToString());
                return true;
            }
            else if (TurnCount > 36)
            {
                Console.WriteLine("Бесконечная игра. Нужен конец");
                return true;
            }
            return false;
        }

        public void PlayTurn()
        {
            Queue<Card> pool = new Queue<Card>();

            var player1card = Player1.Deck.Dequeue();
            var player2card = Player2.Deck.Dequeue();

            pool.Enqueue(player1card);
            pool.Enqueue(player2card);

            Console.WriteLine(Player1.Name + " игрок " + player1card.DisplayName + ", " + Player2.Name + " игрок " + player2card.DisplayName);

            while (player1card.Nominal == player2card.Nominal)
            {
                Console.WriteLine("ошибка игры");
                if (Player1.Deck.Count < 4)
                {
                    Player1.Deck.Clear();
                    return;
                }
                if (Player2.Deck.Count < 4)
                {
                    Player2.Deck.Clear();
                    return;
                }

                pool.Enqueue(Player1.Deck.Dequeue());
                pool.Enqueue(Player1.Deck.Dequeue());
                pool.Enqueue(Player1.Deck.Dequeue());
                pool.Enqueue(Player2.Deck.Dequeue());
                pool.Enqueue(Player2.Deck.Dequeue());
                pool.Enqueue(Player2.Deck.Dequeue());

                player1card = Player1.Deck.Dequeue();
                player2card = Player2.Deck.Dequeue();

                pool.Enqueue(player1card);
                pool.Enqueue(player2card);

                Console.WriteLine(Player1.Name + " игрок " + player1card.DisplayName + ", " + Player2.Name + " игрок " + player2card.DisplayName);
            }



            TurnCount++;
        }
    }
}