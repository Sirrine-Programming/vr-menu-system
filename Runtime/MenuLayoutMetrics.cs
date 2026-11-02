using System;
using UnityEngine;

namespace SirrineProgramming.VRMenu
{
    /// <summary>
    /// Shared uGUI canvas-space measurements for world-space menu pages.
    /// Consumers can override values per application while keeping one layout model.
    /// </summary>
    [Serializable]
    public sealed class MenuLayoutMetrics
    {
        [Min(0f)] public float pagePadding = 40f;
        [Min(0f)] public float sectionGap = 16f;
        [Min(1f)] public float headerHeight = 144f;
        [Min(1f)] public float footerHeight = 116f;
        [Min(1f)] public float backSlotWidth = 112f;
        [Min(1f)] public float buttonHeight = 56f;
        [Min(0f)] public float buttonGap = 12f;
        [Min(1f)] public float minimumTargetWidth = 48f;
        [Min(1f)] public float minimumTargetHeight = 48f;

        public MenuPageRegions GetRegions(float width, float height)
        {
            if (width <= 0f) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0f) throw new ArgumentOutOfRangeException(nameof(height));
            if (headerHeight + footerHeight + sectionGap * 2f >= height)
                throw new InvalidOperationException("Menu header and footer leave no usable content area.");
            if (pagePadding * 2f >= width)
                throw new InvalidOperationException("Menu page padding leaves no usable width.");

            return new MenuPageRegions(
                new Rect(0f, height - headerHeight, width, headerHeight),
                new Rect(pagePadding, footerHeight + sectionGap,
                    width - pagePadding * 2f,
                    height - headerHeight - footerHeight - sectionGap * 2f),
                new Rect(0f, 0f, width, footerHeight));
        }
    }

    public readonly struct MenuPageRegions
    {
        public readonly Rect Header;
        public readonly Rect Content;
        public readonly Rect Footer;

        public MenuPageRegions(Rect header, Rect content, Rect footer)
        {
            Header = header;
            Content = content;
            Footer = footer;
        }
    }
}
