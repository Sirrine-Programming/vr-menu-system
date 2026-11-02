using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SirrineProgramming.VRMenu
{
    public readonly struct MenuLayoutIssue
    {
        public readonly string Message;
        public readonly Object Context;

        public MenuLayoutIssue(string message, Object context)
        {
            Message = message;
            Context = context;
        }
    }

    /// <summary>
    /// Geometry checks used by EditMode tests and the editor menu-layout validator.
    /// </summary>
    public static class MenuLayoutValidator
    {
        private const float Epsilon = 0.5f;

        public static bool IsContained(Rect outer, Rect inner, float tolerance = Epsilon)
        {
            return inner.xMin >= outer.xMin - tolerance && inner.yMin >= outer.yMin - tolerance &&
                   inner.xMax <= outer.xMax + tolerance && inner.yMax <= outer.yMax + tolerance;
        }

        public static bool Overlaps(Rect first, Rect second, float tolerance = Epsilon)
        {
            return first.xMin < second.xMax - tolerance && first.xMax > second.xMin + tolerance &&
                   first.yMin < second.yMax - tolerance && first.yMax > second.yMin + tolerance;
        }

        public static IReadOnlyList<MenuLayoutIssue> Validate(MenuPageShell shell)
        {
            var issues = new List<MenuLayoutIssue>();
            if (shell == null || shell.transform is not RectTransform root)
            {
                issues.Add(new MenuLayoutIssue("Menu page shell has no RectTransform root.", shell));
                return issues;
            }

            Canvas.ForceUpdateCanvases();
            Rect rootRect = root.rect;
            RectTransform[] regions = { shell.Header, shell.Content, shell.Footer };
            foreach (RectTransform region in regions)
            {
                if (region == null)
                {
                    issues.Add(new MenuLayoutIssue("Menu page shell is missing a header, content, or footer region.", shell));
                    continue;
                }

                Rect local = GetRectRelativeTo(region, root);
                if (!IsContained(rootRect, local))
                    issues.Add(new MenuLayoutIssue($"{region.name} extends outside the menu panel.", region));
            }

            if (shell.Header != null && shell.Content != null && shell.Footer != null)
            {
                Rect header = GetRectRelativeTo(shell.Header, root);
                Rect content = GetRectRelativeTo(shell.Content, root);
                Rect footer = GetRectRelativeTo(shell.Footer, root);
                if (Overlaps(header, content)) issues.Add(new MenuLayoutIssue("Header overlaps the content area.", shell.Header));
                if (Overlaps(content, footer)) issues.Add(new MenuLayoutIssue("Content overlaps the footer.", shell.Footer));
                if (Overlaps(header, footer)) issues.Add(new MenuLayoutIssue("Header overlaps the footer.", shell.Footer));
            }

            ValidateButtons(shell, shell.Header, issues);
            ValidateButtons(shell, shell.Content, issues);
            ValidateButtons(shell, shell.Footer, issues);
            ValidateTextBoxes(shell, shell.Header, issues);
            ValidateTextBoxes(shell, shell.Content, issues);
            ValidateTextBoxes(shell, shell.Footer, issues);
            if (shell.BackSlot != null && shell.TitleSlot != null &&
                Overlaps(GetRectRelativeTo(shell.BackSlot, root), GetRectRelativeTo(shell.TitleSlot, root)))
                issues.Add(new MenuLayoutIssue("The reserved Back slot overlaps the centered title slot.", shell.BackSlot));
            return issues;
        }

        private static void ValidateButtons(MenuPageShell shell, RectTransform region, List<MenuLayoutIssue> issues)
        {
            if (region == null) return;
            Rect regionRect = GetRectRelativeTo(region, shell.transform as RectTransform);
            var buttons = new List<(Button button, Rect rect)>();
            foreach (Button button in region.GetComponentsInChildren<Button>(false))
            {
                if (!button.isActiveAndEnabled || button.GetComponent<MenuLayoutIgnore>() != null) continue;
                Rect rect = GetRectRelativeTo(button.transform as RectTransform, shell.transform as RectTransform);
                if (!IsContained(regionRect, rect))
                    issues.Add(new MenuLayoutIssue($"Interactive target '{button.name}' extends outside {region.name}.", button));

                if (rect.width < shell.Metrics.minimumTargetWidth || rect.height < shell.Metrics.minimumTargetHeight)
                    issues.Add(new MenuLayoutIssue($"Interactive target '{button.name}' is below the configured minimum size.", button));
                buttons.Add((button, rect));
            }

            for (int i = 0; i < buttons.Count; i++)
            for (int j = i + 1; j < buttons.Count; j++)
            {
                if (Overlaps(buttons[i].rect, buttons[j].rect))
                    issues.Add(new MenuLayoutIssue($"Interactive targets '{buttons[i].button.name}' and '{buttons[j].button.name}' overlap.", buttons[j].button));
            }
        }

        private static void ValidateTextBoxes(MenuPageShell shell, RectTransform region, List<MenuLayoutIssue> issues)
        {
            if (region == null) return;
            Rect regionRect = GetRectRelativeTo(region, shell.transform as RectTransform);
            var textBoxes = new List<(Text text, Rect rect)>();
            foreach (Text text in region.GetComponentsInChildren<Text>(false))
            {
                if (!text.isActiveAndEnabled || text.GetComponent<MenuLayoutIgnore>() != null) continue;
                Rect rect = GetRectRelativeTo(text.transform as RectTransform, shell.transform as RectTransform);
                if (!IsContained(regionRect, rect))
                    issues.Add(new MenuLayoutIssue($"Text '{text.name}' extends outside {region.name}.", text));
                textBoxes.Add((text, rect));
            }

            for (int i = 0; i < textBoxes.Count; i++)
            for (int j = i + 1; j < textBoxes.Count; j++)
            {
                if (Overlaps(textBoxes[i].rect, textBoxes[j].rect))
                    issues.Add(new MenuLayoutIssue($"Text boxes '{textBoxes[i].text.name}' and '{textBoxes[j].text.name}' overlap.", textBoxes[j].text));
            }
        }

        private static Rect GetRectRelativeTo(RectTransform target, RectTransform reference)
        {
            if (target == null || reference == null) return Rect.zero;
            var corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Vector3 first = reference.InverseTransformPoint(corners[0]);
            Vector3 second = reference.InverseTransformPoint(corners[2]);
            return Rect.MinMaxRect(Mathf.Min(first.x, second.x), Mathf.Min(first.y, second.y),
                Mathf.Max(first.x, second.x), Mathf.Max(first.y, second.y));
        }
    }

    /// <summary>Marks intentional layered interactions which should be skipped by overlap checks.</summary>
    [DisallowMultipleComponent]
    public sealed class MenuLayoutIgnore : MonoBehaviour { }
}
