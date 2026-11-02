using UnityEditor;
using UnityEngine;

namespace SirrineProgramming.VRMenu.Editor
{
    public static class MenuLayoutValidationCommand
    {
        [MenuItem("VR Menu/Validate Active Scene Layouts")]
        public static void ValidateActiveScene()
        {
            MenuPageShell[] shells = Object.FindObjectsByType<MenuPageShell>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            int errorCount = 0;
            foreach (MenuPageShell shell in shells)
            {
                foreach (MenuLayoutIssue issue in MenuLayoutValidator.Validate(shell))
                {
                    Debug.LogError(issue.Message, issue.Context);
                    errorCount++;
                }
            }

            if (errorCount == 0)
                Debug.Log($"VR menu layout validation passed ({shells.Length} page shells checked).");
            else
                Debug.LogError($"VR menu layout validation found {errorCount} issue(s) across {shells.Length} page shells.");
        }
    }
}
