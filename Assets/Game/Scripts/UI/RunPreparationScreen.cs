using System;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Data.Definitions;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public sealed class RunPreparationScreen : UIScreen
    {
        [SerializeField] private ScreenNavigator navigator;
        [SerializeField] private Button startCombatButton;
        [SerializeField] private Button backButton;
        [SerializeField] private DungeonThemeDefinition themeDefinition;

        private void Awake()
        {
            ResolveNavigator();
        }

        private void OnEnable()
        {
            ResolveNavigator();

            if (navigator == null)
            {
                Debug.LogError($"{nameof(RunPreparationScreen)} requires a {nameof(ScreenNavigator)}.", this);
                return;
            }

            if (startCombatButton != null)
            {
                startCombatButton.onClick.AddListener(OnStartCombatPressed);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(navigator.GoToMainMenu);
            }
        }

        private void OnDisable()
        {
            if (navigator == null)
            {
                return;
            }

            if (startCombatButton != null)
            {
                startCombatButton.onClick.RemoveListener(OnStartCombatPressed);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(navigator.GoToMainMenu);
            }
        }

        private void ResolveNavigator()
        {
            if (navigator == null)
            {
                navigator = GetComponent<ScreenNavigator>();
            }
        }

        private async void OnStartCombatPressed()
        {
            if (ServiceRegistry.TryResolve<IDungeonRunService>(out var dungeonRunService))
            {
                try
                {
                    if (dungeonRunService is DungeonRunService concreteService && themeDefinition != null)
                    {
                        concreteService.CurrentThemeDefinition = themeDefinition;
                    }

                    if (!dungeonRunService.HasActiveRun)
                    {
                        await dungeonRunService.StartRunAsync();
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                    return;
                }
            }

            navigator.GoToCombatPrototype();
        }
    }
}
