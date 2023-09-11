public class Face
{
    public Vertex[] vertices;
    public Edge[] edges;

    private void AssignFaceToEdges()
    {
        foreach (var edge in edges)
        {
            edge.AddFace(this);
        }
    }

    public Face(Vertex a, Vertex b, Vertex c, Vertex d)
    {
        vertices = new Vertex[4] { a, b, c, d };
        edges = new Edge[4];
        edges[0] = Edge.Get(a, b);
        edges[1] = Edge.Get(b, c);
        edges[2] = Edge.Get(c, d);
        edges[3] = Edge.Get(d, a);
        AssignFaceToEdges();
    }
}