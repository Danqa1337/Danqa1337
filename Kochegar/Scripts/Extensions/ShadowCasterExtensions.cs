using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class ShadowCasterExtensions
{
    public static void SetMesh(this ShadowCaster2D shadowCaster, Mesh mesh)
    {
        var accessFlagsPrivate = BindingFlags.NonPublic | BindingFlags.Instance;
        var meshField = typeof(ShadowCaster2D).GetField("m_Mesh", accessFlagsPrivate);
        var shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);
        var onEnableMethod = typeof(ShadowCaster2D).GetMethod("OnEnable", accessFlagsPrivate);

        shapePathField.SetValue(shadowCaster, mesh.vertices);
        meshField.SetValue(shadowCaster, null);
        onEnableMethod.Invoke(shadowCaster, new object[0]);
    }
}