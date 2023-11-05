// Modified with the help of Agentalex9
using System.Text;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UndertaleModLib.Util;

EnsureDataLoaded();

bool padded = (!ScriptQuestion("Export all sprites unpadded?"));

string texFolder = GetFolder(FilePath) + "Export_Sprites" + Path.DirectorySeparatorChar;
TextureWorker worker = new TextureWorker();
if (Directory.Exists(texFolder))
{
    ScriptError("A sprites export already exists. Please remove it.", "Error");
    return;
}

Directory.CreateDirectory(texFolder);
Directory.CreateDirectory(texFolder + "/images");

SetProgressBar(null, "Sprites", 0, Data.Sprites.Count);
StartProgressBarUpdater();

await DumpSprites();
worker.Cleanup();

await StopProgressBarUpdater();
HideProgressBar();
ScriptMessage("Export Complete.\n\nLocation: " + texFolder);


string GetFolder(string path)
{
    return Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
}

async Task DumpSprites()
{
    await Task.Run(() => Parallel.ForEach(Data.Sprites, DumpSprite));
}

void DumpSprite(UndertaleSprite sprite)
{
    for (int i = 0; i < sprite.Textures.Count; i++)
        if (sprite.Textures[i]?.Texture != null)
        {
            worker.ExportAsPNG(sprite.Textures[i].Texture, texFolder + "/images/" + sprite.Name.Content + "_" + i + ".png", null, padded); // Include padding to make sprites look neat!
            using (StreamWriter writer = new StreamWriter(texFolder + "sprites_" + sprite.Name.Content + ".sprite.gmx"))// This part by NintenHero
            {
                writer.WriteLine("<sprite>");
                writer.WriteLine("  <type>" + 0 + "</type>");//Todo
                writer.WriteLine("  <xorig>" + sprite.OriginX + "</xorig>");
                writer.WriteLine("  <yorigin>" + sprite.OriginY + "</yorigin>");
                writer.WriteLine("  <colkind>" + 1 + "</colkind>");//Todo
                writer.WriteLine("  <coltolerance>" + 0 + "</coltolerance>");//Todo
                writer.WriteLine("  <sepmasks>" + (int)sprite.SepMasks + "</sepmasks>");
                writer.WriteLine("  <bboxmode>" + sprite.BBoxMode + "</bboxmode>");
                writer.WriteLine("  <bbox_left>" + sprite.MarginLeft + "</bbox_left>");
                writer.WriteLine("  <bbox_right>" + sprite.MarginRight + "</bbox_right>");
                writer.WriteLine("  <bbox_top>" + sprite.MarginTop + "</bbox_top>");
                writer.WriteLine("  <bbox_bottom>" + sprite.MarginBottom + "</bbox_bottom>");
                writer.WriteLine("  <HTile>" + 0 + "</HTile>");//Todo
                writer.WriteLine("  <VTile>" + 0 + "</VTile>");//Todo
                writer.WriteLine("  <TextureGroups>");
                writer.WriteLine("    <TextureGroup0>" + 0 + "</TextureGroup0>");//Todo
                writer.WriteLine("  </TextureGroups>");
                writer.WriteLine("  <For3D>" + 0 + "</For3D>");//Todo
                writer.WriteLine("  <width>" + sprite.Width + "</width>");
                writer.WriteLine("  <height>" + sprite.Height + "</height>");
                writer.WriteLine("  <frames>");
                for(int t = 0; t < sprite.Textures.Count; t++)
                    writer.WriteLine("    <frame index=\"" + t + "\">" + sprite.Textures[t].Texture.Name + "</frame>");
                writer.WriteLine("  </frames>");
                writer.WriteLine("</sprite>");
            }
        }
    IncrementProgressParallel();
}