using UnityEngine;

namespace DungeonCrawler.UI
{
    public class UIScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            SetCanvasGroupVisible(true);
        }

        public virtual void Hide()
        {
            SetCanvasGroupVisible(false);
            gameObject.SetActive(false);
        }

        private void SetCanvasGroupVisible(bool visible)
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
