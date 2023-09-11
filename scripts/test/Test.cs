using System;
using Godot;

public partial class Test : Node3D
{
    public override void _Ready()
    {
        var vertices = new Vector3[] {
                new Vector3(0, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
            };

        // Initialize the ArrayMesh.
        var arrMesh = new ArrayMesh();
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;

        // Create the Mesh.
        arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        var m = new MeshInstance3D();
        m.Mesh = arrMesh;
        AddChild(m);
    }
}