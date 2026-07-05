using System;
using System.Collections.Generic;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Dungeon;

namespace DungeonCrawler.Core.Services
{
    public sealed class RestSiteService : IRestSiteService
    {
        private readonly RestSiteDefinition _definition;

        public bool HasRestTaken { get; private set; }

        public RestSiteService(RestSiteDefinition definition = null)
        {
            _definition = definition;
        }

        public bool ShouldEnterRestSite(GeneratedFloor floor)
        {
            if (floor == null) throw new ArgumentNullException(nameof(floor));
            return floor.HasRestingSite;
        }

        public void Rest(List<CombatantState> party)
        {
            if (party == null) throw new ArgumentNullException(nameof(party));

            var healPercent = _definition != null ? _definition.HealPercent : 1f;

            for (var i = 0; i < party.Count; i++)
            {
                var combatant = party[i];
                if (!combatant.IsAlive) continue;

                var healAmount = Math.Max(1, (int)(combatant.MaxHp * healPercent));
                combatant.CurrentHp = Math.Min(combatant.MaxHp, combatant.CurrentHp + healAmount);
            }

            HasRestTaken = true;
        }

        public void ResetRestTaken()
        {
            HasRestTaken = false;
        }
    }
}
