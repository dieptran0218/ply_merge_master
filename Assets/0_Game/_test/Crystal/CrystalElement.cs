using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalElement : MonoBehaviour
{
    public int CutCascades;
    public int ExplodeForce;

    public void DestroyCrystal(float scale)
    {
        var mesh = gameObject.AddComponent<MeshDestroy>();
        mesh.CutCascades = CutCascades;
        mesh.ExplodeForce = ExplodeForce;
        mesh.DestroyMesh(scale);
    }
}
