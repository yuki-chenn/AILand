using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// 放在 Editor 文件夹下
static class MeshCombineTool
{
    [MenuItem("GameObject/合并网格")]
    static void CombineSelectedMeshes(MenuCommand command)
    {
        var selectedGOs = Selection.gameObjects;
        if (selectedGOs.Length < 2)
        {
            Debug.LogWarning("至少选择两个带 MeshFilter 的物体来合并");
            return;
        }

        var combineInstances = new List<CombineInstance>();
        Material sharedMat = null;

        foreach (var go in selectedGOs)
        {
            var mf = go.GetComponent<MeshFilter>();
            var mr = go.GetComponent<MeshRenderer>();
            if (mf == null || mf.sharedMesh == null) continue;
            if (sharedMat == null && mr != null) sharedMat = mr.sharedMaterial;

            CombineInstance ci = new CombineInstance
            {
                mesh      = mf.sharedMesh,
                transform = mf.transform.localToWorldMatrix
            };
            combineInstances.Add(ci);
        }

        if (combineInstances.Count < 1)
        {
            Debug.LogWarning("没有找到可合并的网格");
            return;
        }

        var combinedGO = new GameObject("CombinedMesh");
        var mfCombined = combinedGO.AddComponent<MeshFilter>();
        var mrCombined = combinedGO.AddComponent<MeshRenderer>();

        var newMesh = new Mesh();
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        newMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        mfCombined.sharedMesh = newMesh;
        mrCombined.sharedMaterial = sharedMat;
        Undo.RegisterCreatedObjectUndo(combinedGO, "Combine Meshes");
        Selection.activeGameObject = combinedGO;
    }
}