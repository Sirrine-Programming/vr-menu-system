using UnityEngine;
using UnityEngine.UI;

namespace SirrineProgramming.VRMenu
{
    public static class MenuLayoutGroups
    {
        public static VerticalLayoutGroup AddVerticalStack(RectTransform parent, float spacing, RectOffset padding = null)
        {
            VerticalLayoutGroup layout = GetOrAdd<VerticalLayoutGroup>(parent.gameObject);
            layout.padding = padding ?? new RectOffset();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return layout;
        }

        public static HorizontalLayoutGroup AddHorizontalRow(RectTransform parent, float spacing, RectOffset padding = null)
        {
            HorizontalLayoutGroup layout = GetOrAdd<HorizontalLayoutGroup>(parent.gameObject);
            layout.padding = padding ?? new RectOffset();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return layout;
        }

        private static T GetOrAdd<T>(GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();
            return component != null ? component : target.AddComponent<T>();
        }
    }
}
