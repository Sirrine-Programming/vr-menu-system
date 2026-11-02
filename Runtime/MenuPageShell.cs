using UnityEngine;
using UnityEngine.UI;

namespace SirrineProgramming.VRMenu
{
    /// <summary>
    /// Standard page regions for a world-space menu: a fixed header, flexible content area,
    /// and a fixed footer row. Header slots are symmetric so Back never shifts the title.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MenuPageShell : MonoBehaviour
    {
        [SerializeField] private MenuLayoutMetrics metrics = new MenuLayoutMetrics();
        [SerializeField] private RectTransform header;
        [SerializeField] private RectTransform backSlot;
        [SerializeField] private RectTransform titleSlot;
        [SerializeField] private RectTransform trailingSlot;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform footer;

        public MenuLayoutMetrics Metrics => metrics;
        public RectTransform Header => header;
        public RectTransform BackSlot => backSlot;
        public RectTransform TitleSlot => titleSlot;
        public RectTransform TrailingSlot => trailingSlot;
        public RectTransform Content => content;
        public RectTransform Footer => footer;

        public void Build(RectTransform pageRoot, MenuLayoutMetrics layoutMetrics = null)
        {
            if (pageRoot == null) throw new System.ArgumentNullException(nameof(pageRoot));
            metrics = layoutMetrics ?? metrics ?? new MenuLayoutMetrics();
            if (transform != pageRoot) transform.SetParent(pageRoot, false);

            header = CreateRegion("Header", transform as RectTransform);
            content = CreateRegion("Content", transform as RectTransform);
            footer = CreateRegion("Footer", transform as RectTransform);
            ConfigureRegions();
        }

        private void ConfigureRegions()
        {
            RectTransform root = transform as RectTransform;
            if (root == null || header == null || content == null || footer == null) return;

            header.anchorMin = new Vector2(0f, 1f);
            header.anchorMax = Vector2.one;
            header.pivot = new Vector2(0.5f, 1f);
            header.sizeDelta = new Vector2(0f, metrics.headerHeight);
            header.anchoredPosition = Vector2.zero;

            footer.anchorMin = Vector2.zero;
            footer.anchorMax = new Vector2(1f, 0f);
            footer.pivot = new Vector2(0.5f, 0f);
            footer.sizeDelta = new Vector2(0f, metrics.footerHeight);
            footer.anchoredPosition = Vector2.zero;

            content.anchorMin = Vector2.zero;
            content.anchorMax = Vector2.one;
            content.offsetMin = new Vector2(metrics.pagePadding, metrics.footerHeight + metrics.sectionGap);
            content.offsetMax = new Vector2(-metrics.pagePadding, -(metrics.headerHeight + metrics.sectionGap));

            backSlot = CreateRegion("BackSlot", header);
            backSlot.anchorMin = new Vector2(0f, 0.15f);
            backSlot.anchorMax = new Vector2(0f, 0.85f);
            backSlot.pivot = new Vector2(0f, 0.5f);
            backSlot.sizeDelta = new Vector2(metrics.backSlotWidth, 0f);
            backSlot.anchoredPosition = new Vector2(metrics.pagePadding, 0f);

            titleSlot = CreateRegion("TitleSlot", header);
            titleSlot.anchorMin = new Vector2(0.5f, 0f);
            titleSlot.anchorMax = new Vector2(0.5f, 1f);
            titleSlot.pivot = new Vector2(0.5f, 0.5f);
            titleSlot.sizeDelta = new Vector2(Mathf.Max(0f,
                root.rect.width - metrics.backSlotWidth * 2f - metrics.pagePadding * 2f), 0f);
            titleSlot.anchoredPosition = Vector2.zero;

            trailingSlot = CreateRegion("TrailingSlot", header);
            trailingSlot.anchorMin = new Vector2(1f, 0.15f);
            trailingSlot.anchorMax = new Vector2(1f, 0.85f);
            trailingSlot.pivot = new Vector2(1f, 0.5f);
            trailingSlot.sizeDelta = new Vector2(metrics.backSlotWidth, 0f);
            trailingSlot.anchoredPosition = new Vector2(-metrics.pagePadding, 0f);

            MenuLayoutGroups.AddHorizontalRow(footer, metrics.buttonGap,
                new RectOffset(Mathf.CeilToInt(metrics.pagePadding), Mathf.CeilToInt(metrics.pagePadding), 8, 8));
        }

        private static RectTransform CreateRegion(string regionName, RectTransform parent)
        {
            Transform existing = parent.Find(regionName);
            if (existing != null) return existing as RectTransform;
            GameObject region = new GameObject(regionName, typeof(RectTransform));
            RectTransform rect = region.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.localScale = Vector3.one;
            return rect;
        }
    }
}
