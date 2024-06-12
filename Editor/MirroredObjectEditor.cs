using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MirroredObject))]
public class MirroredObjectEditor : Editor
{
    private void OnEnable()
    {
        EditorApplication.update += UpdateMirroring;
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateMirroring;
    }

    public override void OnInspectorGUI()
    {
        var script = (MirroredObject)target;

        EditorGUI.BeginChangeCheck();

        // Custom fields
        var newMirrorTarget = (GameObject)EditorGUILayout.ObjectField("Mirror Target", script.MirrorTarget, typeof(GameObject), true);
        var newMirrorRotation = EditorGUILayout.Toggle("Mirror Rotation", script.mirrorRotation);
        var newMirrorPosition = EditorGUILayout.Toggle("Mirror Position", script.mirrorPosition);
        var newMirrorAxis = (MirroredObject.Axis)EditorGUILayout.EnumPopup("Mirror Axis", script.mirrorAxis);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "Change Mirror Properties");
            script.MirrorTarget = newMirrorTarget;
            script.mirrorRotation = newMirrorRotation;
            script.mirrorPosition = newMirrorPosition;
            script.mirrorAxis = newMirrorAxis;

            // Update and sync settings without causing recursion
            script.UpdateMirror();
            script.SyncSettings(); // Only sync downstream, not upstream
        }

        if (GUILayout.Button("Create Mirrored Copy"))
        {
            CreateMirroredCopy(script);
        }
    }

    private void UpdateMirroring()
    {
        var script = (MirroredObject)target;
        if (script != null && script.MirrorTarget != null)
        {
            script.UpdateMirror(); // Update the mirror continuously
        }
    }

    private void CreateMirroredCopy(MirroredObject script)
    {
        // Check if a mirrored object already exists
        if (script.MirrorTarget != null)
        {
            // Display a popup dialog informing the user
            EditorUtility.DisplayDialog("Mirror Target Exists",
                "A mirrored object already exists for this GameObject. Please remove the existing mirror before creating a new one.", "OK");
            return;  // Exit the function to prevent any further action
        }

        // Proceed to create a new mirrored object if none exists
        var original = script.gameObject;
        var copy = Instantiate(original, original.transform.parent);
        copy.name = original.name + " (Mirrored)";
        //UpdateMirrorTransform(script, copy);

        // Ensure the copied object has the MirroredObject component and link them
        if (copy.TryGetComponent<MirroredObject>(out var copyScript))
        {
            copyScript.MirrorTarget = original;
        }

        // Select the new object in the editor
        Selection.activeGameObject = copy;
        script.MirrorTarget = copy;  // Set the mirror target
        script.UpdateMirror();  // Immediately apply mirroring
    }
}