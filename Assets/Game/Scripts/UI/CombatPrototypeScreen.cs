using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public sealed class CombatPrototypeScreen : UIScreen
    {
        [SerializeField] private ScreenNavigator navigator;
        [SerializeField] private Button backToMainMenuButton;

        private void Awake()
        {
            ResolveNavigator();
        }

        private void OnEnable()
        {
            ResolveNavigator();

            if (navigator == null)
            {
                Debug.LogError($"{nameof(CombatPrototypeScreen)} requires a {nameof(ScreenNavigator)}.", this);
                return;
            }

            if (backToMainMenuButton != null)
            {
                backToMainMenuButton.onClick.AddListener(navigator.GoToMainMenu);
            }
        }

        private void OnDisable()
        {
            if (navigator == null)
            {
                return;
            }

            if (backToMainMenuButton != null)
            {
                backToMainMenuButton.onClick.RemoveListener(navigator.GoToMainMenu);
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
