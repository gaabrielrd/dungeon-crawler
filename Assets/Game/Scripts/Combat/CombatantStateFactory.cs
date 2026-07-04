using System;
using DungeonCrawler.Data.Definitions;

namespace DungeonCrawler.Combat
{
    public static class CombatantStateFactory
    {
        public static CombatantState CreateHero(HeroClassDefinition definition, int rank)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            return new CombatantState(
                definition.Id,
                definition.DisplayName,
                CombatSide.Player,
                rank,
                definition.BaseStats);
        }

        public static CombatantState CreateEnemy(EnemyDefinition definition, int rank)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            return new CombatantState(
                definition.Id,
                definition.DisplayName,
                CombatSide.Enemy,
                rank,
                definition.BaseStats);
        }
    }
}
