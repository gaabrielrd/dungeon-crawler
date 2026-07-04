using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public sealed class MainMenuScreen : UIScreen
    {
        [SerializeField] private ScreenNavigator navigator;
        [SerializeField] private Button startRunButton;
        [SerializeField] private Button settingsButton;

        private void Awake()
        {
            ResolveNavigator();
        }

        private void OnEnable()
        {
            ResolveNavigator();

            if (navigator == null)
            {
                Debug.LogError($"{nameof(MainMenuScreen)} requires a {nameof(ScreenNavigator)}.", this);
                return;
            }

            if (startRunButton != null)
            {
                startRunButton.onClick.AddListener(navigator.GoToRunPreparation);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(navigator.GoToSettings);
            }
        }

        private void OnDisable()
        {
            if (navigator == null)
            {
                return;
            }

            if (startRunButton != null)
            {
                startRunButton.onClick.RemoveListener(navigator.GoToRunPreparation);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(navigator.GoToSettings);
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
