using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MenuCameraOrbit))]
public class MenuCameraOrbitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var orbit = (MenuCameraOrbit)target;
        var so = serializedObject;
        var shotsProp = so.FindProperty("shots");
        var targetProp = so.FindProperty("target");

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Shot Capture Tools", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Move the Scene View camera to the position you want, then use the buttons below.\n" +
            "Positions are stored as offsets from the Target transform.",
            MessageType.Info);

        Transform targetTransform = targetProp.objectReferenceValue as Transform;
        Vector3 targetPos = targetTransform != null ? targetTransform.position : Vector3.zero;

        // Add new shot from scene view
        if (GUILayout.Button("➕ Add New Shot (Scene View → Start Position)", GUILayout.Height(30)))
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                so.Update();
                shotsProp.InsertArrayElementAtIndex(shotsProp.arraySize);
                var newShot = shotsProp.GetArrayElementAtIndex(shotsProp.arraySize - 1);

                Vector3 offset = sceneView.camera.transform.position - targetPos;
                newShot.FindPropertyRelative("startPosition").vector3Value = offset;
                newShot.FindPropertyRelative("endPosition").vector3Value = offset + sceneView.camera.transform.forward * 1.5f;
                newShot.FindPropertyRelative("lookAtOffset").vector3Value = Vector3.up * 0.5f;

                so.ApplyModifiedProperties();
                Debug.Log($"Added shot {shotsProp.arraySize - 1}: start at scene view position");
            }
        }

        // Per-shot capture buttons
        if (shotsProp.arraySize > 0)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Update Existing Shots", EditorStyles.boldLabel);

            for (int i = 0; i < shotsProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Shot {i}", GUILayout.Width(50));

                if (GUILayout.Button("Set Start"))
                {
                    var sceneView = SceneView.lastActiveSceneView;
                    if (sceneView != null)
                    {
                        so.Update();
                        var shot = shotsProp.GetArrayElementAtIndex(i);
                        shot.FindPropertyRelative("startPosition").vector3Value =
                            sceneView.camera.transform.position - targetPos;
                        so.ApplyModifiedProperties();
                        Debug.Log($"Shot {i}: start position set from scene view");
                    }
                }

                if (GUILayout.Button("Set End"))
                {
                    var sceneView = SceneView.lastActiveSceneView;
                    if (sceneView != null)
                    {
                        so.Update();
                        var shot = shotsProp.GetArrayElementAtIndex(i);
                        shot.FindPropertyRelative("endPosition").vector3Value =
                            sceneView.camera.transform.position - targetPos;
                        so.ApplyModifiedProperties();
                        Debug.Log($"Shot {i}: end position set from scene view");
                    }
                }

                if (GUILayout.Button("Preview Start"))
                {
                    var sceneView = SceneView.lastActiveSceneView;
                    if (sceneView != null)
                    {
                        var shot = shotsProp.GetArrayElementAtIndex(i);
                        Vector3 pos = targetPos + shot.FindPropertyRelative("startPosition").vector3Value;
                        sceneView.LookAt(targetPos, Quaternion.LookRotation(targetPos - pos), (targetPos - pos).magnitude);
                        sceneView.pivot = targetPos;
                    }
                }

                if (GUILayout.Button("🗑", GUILayout.Width(30)))
                {
                    so.Update();
                    shotsProp.DeleteArrayElementAtIndex(i);
                    so.ApplyModifiedProperties();
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
