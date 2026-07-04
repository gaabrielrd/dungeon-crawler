using System.Collections.Generic;
using DungeonCrawler.Data.Definitions;

namespace DungeonCrawler.Combat
{
    public sealed class TargetingRulesService
    {
        public const string NullSkill = "null_skill";
        public const string NullUser = "null_user";
        public const string NullTarget = "null_target";
        public const string NullFormation = "null_formation";
        public const string DeadUser = "dead_user";
        public const string DeadTarget = "dead_target";
        public const string InvalidUserRank = "invalid_user_rank";
        public const string InvalidTargetRank = "invalid_target_rank";
        public const string InvalidTargetSide = "invalid_target_side";

        public IReadOnlyList<CombatantState> GetValidTargets(
            SkillDefinition skill,
            CombatantState user,
            CombatFormationState formation)
        {
            var targets = new List<CombatantState>();

            if (!CanUseSkill(skill, user).IsValid || formation == null)
            {
                return targets;
            }

            var combatants = formation.Combatants;
            for (var index = 0; index < combatants.Count; index++)
            {
                var target = combatants[index];

                if (ValidateTarget(skill, user, target).IsValid)
                {
                    targets.Add(target);
                }
            }

            return targets;
        }

        public TargetingValidationResult CanUseSkill(SkillDefinition skill, CombatantState user)
        {
            if (skill == null)
            {
                return TargetingValidationResult.Invalid(NullSkill, "Skill is required.");
            }

            if (user == null)
            {
                return TargetingValidationResult.Invalid(NullUser, "User is required.");
            }

            if (!user.IsAlive)
            {
                return TargetingValidationResult.Invalid(DeadUser, "Dead combatants cannot use skills.");
            }

            if (!ContainsRank(skill.ValidUserRanks, user.Rank))
            {
                return TargetingValidationResult.Invalid(InvalidUserRank, "User rank is not valid for this skill.");
            }

            return TargetingValidationResult.Valid();
        }

        public bool CanTarget(SkillDefinition skill, CombatantState user, CombatantState target)
        {
            return ValidateTarget(skill, user, target).IsValid;
        }

        public TargetingValidationResult ValidateTarget(
            SkillDefinition skill,
            CombatantState user,
            CombatantState target)
        {
            var userValidation = CanUseSkill(skill, user);
            if (!userValidation.IsValid)
            {
                return userValidation;
            }

            if (target == null)
            {
                return TargetingValidationResult.Invalid(NullTarget, "Target is required.");
            }

            if (!target.IsAlive)
            {
                return TargetingValidationResult.Invalid(DeadTarget, "Dead combatants cannot be targeted.");
            }

            if (!IsValidTargetSide(skill.TargetType, user, target))
            {
                return TargetingValidationResult.Invalid(InvalidTargetSide, "Target side is not valid for this skill.");
            }

            if (skill.TargetType != SkillTargetType.Self && !ContainsRank(skill.ValidTargetRanks, target.Rank))
            {
                return TargetingValidationResult.Invalid(InvalidTargetRank, "Target rank is not valid for this skill.");
            }

            return TargetingValidationResult.Valid();
        }

        private static bool IsValidTargetSide(SkillTargetType targetType, CombatantState user, CombatantState target)
        {
            switch (targetType)
            {
                case SkillTargetType.Self:
                    return ReferenceEquals(user, target);
                case SkillTargetType.Ally:
                    return !ReferenceEquals(user, target) && user.Side == target.Side;
                case SkillTargetType.AllAllies:
                    return user.Side == target.Side;
                case SkillTargetType.Enemy:
                case SkillTargetType.AllEnemies:
                    return user.Side != target.Side;
                case SkillTargetType.Any:
                    return true;
                default:
                    return false;
            }
        }

        private static bool ContainsRank(int[] ranks, int rank)
        {
            if (ranks == null || ranks.Length == 0)
            {
                return false;
            }

            for (var index = 0; index < ranks.Length; index++)
            {
                if (ranks[index] == rank)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
