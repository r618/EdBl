using UnityEngine;
/// <summary>
/// screen gird class 
/// </summary>
public partial class MP_ScreenGrid : MonoBehaviour
{
    /// <summary>
    /// update UV
    /// </summary>
    /// <param name="uv"></param>
    public void update_mesh_uv(Vector4 uv)
    {
        Material m = gird_mesh.GetComponent<Renderer>().material;
        m.SetVector("_UVRect", uv);

        Material m2 = edge_mesh.GetComponent<Renderer>().material;
        m2.SetVector("_UVRect", uv);
    }
}
