using GBX.NET.Engines.Game;
using GBX.NET;
using GBX.NET.Engines.Plug;
using System.Collections.ObjectModel;
using GBX.NET.Engines.GameData;
using GBX.NET.Extensions;
using GBX.NET.LZO;
using System.Linq;
using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using GBX.NET.ZLib;
using GBX.NET.Engines.Scene;

namespace Triangle3DAnimation
{
	internal class Program
	{

		public static void Main(string[] args){
			Program p = new Program();
			if(args.Count() > 0){
				p.TestSigns(args[0]);
			}else{
				Console.Write("No folder provided. Press a key to exit.");
				Console.ReadLine();
			}
		}
	
		public void TestSigns(string folderPath){
			// folderPath = @"C:\YourFolderPath";
			Console.Write("File name prefix for start. Your file name needs to include \"MAPNUMBER\" in its file name: ");
			string startURL = Console.ReadLine();
			if(String.IsNullOrEmpty(startURL)){
				Console.WriteLine("Please write an start sign URL.");
				Environment.Exit(0);
			}
			if(String.IsNullOrEmpty(Path.GetFileName(new Uri(startURL).LocalPath))){
				Console.WriteLine("Please write a valid start sign URL.");
				Environment.Exit(0);
			}
			if(!startURL.Contains("MAPNUMBER")){
				Console.WriteLine("File name needs to include \"MAPNUMBER\".");
				Environment.Exit(0);
			}
			string startURLFileName = Path.GetFileName(new Uri(startURL).LocalPath);

			Console.Write("End sign URL: ");
			string endURL = Console.ReadLine();
			if(String.IsNullOrEmpty(endURL)){
				Console.WriteLine("Please write an end sign URL.");
				Environment.Exit(0);
			}
			if(String.IsNullOrEmpty(Path.GetFileName(new Uri(endURL).LocalPath))){
				Console.WriteLine("Please write a valid end sign URL.");
				Environment.Exit(0);
			}
			string endURLFileName = Path.GetFileName(new Uri(endURL).LocalPath);

			Console.Write("Checkpoint sign URL: ");
			string CPURL = Console.ReadLine();
			if(String.IsNullOrEmpty(CPURL)){
				Console.WriteLine("Please write an end sign URL.");
				Environment.Exit(0);
			}
			if(String.IsNullOrEmpty(Path.GetFileName(new Uri(CPURL).LocalPath))){
				Console.WriteLine("Please write a valid end sign URL.");
				Environment.Exit(0);
			}
			string CPURLFileName = Path.GetFileName(new Uri(CPURL).LocalPath);

			if (Directory.Exists(folderPath))
			{
				Gbx.LZO = new MiniLZO();
				Gbx.ZLib = new ZLib();

				// Get all files in the folder
				string[] files = Directory.GetFiles(folderPath);

				foreach (string file in files)
				{
					string mapNumber = "";
					if(file.ToLower().EndsWith(".map.gbx")){
						string fileName = file.ToLower().Replace(".map.gbx", "");
						mapNumber = fileName.Substring(fileName.Length - 3);
						bool isValidInt = int.TryParse(mapNumber, out int result);
						if(isValidInt){
							string map2 = file;
							CGameCtnChallenge map2_node = GameBox.ParseNode<CGameCtnChallenge>(map2);

							for(int i = 0; i < map2_node.Blocks.Count; i++){
								CGameCtnBlock block = map2_node.Blocks[i];

								if(block.Name.Contains("Start") || block.Name.Contains("Multilap")){
									// CGameCtnBlockSkin newSkin = sampleSkin;	
									// block.Skin = newSkin;
									block.Skin = new CGameCtnBlockSkin();
									block.Skin.ForegroundPackDesc = new PackDesc();
									block.Skin.ParentPackDesc = new PackDesc();
									block.Skin.Text = "";
									// block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk01001000>();
									block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk03059002>();
									block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk03059003>();
									block.Skin.PackDesc = new PackDesc(@"Skins\Any\Advertisement6x1\"+startURLFileName.Replace("MAPNUMPER", mapNumber), null, startURL.Replace("MAPNUMBER", mapNumber));
								}else if(block.Name.Contains("Finish")){
									// CGameCtnBlockSkin newSkin = sampleSkin;	
									block.Skin = new CGameCtnBlockSkin();
									block.Skin.ForegroundPackDesc = new PackDesc();
									block.Skin.ParentPackDesc = new PackDesc();
									block.Skin.Text = "";
									// block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk01001000>();
									block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk03059002>();
									block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk03059003>();
									block.Skin.PackDesc = new PackDesc(@"Skins\Any\Advertisement6x1\"+endURLFileName, null, endURL);
								}else if(block.Name.Contains("Checkpoint")){
									// CGameCtnBlockSkin newSkin = sampleSkin;	
									block.Skin = new CGameCtnBlockSkin();
									block.Skin.ForegroundPackDesc = new PackDesc();
									block.Skin.ParentPackDesc = new PackDesc();
									block.Skin.Text = "";
									// block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk01001000>();
									block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk03059002>();
									block.Skin.CreateChunk<CGameCtnBlockSkin.Chunk03059003>();
									block.Skin.PackDesc = new PackDesc(@"Skins\Any\Advertisement6x1\"+CPURLFileName, null, CPURL);
								}
							}
							
							for (int i = 0; i < map2_node.AnchoredObjects.Count; i++)
							{
								CGameCtnAnchoredObject anchoredObject = map2_node.AnchoredObjects[i];
								if((anchoredObject.ItemModel.Id.Contains("Start") && anchoredObject.ItemModel.Author == "Nadeo") || (anchoredObject.ItemModel.Id.Contains("Multilap") && anchoredObject.ItemModel.Author == "Nadeo")){
									anchoredObject.PackDesc = new PackDesc(@"Skins\Any\Advertisement6x1\"+startURLFileName.Replace("MAPNUMBER", mapNumber), null, startURL.Replace("MAPNUMBER", mapNumber));
								}else if(anchoredObject.ItemModel.Id.Contains("Finish") && anchoredObject.ItemModel.Author == "Nadeo"){
									anchoredObject.Flags = 4;
									anchoredObject.CreateChunk<CGameCtnAnchoredObject.Chunk03101002>();
									anchoredObject.PackDesc = new PackDesc
									(@"Skins\Any\Advertisement6x1\"+endURLFileName, null, endURL);
								}else if(anchoredObject.ItemModel.Id.Contains("Checkpoint") && anchoredObject.ItemModel.Author == "Nadeo"){
									anchoredObject.PackDesc = new PackDesc(@"Skins\Any\Advertisement6x1\"+CPURLFileName, null, CPURL);
								}
							}

							if(map2_node.AnchoredObjects.Count > 0){
								map2_node.CreateChunk<CGameCtnChallenge.Chunk03043065>();
							}
							map2_node.Save(file);
						}
					}
				}
			}
			else
			{
				Console.WriteLine("The folder does not exist.");
			}
		}
	}
}