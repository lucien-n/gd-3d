using Godot;
using Godot.Collections;

public partial class TextureLoader : Node
{
    public static Dictionary<string, Dictionary<string, Texture2D>> textures = new();

    public static void LoadTextures()
    {
        textures["blocks"] = GetTexturesInDir("D:/godot/3d-iso/assets/textures/blocks");
    }

    public static Dictionary<string, Texture2D> GetTexturesInDir(string path)
    {
        Dictionary<string, Texture2D> dir_textures = new();

        DirAccess dir = DirAccess.Open(path);
        if (dir == null) return null;

        dir.ListDirBegin();
        string file_name = dir.GetNext();

        while (file_name != "")
        {
            if (!dir.CurrentIsDir() && file_name.EndsWith(".png"))
            {
                string name = file_name.Replace(".png", "");
                string full_path = path + "/" + file_name;
                GD.Print("Loading ", name, " at ", full_path);
                Texture2D texture = ResourceLoader.Load<Texture2D>(full_path);
                dir_textures[name] = texture;
            }

            file_name = dir.GetNext();
        }

        return dir_textures;
    }

    public static Texture2D GetBlockTexture(string block_name)
    {
        if (textures["blocks"].ContainsKey(block_name)) return textures["blocks"][block_name];
        return null;
    }
}