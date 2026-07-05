using System;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Data.State;

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

        public static CombatantState CreateHeroFromState(HeroState heroState, int rank)
        {
            if (heroState == null)
            {
                throw new ArgumentNullException(nameof(heroState));
            }

            CombatRank.Validate(rank);

            var combatant = new CombatantState(
                heroState.ClassId,
                heroState.HeroName,
                CombatSide.Player,
                rank,
                new CombatStats(
                    heroState.MaxHp,
                    heroState.Attack,
                    heroState.Defense,
                    heroState.Speed));

            combatant.CurrentHp = heroState.CurrentHp;
            return combatant;
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

        public static CombatantState CreateBoss(BossDefinition definition, int rank)
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
