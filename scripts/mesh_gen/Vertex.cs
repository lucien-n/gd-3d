using System.Collections.Generic;
using Godot;

public class Vertex
{
    private static Dictionary<Vector3, Vertex> vertices = new Dictionary<Vector3, Vertex>();
    public Vector3 point;

    private Vertex(Vector3 point)
    {
        this.point = point;
    }

    public static Vertex Get(Vector3 point)
    {
        if (vertices.ContainsKey(point))
        {
            return vertices[point];
        }
        else
        {
            Vertex vertex = new(point);
            vertices.Add(point, vertex);
            return vertex;
        }
    }
}