using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Data.Definitions;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class GameDefinitionTests
    {
        private static readonly Type[] DefinitionTypes =
        {
            typeof(HeroClassDefinition),
            typeof(SkillDefinition),
            typeof(EnemyDefinition),
            typeof(BossDefinition),
            typeof(RewardDefinition),
            typeof(ItemDefinition),
            typeof(EquipmentDefinition),
            typeof(StatusEffectDefinition),
            typeof(DungeonThemeDefinition),
            typeof(EncounterTableDefinition),
            typeof(ShopTableDefinition),
            typeof(UpgradeDefinition),
        };

        [Test]
        public void DefinitionsInheritFromGameDefinition()
        {
            foreach (var definitionType in DefinitionTypes)
            {
                Assert.That(
                    typeof(GameDefinition).IsAssignableFrom(definitionType),
                    Is.True,
                    $"{definitionType.Name} must inherit from {nameof(GameDefinition)}.");
            }
        }

        [Test]
        public void DefinitionsHaveCreateAssetMenu()
        {
            foreach (var definitionType in DefinitionTypes)
            {
                var attribute = definitionType.GetCustomAttribute<CreateAssetMenuAttribute>();

                Assert.That(attribute, Is.Not.Null, $"{definitionType.Name} must be creatable from Unity's asset menu.");
                Assert.That(attribute.menuName, Does.StartWith("DungeonCrawler/Data/"));
            }
        }

        [Test]
        public void DefinitionsCanBeInstantiatedAsScriptableObjects()
        {
            var instances = new List<ScriptableObject>();

            try
            {
                foreach (var definitionType in DefinitionTypes)
                {
                    var instance = ScriptableObject.CreateInstance(definitionType);
                    instances.Add(instance);

                    Assert.That(instance, Is.InstanceOf<GameDefinition>());
                }
            }
            finally
            {
                foreach (var instance in instances)
                {
                    UnityEngine.Object.DestroyImmediate(instance);
                }
            }
        }

        [Test]
        public void GameDefinitionExposesStableIdentityFields()
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();

            try
            {
                SetPrivateField(definition, "id", "item.consumable.small_health_potion");
                SetPrivateField(definition, "displayName", "Small Health Potion");
                SetPrivateField(definition, "description", "Restores a small amount of health.");

                Assert.That(definition.Id, Is.EqualTo("item.consumable.small_health_potion"));
                Assert.That(definition.DisplayName, Is.EqualTo("Small Health Potion"));
                Assert.That(definition.Description, Is.EqualTo("Restores a small amount of health."));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(definition);
            }
        }

        private static void SetPrivateField(GameDefinition definition, string fieldName, string value)
        {
            var field = typeof(GameDefinition).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' was not found on GameDefinition.");

            field.SetValue(definition, value);
        }
    }
}
