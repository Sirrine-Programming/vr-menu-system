using System.Linq;
using NUnit.Framework;
using UnityEngine;
using SirrineProgramming.VRMenu;

namespace SirrineProgramming.VRMenu.Tests
{
    public class MenuLayoutGeometryTests
    {
        [Test]
        public void StandardMenuRegionsStayInsidePageWithoutColliding()
        {
            var metrics = new MenuLayoutMetrics();
            MenuPageRegions regions = metrics.GetRegions(1050f, 760f);
            Rect page = new Rect(0f, 0f, 1050f, 760f);

            Assert.That(MenuLayoutValidator.IsContained(page, regions.Header), Is.True);
            Assert.That(MenuLayoutValidator.IsContained(page, regions.Content), Is.True);
            Assert.That(MenuLayoutValidator.IsContained(page, regions.Footer), Is.True);
            Assert.That(MenuLayoutValidator.Overlaps(regions.Header, regions.Content), Is.False);
            Assert.That(MenuLayoutValidator.Overlaps(regions.Content, regions.Footer), Is.False);
            Assert.That(MenuLayoutValidator.Overlaps(regions.Header, regions.Footer), Is.False);
        }

        [Test]
        public void LayoutValidatorDetectsOverflowAndInteractiveOverlap()
        {
            Assert.That(MenuLayoutValidator.IsContained(new Rect(0f, 0f, 100f, 100f), new Rect(90f, 10f, 20f, 20f)), Is.False);
            Assert.That(MenuLayoutValidator.Overlaps(new Rect(0f, 0f, 50f, 50f), new Rect(49f, 10f, 30f, 30f)), Is.True);
            Assert.That(MenuLayoutValidator.Overlaps(new Rect(0f, 0f, 40f, 40f), new Rect(40f, 0f, 40f, 40f)), Is.False);
        }

        [Test]
        public void BuiltPageShellHasSeparatedRegionsAndStableHeaderSlots()
        {
            var root = new GameObject("Menu Test Page", typeof(RectTransform), typeof(Canvas));
            try
            {
                RectTransform page = root.GetComponent<RectTransform>();
                page.sizeDelta = new Vector2(1050f, 760f);
                MenuPageShell shell = root.AddComponent<MenuPageShell>();
                shell.Build(page, new MenuLayoutMetrics());
                Canvas.ForceUpdateCanvases();

                Assert.That(MenuLayoutValidator.Validate(shell), Is.Empty);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void LayoutValidatorReportsOverlappingTextBoxes()
        {
            var root = new GameObject("Menu Text Test Page", typeof(RectTransform), typeof(Canvas));
            try
            {
                RectTransform page = root.GetComponent<RectTransform>();
                page.sizeDelta = new Vector2(1050f, 760f);
                MenuPageShell shell = root.AddComponent<MenuPageShell>();
                shell.Build(page, new MenuLayoutMetrics());
                CreateTextBox(shell.TitleSlot, "Title", Vector2.zero, new Vector2(500f, 70f));
                CreateTextBox(shell.TitleSlot, "Subtitle", new Vector2(0f, -5f), new Vector2(500f, 70f));
                Canvas.ForceUpdateCanvases();

                Assert.That(MenuLayoutValidator.Validate(shell)
                    .Any(issue => issue.Message.Contains("Text boxes")), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void LayoutValidatorReportsOverlappingInteractiveTargets()
        {
            var root = new GameObject("Menu Button Test Page", typeof(RectTransform), typeof(Canvas));
            try
            {
                RectTransform page = root.GetComponent<RectTransform>();
                page.sizeDelta = new Vector2(1050f, 760f);
                MenuPageShell shell = root.AddComponent<MenuPageShell>();
                shell.Build(page, new MenuLayoutMetrics());
                CreateButton(shell.Content, "First", new Vector2(100f, 140f));
                CreateButton(shell.Content, "Second", new Vector2(140f, 160f));
                Canvas.ForceUpdateCanvases();

                Assert.That(MenuLayoutValidator.Validate(shell)
                    .Any(issue => issue.Message.Contains("Interactive targets")), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        private static void CreateButton(RectTransform parent, string name, Vector2 position)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button));
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.sizeDelta = new Vector2(100f, 64f);
            rect.anchoredPosition = position;
        }

        private static void CreateTextBox(RectTransform parent, string name, Vector2 position, Vector2 size)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Text));
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }
    }
}
