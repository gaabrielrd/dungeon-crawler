using System;
using DungeonCrawler.Core.Services;
using UnityEngine;

namespace DungeonCrawler.UI
{
    public sealed class ScreenNavigator : MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenu";
        private const string SettingsSceneName = "Settings";
        private const string RunPreparationSceneName = "RunPreparation";
        private const string CombatPrototypeSceneName = "CombatPrototype";
        private const string RestSiteSceneName = "RestSite";

        private bool _isNavigating;

        public void GoToMainMenu()
        {
            NavigateToScene(MainMenuSceneName);
        }

        public void GoToSettings()
        {
            NavigateToScene(SettingsSceneName);
        }

        public void GoToRunPreparation()
        {
            NavigateToScene(RunPreparationSceneName);
        }

        public void GoToCombatPrototype()
        {
            NavigateToScene(CombatPrototypeSceneName);
        }

        public void GoToRestSite()
        {
            NavigateToScene(RestSiteSceneName);
        }

        private async void NavigateToScene(string sceneName)
        {
            if (_isNavigating)
            {
                return;
            }

            _isNavigating = true;

            try
            {
                await ServiceRegistry.Resolve<ISceneLoaderService>().LoadSceneAsync(sceneName);
            }
            catch (Exception exception)
            {
                _isNavigating = false;
                Debug.LogException(exception);
            }
        }
    }
}
