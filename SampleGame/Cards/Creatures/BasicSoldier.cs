using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    public class BasicSoldier : Card
    {
        public override string ShortName => "BasicSoldier";
        public override string Name => "Basic Soldier";
        public override string Description => "Description of a basic soldier";
        public override int ManaCost => 1;
    }
}
