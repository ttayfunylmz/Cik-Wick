using UnityEditor;
using UnityEngine;
using OccaSoftware.OutlineObjects.Runtime;

namespace OccaSoftware.OutlineObjects.Editor
{
    public class SmoothNormalsWindow : EditorWindow
    {
        private Mesh mesh = null;
        private bool isInputValid = false;

        [MenuItem("OccaSoftware/Outline Objects/Generate Smooth Normals")]
        static void OpenWindow()
        {
            //Create window
            SmoothNormalsWindow window = GetWindow<SmoothNormalsWindow>("Generate Smooth Normals");
            window.Show();
            window.ValidateInputs();
        }

        private void OnGUI()
        {
            DrawUI();
            DrawActionButton();
        }

        private void DrawUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.HelpBox(
                "This tool will bake smooth normals to the Mesh's UV3 channel.\n\n"
                    + "Set the mesh you want to bake, then press the \"Bake\" button.\n\n"
                    + "Drag and drop the newly created mesh to the mesh input for the object (automatically created in the same directory as the original mesh)\n\n"
                    + "Then, enable usage of the smoothed normals from the material inspector (enable Use Smoothed Normals).",
                MessageType.Info
            );

            mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", mesh, typeof(Mesh), false);

            if (EditorGUI.EndChangeCheck())
            {
                ValidateInputs();
            }
        }

        private void DrawActionButton()
        {
            GUI.enabled = isInputValid;
            if (GUILayout.Button("Bake"))
            {
                CreateNewMeshWithSmoothNormals();
            }
            GUI.enabled = true;
        }

        private void ValidateInputs()
        {
            isInputValid = false;
            if (mesh != null)
            {
                isInputValid = true;
            }
        }

        private void CreateNewMeshWithSmoothNormals()
        {
            GenerateNormals generateNormals = new GenerateNormals();

            Mesh newMesh = Instantiate(mesh);
            generateNormals.GenerateSmoothNormals(newMesh);

            string path = AssetDatabase.GetAssetPath(mesh);
            path = System.IO.Path.GetDirectoryName(path) + System.IO.Path.DirectorySeparatorChar;
            AssetDatabase.CreateAsset(newMesh, path + mesh.name + "_smoothed" + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}
