using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AutoLevelMenu.EditorNS
{
    public class ObjectHelpersWindow : EditorWindow
    {
        float zPosition = 0;
        int zType;

        enum ZTypes { Pos, Close, Center, Far }

        [MenuItem(EditorGlobal.menuItemStart + "/ObjectHelpers")]
        public static void ShowWindow()
        {
            GetWindow<ObjectHelpersWindow>("ObjectHelpers");
        }

        void OnGUI()
        {
            GUILayout.Label("These are object helpers");

            // Set Z position
            EditorUtils.GuiLine();
            GUILayout.Label("Z Positioning", EditorStyles.boldLabel);

            // Toggle type of z position set
            var zTypes = Utils.GetEnumNames<ZTypes>();
            zType = GUILayout.SelectionGrid(zType, zTypes, zTypes.Length, EditorStyles.miniButton);


            GUILayout.BeginHorizontal("box");
            zPosition = EditorGUILayout.FloatField("Z position", zPosition);
            var copyZPositionString = "";
            var zPosSet = GetZPosSet();
            if (zPosSet != null)
            {
                copyZPositionString = zPosSet.ToString();
            }
            EditorGUI.BeginDisabledGroup(zPosSet == null);
            if (GUILayout.Button("Copy Z " + zTypes[zType] + " " + copyZPositionString))
            {
                SetZPosition();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length == 0);
            if (GUILayout.Button("Set Objects Z " + zTypes[zType]))
            {
                SetObjectsZPosition();
            }
            EditorGUI.EndDisabledGroup();

            // Lock Z
            EditorUtils.GuiLine();
            GUILayout.Label("Set for locked z position", EditorStyles.boldLabel);
            if (GUILayout.Button("Lock Z Axis"))
            {
                LockZ();
            }

            // Lock on z rotation
            EditorUtils.GuiLine();
            GUILayout.Label("Set for locked z rotation", EditorStyles.boldLabel);
            if (GUILayout.Button("Lock To Z Rotation"))
            {
                LockToZRotation();
            }

            // Remove all constraints
            EditorUtils.GuiLine();
            GUILayout.Label("Remove constraints", EditorStyles.boldLabel);
            if (GUILayout.Button("Remove Constraints"))
            {
                RemoveConstraints();
            }
        }

        float? GetZPosSet()
        {
            if (Selection.gameObjects.Length == 0)
            {
                return null;
            }
            var obj = Selection.gameObjects[0];
            switch (zType)
            {
                case (int)ZTypes.Pos:
                    return obj.transform.position.z;
                case (int)ZTypes.Close:
                    return ObjMeshZ(obj, true);
                case (int)ZTypes.Center:
                    if (obj.GetComponent<Renderer>() == null)
                    {
                        return null;
                    }
                    return obj.GetComponent<Renderer>().bounds.center.z;
                case (int)ZTypes.Far:
                    return ObjMeshZ(obj, false);
            }
            return null;
        }

        void SetZPosition()
        {
            if (Selection.gameObjects.Length > 1)
            {
                Debug.Log("Warning multiple objects selected");
            }
            if (Selection.gameObjects.Length == 0)
            {
                Debug.Log("No object selected");
                return;
            }
            var obj = Selection.gameObjects[0];

            var zFloat = GetZPosSet();
            if (zFloat != null)
            {
                zPosition = (float)zFloat;
            }
        }

        void SetObjectsZPosition()
        {
            foreach (var obj in Selection.gameObjects)
            {
                var pos = obj.transform.position;
                float? objMeshZ = 0.0f;
                switch (zType)
                {
                    case (int)ZTypes.Pos:
                        pos.z = zPosition;
                        break;
                    case (int)ZTypes.Close:
                        objMeshZ = ObjMeshZ(obj, true);
                        if (objMeshZ == null)
                        {
                            return;
                        }
                        pos.z = zPosition + (obj.transform.position.z - (float)objMeshZ);
                        break;
                    case (int)ZTypes.Center:
                        if (obj.GetComponent<Renderer>() == null)
                        {
                            return;
                        }
                        pos.z = zPosition + (obj.transform.position.z - obj.GetComponent<Renderer>().bounds.center.z);
                        break;
                    case (int)ZTypes.Far:
                        objMeshZ = ObjMeshZ(obj, false);
                        if (objMeshZ == null)
                        {
                            return;
                        }
                        pos.z = zPosition + (obj.transform.position.z - (float)objMeshZ);
                        break;
                }

                obj.transform.position = pos;
            }
        }

        float? ObjMeshZ(GameObject obj, bool low)
        {
            if (obj.GetComponent<MeshFilter>() == null)
            {
                return null;
            }
            var mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            if (mesh == null)
            {
                return 0;
            }
            var localToWorld = obj.transform.localToWorldMatrix;
            if (mesh.vertices.Length == 0)
            {
                return 0;
            }
            var z = localToWorld.MultiplyPoint3x4(mesh.vertices[0]).z;
            for (var i = 0; i < mesh.vertices.Length; ++i)
            {
                var world_z = localToWorld.MultiplyPoint3x4(mesh.vertices[i]).z;
                if (low)
                {
                    if (world_z < z)
                    {
                        z = world_z;
                    }
                }
                else
                {
                    if (world_z > z)
                    {
                        z = world_z;
                    }
                }
            }
            return z;
        }


        // Lock the z axis for all objects in selection that have rigidbodies
        void LockZ()
        {
            // find the rigidbody and set z
            foreach (var obj in Selection.gameObjects)
            {
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    continue;
                }
                rb.constraints |= RigidbodyConstraints.FreezePositionZ;
            }
        }

        // Lock the z axis for all objects in selection that have rigidbodies
        void LockToZRotation()
        {
            // find the rigidbody and set z
            foreach (var obj in Selection.gameObjects)
            {
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    continue;
                }
                rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            }
        }

        // remove all constraints
        void RemoveConstraints()
        {
            // find the rigidbody and set z
            foreach (var obj in Selection.gameObjects)
            {
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    continue;
                }
                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }
}
