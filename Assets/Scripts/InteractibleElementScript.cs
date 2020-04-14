using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class InteractibleElementScript : MonoBehaviour {

    protected class ListVector3
    {
        public List<Vector3> data;
    }
    protected static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();
    protected Color outlineColor = Color.yellow, outlineSecondColor = Color.red;
    protected float outlineWidth = 7f;
    protected Material outlineMaskMaterial;
    protected Material outlineFillMaterial;
    protected List<Mesh> bakeKeys = new List<Mesh>();
    protected List<ListVector3> bakeValues = new List<ListVector3>();
    protected bool needsUpdate, battlingMonster = false;

    protected Renderer objRenderer;

    protected virtual void Awake()
    {
        // Instantiate outline materials
        outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
        outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

        outlineMaskMaterial.name = "OutlineMask (Instance)";
        outlineFillMaterial.name = "OutlineFill (Instance)";
        
        // Apply material properties immediately
        needsUpdate = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (needsUpdate)
        {
            needsUpdate = false;

            UpdateMaterialProperties();
        }
    }

    void OnDestroy()
    {
        // Destroy material instances
        Destroy(outlineMaskMaterial);
        Destroy(outlineFillMaterial);
    }

    protected void LoadSmoothNormals()
    {

        // Retrieve or generate smooth normals
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {

            // Skip if smooth normals have already been adopted
            if (!registeredMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Retrieve or generate smooth normals
            var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

            // Store smooth normals in UV3
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);
        }

        // Clear UV3 on skinned mesh renderers
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
            {
                skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
            }
        }
    }

    protected List<Vector3> SmoothNormals(Mesh mesh)
    {

        // Group vertices by location
        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        // Copy normals to a new list
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Average normals for grouped vertices
        foreach (var group in groups)
        {

            // Skip single vertices
            if (group.Count() == 1)
            {
                continue;
            }

            // Calculate the average normal
            var smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += mesh.normals[pair.Value];
            }

            smoothNormal.Normalize();

            // Assign smooth normal to each vertex
            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    protected void UpdateMaterialProperties()
    {
        // Apply properties according to mode
        if(battlingMonster)
        {
            outlineFillMaterial.SetColor("_OutlineColor", outlineSecondColor);
        } else
        {
            outlineFillMaterial.SetColor("_OutlineColor", outlineColor);
        }

        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
    }

    public void HighlightObject() {
        if (objRenderer == null)
            return;
        var materials = objRenderer.sharedMaterials.ToList();

        materials.Add(outlineMaskMaterial);
        materials.Add(outlineFillMaterial);

        objRenderer.materials = materials.ToArray();
    }

    public void UnhighlightObject() {
        if (objRenderer == null)
            return;
        var materials = objRenderer.sharedMaterials.ToList();

        materials.Remove(outlineMaskMaterial);
        materials.Remove(outlineFillMaterial);

        objRenderer.materials = materials.ToArray();
    }

    public void SetBattlingMonster(bool vBattlingMonster)
    {
        battlingMonster = vBattlingMonster;
        needsUpdate = true;
    }

    protected bool IsBattlingMonster()
    {
        return battlingMonster;
    }

    public abstract void InteractWithElement();
}
