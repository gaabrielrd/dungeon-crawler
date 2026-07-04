using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public sealed class SettingsScreen : UIScreen
    {
        [SerializeField] private ScreenNavigator navigator;
        [SerializeField] private Button backButton;

        private void Awake()
        {
            ResolveNavigator();
        }

        private void OnEnable()
        {
            ResolveNavigator();

            if (navigator == null)
            {
                Debug.LogError($"{nameof(SettingsScreen)} requires a {nameof(ScreenNavigator)}.", this);
                return;
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
    }
}
