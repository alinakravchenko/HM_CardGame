using CardGameWar;

namespace CardGame
{
    internal static class ExtensionsHelpers
    {
        public static void Enqueue(this Queue<Card> cards, Queue<Card> newCards)
        {
            foreach (var card in newCards)
            {
                cards.Enqueue(card);
            }
        }
    }
}