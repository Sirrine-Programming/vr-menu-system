using UnityEngine;
using UnityEngine.UI;

namespace SirrineProgramming.VRMenu
{
    public enum MenuButtonRole
    {
        Secondary,
        Primary,
        Destructive
    }

    /// <summary>
    /// Applies the same clear interaction states to buttons while allowing apps to supply
    /// their own normal role colors and brand palette.
    /// </summary>
    public static class MenuButtonTheme
    {
        public static readonly Color Secondary = new Color(0.10f, 0.22f, 0.31f, 1f);
        public static readonly Color Primary = new Color(0.10f, 0.42f, 0.28f, 1f);
        public static readonly Color Destructive = new Color(0.42f, 0.16f, 0.17f, 1f);
        public static readonly Color Hover = new Color(0.19f, 0.38f, 0.49f, 1f);
        public static readonly Color Pressed = new Color(0.07f, 0.27f, 0.35f, 1f);
        public static readonly Color Disabled = new Color(0.22f, 0.25f, 0.28f, 1f);
        public static readonly Color Text = new Color(0.96f, 0.97f, 0.99f, 1f);

        public static Color GetNormalColor(MenuButtonRole role)
        {
            switch (role)
            {
                case MenuButtonRole.Primary: return Primary;
                case MenuButtonRole.Destructive: return Destructive;
                default: return Secondary;
            }
        }

        public static void Apply(Button button, MenuButtonRole role)
        {
            if (button == null) return;
            Apply(button, GetNormalColor(role));
        }

        public static void Apply(Button button, Color normalColor)
        {
            if (button == null) return;
            ColorBlock colors = button.colors;
            colors.normalColor = normalColor;
            colors.highlightedColor = Hover;
            colors.pressedColor = Pressed;
            colors.selectedColor = Hover;
            colors.disabledColor = Disabled;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            BoxCollider collider = button.GetComponent<BoxCollider>();
            if (collider != null && button.GetComponent<MenuButtonColliderSync>() == null)
                button.gameObject.AddComponent<MenuButtonColliderSync>();

            if (button.targetGraphic != null)
            {
                button.targetGraphic.color = normalColor;
                button.targetGraphic.raycastTarget = true;
            }
        }
    }

    /// <summary>Keeps a physics ray target aligned with a resized uGUI button.</summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public sealed class MenuButtonColliderSync : MonoBehaviour
    {
        private RectTransform rectTransform;
        private BoxCollider boxCollider;

        private void Awake() => Cache();
        private void OnEnable() => Sync();
        private void OnValidate() => Sync();
        private void LateUpdate() => Sync();

        private void Cache()
        {
            if (rectTransform == null) rectTransform = transform as RectTransform;
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        }

        private void Sync()
        {
            Cache();
            if (rectTransform == null || boxCollider == null) return;
            Rect rect = rectTransform.rect;
            boxCollider.size = new Vector3(Mathf.Max(1f, rect.width), Mathf.Max(1f, rect.height), Mathf.Max(1f, boxCollider.size.z));
            boxCollider.center = Vector3.zero;
        }
    }
}
