using NvTriStripDotNet;
using PAKLib;
using SA_Tools;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Color = System.Drawing.Color;

namespace SetConv
{
	static class Program
	{
		static readonly Dictionary<ushort, ushort> idmap = new Dictionary<ushort, ushort>()
		{
			{ 0, 0 }, // ring
			{ 1, 3 }, // spring a
			{ 2, 4 }, // spring b
			{ 3, 7 }, // dash panel
			{ 4, 0x37 }, // swinging spike ball
			{ 7, 0xF }, // emerald
			{ 8, 0xE }, // goal
			{ 9, 9 }, // switch
			{ 0xD, 0xA }, // item box
			{ 0x13, 8 }, // checkpoint
			{ 0x14, 0x31 }, // wall
			{ 0x16, 0x2E }, // sphere
			{ 0x17, 0x2F }, // cylinder
			{ 0x18, 0x30 }, // cube
			{ 0x1A, 0x19 }, // hint
			{ 0x1D, 0x5B }, // kiki
			{ 0x1E, 0x39 }, // rhinotank
			{ 0x1F, 0x38 }, // sweep -> beetle
			{ 0x28, 6 }, // launch ramp
			{ 0x4F, 0x35 }, // updraft
			{ 0x50, 0xE }, // froggy -> goal
			{ 0x51, 0xE }, // goal
			{ 0x52, 0xB }, // item box air
		};

		static readonly int[] itemboxmap = new int[]
		{
			0, // speedup
			10, // invincibility
			1, // five rings
			3, // ten rings
			4, // random rings -> 20 rings
			5, // barrier
			2, // 1up
			6, // bomb
			8, // magnetic barrier
		};

		static Dictionary<int, int> texmap;

		static void Main(string[] args)
		{
			Environment.CurrentDirectory = @"C:\SONICADVENTUREDX\Projects\ECPort";

			List<BMPInfo> textures = new List<BMPInfo>(TextureArchive.GetTextures(@"C:\SONICADVENTUREDX\system\BEACH01.PVM"));
			LandTable landTable = LandTable.LoadFromFile(@"Levels\Emerald Coast\Act 1\LandTable.sa1lvl");
			texmap = new Dictionary<int, int>();
			BMPInfo[] newtexs = TextureArchive.GetTextures(@"C:\SONICADVENTUREDX\system\BEACH03.PVM");
			for (int i = 0; i < newtexs.Length; i++)
			{
				BMPInfo found = textures.FirstOrDefault(a => a.Name.Equals(newtexs[i].Name));
				if (found == null)
				{
					texmap[i] = textures.Count;
					textures.Add(newtexs[i]);
				}
				else
					texmap[i] = textures.IndexOf(found);
			}
			foreach (COL col in LandTable.LoadFromFile(@"Levels\Emerald Coast\Act 3\LandTable.sa1lvl").COL)
			{
				foreach (NJS_MATERIAL mat in ((BasicAttach)col.Model.Attach).Material)
					mat.TextureID = texmap[mat.TextureID];
				landTable.COL.Add(col);
			}
			texmap = new Dictionary<int, int>();
			newtexs = TextureArchive.GetTextures(@"C:\SONICADVENTUREDX\system\BEACH02.PVM");
			for (int i = 0; i < newtexs.Length; i++)
			{
				BMPInfo found = textures.FirstOrDefault(a => a.Name.Equals(newtexs[i].Name));
				if (found == null)
				{
					texmap[i] = textures.Count;
					textures.Add(newtexs[i]);
				}
				else
					texmap[i] = textures.IndexOf(found);
			}
			foreach (COL col in LandTable.LoadFromFile(@"Levels\Emerald Coast\Act 2\LandTable.sa1lvl").COL)
			{
				col.Bounds.Center.Z -= 2000;
				col.Model.Position.Z -= 2000;
				foreach (NJS_MATERIAL mat in ((BasicAttach)col.Model.Attach).Material)
					mat.TextureID = texmap[mat.TextureID];
				landTable.COL.Add(col);
			}

			texmap = new Dictionary<int, int>();
			newtexs = TextureArchive.GetTextures(@"C:\SONICADVENTUREDX\system\OBJ_BEACH.PVM");
			for (int i = 0; i < newtexs.Length; i++)
			{
				BMPInfo found = textures.FirstOrDefault(a => a.Name.Equals(newtexs[i].Name));
				if (found == null)
				{
					texmap[i] = textures.Count;
					textures.Add(newtexs[i]);
				}
				else
					texmap[i] = textures.IndexOf(found);
			}

			PAKFile pak = new PAKFile();
			List<byte> inf = new List<byte>();
			string filenoext = "beach01";
			string longdir = "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
			using (System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel())
			using (Direct3D d3d = new Direct3D())
			using (Device dev = new Device(d3d, 0, DeviceType.Hardware, panel.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(640, 480)))
			{
				for (int i = 0; i < textures.Count; i++)
				{
					using (Texture tex = textures[i].Image.ToTexture(dev))
					using (DataStream str = Surface.ToStream(tex.GetSurfaceLevel(0), ImageFileFormat.Dds))
					using (MemoryStream ms = new MemoryStream())
					{
						str.CopyTo(ms);
						pak.Files.Add(new PAKFile.File(filenoext + '\\' + Path.ChangeExtension(textures[i].Name, ".dds"), longdir + '\\' + Path.ChangeExtension(textures[i].Name, ".dds"), ms.ToArray()));
					}
					int infsz = inf.Count;
					inf.AddRange(Encoding.ASCII.GetBytes(Path.ChangeExtension(textures[i].Name, null)));
					inf.AddRange(new byte[0x1C - (inf.Count - infsz)]);
					inf.AddRange(BitConverter.GetBytes(i + 200));
					inf.AddRange(BitConverter.GetBytes(0));
					inf.AddRange(BitConverter.GetBytes(0));
					inf.AddRange(BitConverter.GetBytes(0));
					inf.AddRange(BitConverter.GetBytes(textures[i].Image.Width));
					inf.AddRange(BitConverter.GetBytes(textures[i].Image.Height));
					inf.AddRange(BitConverter.GetBytes(0));
					inf.AddRange(BitConverter.GetBytes(0x80000000));
				}
			}
			pak.Files.Insert(0, new PAKFile.File(filenoext + '\\' + filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
			pak.Save(@"C:\Program Files (x86)\Steam\steamapps\common\Sonic Adventure 2\mods\Emerald Coast\gd_PC\PRS\beach01.pak");

			List<COL> newcollist = new List<COL>();
			Dictionary<string, Attach> visitedAttaches = new Dictionary<string, Attach>();
			foreach (COL col in landTable.COL.Where((col) => col.Model != null && col.Model.Attach != null))
				ConvertCOL(newcollist, visitedAttaches, col);
			landTable.COL = newcollist;

			Console.WriteLine("Loading Object Definitions:");
			Console.WriteLine("Parsing...");

			LevelData.ObjDefs = new List<ObjectDefinition>();
			Dictionary<string, ObjectData> objdefini =
				IniSerializer.Deserialize<Dictionary<string, ObjectData>>("objdefs.ini");

			List<ObjectData> objectErrors = new List<ObjectData>();
			ObjectListEntry[] objlstini = ObjectList.Load(@"Levels\Emerald Coast\Object List.ini", false);
			Directory.CreateDirectory("dllcache").Attributes |= FileAttributes.Hidden;

			List<KeyValuePair<string, string>> compileErrors = new List<KeyValuePair<string, string>>();

			for (int ID = 0; ID < objlstini.Length; ID++)
			{
				string codeaddr = objlstini[ID].CodeString;

				if (!objdefini.ContainsKey(codeaddr))
					codeaddr = "0";

				ObjectData defgroup = objdefini[codeaddr];
				ObjectDefinition def;

				if (!string.IsNullOrEmpty(defgroup.CodeFile))
				{
					Console.WriteLine("Compiling: " + defgroup.CodeFile);


					def = CompileObjectDefinition(defgroup, out bool errorOccured, out string errorText);

					if (errorOccured)
					{
						KeyValuePair<string, string> errorValue = new KeyValuePair<string, string>(
							defgroup.CodeFile, errorText);

						compileErrors.Add(errorValue);
					}
				}
				else
				{
					def = new DefaultObjectDefinition();
				}

				LevelData.ObjDefs.Add(def);

				// The only reason .Model is checked for null is for objects that don't yet have any
				// models defined for them. It would be annoying seeing that error all the time!
				if (string.IsNullOrEmpty(defgroup.CodeFile) && !string.IsNullOrEmpty(defgroup.Model))
				{
					Console.WriteLine("Loading: " + defgroup.Model);
					// Otherwise, if the model file doesn't exist and/or no texture file is defined,
					// load the "default object" instead ("?").
					if (!File.Exists(defgroup.Model))
					{
						ObjectData error = new ObjectData { Name = defgroup.Name, Model = defgroup.Model, Texture = defgroup.Texture };
						objectErrors.Add(error);
						defgroup.Model = null;
					}
				}

				def.Init(defgroup, objlstini[ID].Name);
				def.SetInternalName(objlstini[ID].Name);
			}

			// Checks if there have been any errors added to the error list and does its thing
			// This thing is a mess. If anyone can think of a cleaner way to do this, be my guest.
			if (objectErrors.Count > 0)
			{
				int count = objectErrors.Count;
				List<string> errorStrings = new List<string> { "The following objects failed to load:" };

				foreach (ObjectData o in objectErrors)
				{
					bool texEmpty = string.IsNullOrEmpty(o.Texture);
					bool texExists = (!string.IsNullOrEmpty(o.Texture) && LevelData.Textures.ContainsKey(o.Texture));
					errorStrings.Add("");
					errorStrings.Add("Object:\t\t" + o.Name);
					errorStrings.Add("\tModel:");
					errorStrings.Add("\t\tName:\t" + o.Model);
					errorStrings.Add("\t\tExists:\t" + File.Exists(o.Model));
					errorStrings.Add("\tTexture:");
					errorStrings.Add("\t\tName:\t" + ((texEmpty) ? "(N/A)" : o.Texture));
					errorStrings.Add("\t\tExists:\t" + texExists);
				}

				// TODO: Proper logging. Who knows where this file may end up
				File.WriteAllLines("SADXLVL2.log", errorStrings.ToArray());

			}

			// Loading SET Layout
			Console.WriteLine("Loading SET items", "Initializing...");

			List<SETItem> setlist = new List<SETItem>();
			SonicRetro.SAModel.SAEditorCommon.UI.EditorItemSelection selection = new SonicRetro.SAModel.SAEditorCommon.UI.EditorItemSelection();
			if (LevelData.ObjDefs.Count > 0)
			{
				string setstr = @"C:\SONICADVENTUREDX\Projects\ECPort\system\SET0100S.BIN";
				if (File.Exists(setstr))
				{
					Console.WriteLine("SET: " + setstr.Replace(Environment.CurrentDirectory, ""));

					setlist = SETItem.Load(setstr, selection);
				}
				setstr = @"C:\SONICADVENTUREDX\Projects\ECPort\system\SET0102B.BIN";
				if (File.Exists(setstr))
				{
					Console.WriteLine("SET: " + setstr.Replace(Environment.CurrentDirectory, ""));

					setlist.AddRange(SETItem.Load(setstr, selection));
				}
				setstr = @"C:\SONICADVENTUREDX\Projects\ECPort\system\SET0101S.BIN";
				if (File.Exists(setstr))
				{
					Console.WriteLine("SET: " + setstr.Replace(Environment.CurrentDirectory, ""));

					List<SETItem> newlist = SETItem.Load(setstr, selection);
					foreach (SETItem item in newlist)
						item.Position.Z -= 2000;
					setlist.AddRange(newlist);
				}
			}

			MatrixStack transform = new MatrixStack();
			List<SETItem> add = new List<SETItem>();
			List<SETItem> del = new List<SETItem>();
			List<PalmtreeData> trees = new List<PalmtreeData>();
			foreach (SETItem item in setlist)
				switch (item.ID)
				{
					case 0xD: // item box
						item.ID = 0xA;
						item.Scale.X = itemboxmap[(int)item.Scale.X];
						break;
					case 0x15: // ring group to rings
						for (int i = 0; i < Math.Min(item.Scale.X + 1, 8); i++)
						{
							if (item.Scale.Z == 1) // circle
							{
								double v4 = i * 360.0;
								Vector3 v7 = new Vector3(
									ObjectHelper.NJSin((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y,
									0,
									ObjectHelper.NJCos((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y);
								transform.Push();
								transform.NJTranslate(item.Position);
								transform.NJRotateObject(item.Rotation);
								Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
								transform.Pop();
								add.Add(new SETItem(0, selection) { Position = pos.ToVertex() });
							}
							else // line
							{
								transform.Push();
								transform.NJTranslate(item.Position);
								transform.NJRotateObject(item.Rotation);
								double v5;
								if (i % 2 == 1)
									v5 = i * item.Scale.Y * -0.5;
								else
									v5 = Math.Ceiling(i * 0.5) * item.Scale.Y;
								Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
								transform.Pop();
								add.Add(new SETItem(0, selection) { Position = pos.ToVertex() });
							}
						}
						del.Add(item);
						break;
					case 0x1A: // tikal -> omochao
						item.ID = 0x19;
						item.Position.Y += 3;
						break;
					case 0x1D: // kiki
						item.ID = 0x5B;
						item.Rotation = new Rotation();
						item.Scale = new Vertex();
						break;
					case 0x1F: // sweep ->beetle
						item.ID = 0x38;
						item.Rotation = new Rotation();
						item.Scale = new Vertex(1, 0, 0);
						break;
					case 0x28: // launch ramp
						item.ID = 6;
						item.Scale.X /= 2.75f;
						item.Scale.Z = 0.799999952316284f;
						break;
					case 0x4F: // updraft
						item.ID = 0x35;
						item.Scale.X = Math.Max(Math.Min(item.Scale.X, 200), 10) / 2;
						item.Scale.Y = Math.Max(Math.Min(item.Scale.Y, 200), 10) / 2;
						item.Scale.Z = Math.Max(Math.Min(item.Scale.Z, 200), 10) / 2;
						break;
					case 0x52: // item box air
						item.ID = 0xB;
						item.Scale.X = itemboxmap[(int)item.Scale.X];
						break;
					// palm trees
					case 32:
					case 33:
					case 34:
					case 35:
						trees.Add(new PalmtreeData((byte)(item.ID - 32), item.Position, item.Rotation));
						del.Add(item);
						break;
					// nonsolid objects
					case 47:
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 59:
					case 62:
					case 63:
					case 64:
					case 70:
						ConvertSETItem(newcollist, item, false, setlist.IndexOf(item));
						del.Add(item);
						break;
						// solid objects
					case 36:
					case 37:
					case 39:
					case 41:
					case 42:
					case 43:
					case 44:
					case 45:
					case 46:
					case 54:
					case 58:
					case 66:
					case 71:
					case 72:
					case 73:
					case 74:
						ConvertSETItem(newcollist, item, true, setlist.IndexOf(item));
						del.Add(item);
						break;
					case 81: // goal
						item.ID = 0xE;
						item.Position.Y += 30;
						break;
					default:
						if (idmap.ContainsKey(item.ID))
							item.ID = idmap[item.ID];
						else
							del.Add(item);
						break;
				}
			setlist.AddRange(add);
			foreach (SETItem item in del)
				setlist.Remove(item);
			setlist.Add(new SETItem(0x55, selection) { Position = new Vertex(6158.6f, -88f, 2384.97f), Scale = new Vertex(3, 0, 0) });
			{
				COL col = new COL() { Model = new ModelFile(@"E:\Bridge Model.sa1mdl").Model, SurfaceFlags = SurfaceFlags.Visible };
				col.Model.Position = new Vertex(2803, -1, 365);
				foreach (NJS_MATERIAL mat in ((BasicAttach)col.Model.Attach).Material)
					mat.TextureID = texmap[mat.TextureID];
				col.Model.ProcessVertexData();
				col.CalculateBounds();
				ConvertCOL(newcollist, new Dictionary<string, Attach>(), col);
				col = new COL() { Model = new ModelFile(@"E:\Bridge Model COL.sa1mdl").Model, SurfaceFlags = SurfaceFlags.Solid };
				col.Model.Position = new Vertex(2803, -1, 365);
				col.Model.ProcessVertexData();
				col.CalculateBounds();
				newcollist.Add(col);
				col = new COL() { Model = new ModelFile(@"E:\BridgeSegment0.sa1mdl").Model, SurfaceFlags = SurfaceFlags.Solid };
				col.Model.ProcessVertexData();
				col.CalculateBounds();
				newcollist.Add(col);
				col = new COL() { Model = new ModelFile(@"E:\BridgeSegment1.sa1mdl").Model, SurfaceFlags = SurfaceFlags.Solid };
				col.Model.ProcessVertexData();
				col.CalculateBounds();
				newcollist.Add(col);
				col = new COL() { Model = new ModelFile(@"E:\BridgeSegment2.sa1mdl").Model, SurfaceFlags = SurfaceFlags.Solid };
				col.Model.ProcessVertexData();
				col.CalculateBounds();
				newcollist.Add(col);
				col = new COL() { Model = new ModelFile(@"E:\BridgeSegment3.sa1mdl").Model, SurfaceFlags = SurfaceFlags.Solid };
				col.Model.ProcessVertexData();
				col.CalculateBounds();
				newcollist.Add(col);
			}
			landTable.SaveToFile(@"C:\Program Files (x86)\Steam\steamapps\common\Sonic Adventure 2\mods\Emerald Coast\LandTable.sa2lvl", LandTableFormat.SA2);
			ByteConverter.BigEndian = true;
			SETItem.Save(setlist, @"C:\Program Files (x86)\Steam\steamapps\common\Sonic Adventure 2\mods\Emerald Coast\gd_PC\set0013_s.bin");
			for (int i = 0; i < 4; i++)
			{
				ModelFile modelFile = new ModelFile($@"C:\SONICADVENTUREDX\Projects\Test\Objects\Levels\Emerald Coast\YASI{i}.sa1mdl");
				foreach (BasicAttach attach in modelFile.Model.GetObjects().Where(a => a.Attach != null).Select(a => a.Attach))
					foreach (NJS_MATERIAL mat in attach.Material)
						mat.TextureID = texmap[mat.TextureID];
				modelFile.SaveToFile($@"C:\Program Files (x86)\Steam\steamapps\common\Sonic Adventure 2\mods\Emerald Coast\YASI{i}.sa1mdl");
			}
			using (StreamWriter sw = File.CreateText(@"E:\Documents\Visual Studio 2017\Projects\LevelTest\LevelTest\pt.c"))
				sw.WriteLine(string.Join(",\r\n", trees));
		}

		private static void ConvertSETItem(List<COL> newcollist, SETItem item, bool solid, int ind)
		{
			NJS_OBJECT obj = new NJS_OBJECT() { Scale = new Vertex(1, 1, 1), Name = item.InternalName + " " + ind };
			BasicAttach attach = new BasicAttach();
			obj.Attach = attach;
			foreach (ModelTransform mt in item.GetObjectDefinition().GetModels(item, new MatrixStack()))
			{
				MatrixStack transform = new MatrixStack();
				transform.LoadMatrix(mt.Transform);
				ProcessModel(mt.Model.Clone(), transform, attach);
			}
			attach.ProcessVertexData();
			attach.CalculateBounds();
			COL col = new COL() { Model = obj, SurfaceFlags = SurfaceFlags.Visible };
			if (solid) col.SurfaceFlags |= SurfaceFlags.Solid | SurfaceFlags.Unclimbable;
			col.CalculateBounds();
			ConvertCOL(newcollist, new Dictionary<string, Attach>(), col);
		}

		private static void ProcessModel(NJS_OBJECT obj, MatrixStack transform, BasicAttach dest)
		{
			transform.Push();
			obj.ProcessTransforms(transform);
			if (obj.Attach != null)
			{
				BasicAttach src = (BasicAttach)obj.Attach;
				ushort vertind = (ushort)dest.Vertex.Length;
				ushort matind = (ushort)dest.Material.Count;
				dest.ResizeVertexes(vertind + src.Vertex.Length);
				for (int i = 0; i < src.Vertex.Length; i++)
				{
					dest.Vertex[i + vertind] = Vector3.TransformCoordinate(src.Vertex[i].ToVector3(), transform.Top).ToVertex();
					dest.Normal[i + vertind] = Vector3.TransformNormal(src.Normal[i].ToVector3(), transform.Top).ToVertex();
				}
				foreach (NJS_MATERIAL mat in src.Material)
				{
					mat.TextureID = texmap[mat.TextureID];
					dest.Material.Add(mat);
				}
				foreach (NJS_MESHSET mesh in src.Mesh)
				{
					mesh.MaterialID += matind;
					mesh.PolyName = "poly_" + SonicRetro.SAModel.Extensions.GenerateIdentifier();
					foreach (Poly poly in mesh.Poly)
						for (int i = 0; i < poly.Indexes.Length; i++)
							poly.Indexes[i] += vertind;
					dest.Mesh.Add(mesh);
				}
			}
			foreach (NJS_OBJECT child in obj.Children)
				ProcessModel(child, transform, dest);
			transform.Pop();
		}

		static NvStripifier nvStripifier = new NvStripifier() { StitchStrips = false, UseRestart = false };
		private static void ConvertCOL(List<COL> newcollist, Dictionary<string, Attach> visitedAttaches, COL col)
		{
			if ((col.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible)
			{
				BasicAttach basatt = (BasicAttach)col.Model.Attach;
				COL newcol = new COL() { Bounds = col.Bounds };
				newcol.SurfaceFlags = SurfaceFlags.Visible;
				newcol.Model = new NJS_OBJECT() { Name = col.Model.Name + "_cnk" };
				newcol.Model.Position = col.Model.Position;
				newcol.Model.Rotation = col.Model.Rotation;
				newcol.Model.Scale = col.Model.Scale;
				string newname = basatt.Name + "_cnk";
				if (visitedAttaches != null && visitedAttaches.ContainsKey(newname))
					newcol.Model.Attach = visitedAttaches[newname];
				else
				{
					ChunkAttach cnkatt = new ChunkAttach(true, true) { Name = newname, Bounds = basatt.Bounds };
					if (visitedAttaches != null)
						visitedAttaches[newname] = cnkatt;
					newcol.Model.Attach = cnkatt;
					VertexChunk vcnk;
					bool hasvcolor = basatt.Mesh.Any(a => a.VColor != null);
					bool hasnormal = !hasvcolor && basatt.Normal?.Length > 0;
					if (hasvcolor)
						vcnk = new VertexChunk(ChunkType.Vertex_VertexDiffuse8);
					else if (hasnormal)
						vcnk = new VertexChunk(ChunkType.Vertex_VertexNormal);
					else
						vcnk = new VertexChunk(ChunkType.Vertex_Vertex);
					List<CachedVertex> cache = new List<CachedVertex>(basatt.Vertex.Length);
					List<List<Strip>> strips = new List<List<Strip>>();
					List<List<List<UV>>> uvs = new List<List<List<UV>>>();
					foreach (NJS_MESHSET mesh in basatt.Mesh)
					{
						List<Strip> polys = new List<Strip>();
						List<List<UV>> us = null;
						bool hasUV = mesh.UV != null;
						bool hasVColor = mesh.VColor != null;
						int currentstriptotal = 0;
						switch (mesh.PolyType)
						{
							case Basic_PolyType.Triangles:
								{
									List<ushort> tris = new List<ushort>();
									Dictionary<ushort, UV> uvmap = new Dictionary<ushort, UV>();
									foreach (Poly poly in mesh.Poly)
										for (int i = 0; i < 3; i++)
										{
											ushort ind = (ushort)cache.AddUnique(new CachedVertex(
												basatt.Vertex[poly.Indexes[i]],
												basatt.Normal[poly.Indexes[i]],
												hasVColor ? mesh.VColor[currentstriptotal] : Color.White,
												mesh.UV?[currentstriptotal]));
											if (hasUV)
												uvmap[ind] = mesh.UV[currentstriptotal];
											++currentstriptotal;
											tris.Add(ind);
										}

									if (hasUV)
										us = new List<List<UV>>();

									System.Diagnostics.Debug.Assert(nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups));

									// Add strips
									for (var i = 0; i < primitiveGroups.Length; i++)
									{
										var primitiveGroup = primitiveGroups[i];
										System.Diagnostics.Debug.Assert(primitiveGroup.Type == NvTriStripDotNet.PrimitiveType.TriangleStrip);

										var stripIndices = new ushort[primitiveGroup.Indices.Length];
										List<UV> stripuv = new List<UV>();
										for (var j = 0; j < primitiveGroup.Indices.Length; j++)
										{
											var vertexIndex = primitiveGroup.Indices[j];
											stripIndices[j] = vertexIndex;
											if (hasUV)
												stripuv.Add(uvmap[vertexIndex]);
										}

										polys.Add(new Strip(stripIndices, false));
										if (hasUV)
											us.Add(stripuv);
									}
								}
								break;
							case Basic_PolyType.Quads:
								{
									List<ushort> tris = new List<ushort>();
									Dictionary<ushort, UV> uvmap = new Dictionary<ushort, UV>();
									foreach (Poly poly in mesh.Poly)
									{
										ushort[] quad = new ushort[4];
										for (int i = 0; i < 4; i++)
										{
											ushort ind = (ushort)cache.AddUnique(new CachedVertex(
												basatt.Vertex[poly.Indexes[i]],
												basatt.Normal[poly.Indexes[i]],
												hasVColor ? mesh.VColor[currentstriptotal] : Color.White,
												mesh.UV?[currentstriptotal]));
											if (hasUV)
												uvmap[ind] = mesh.UV[currentstriptotal];
											++currentstriptotal;
											quad[i] = ind;
										}
										tris.Add(quad[0]);
										tris.Add(quad[1]);
										tris.Add(quad[2]);
										tris.Add(quad[2]);
										tris.Add(quad[1]);
										tris.Add(quad[3]);
									}

									if (hasUV)
										us = new List<List<UV>>();

									System.Diagnostics.Debug.Assert(nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups));

									// Add strips
									for (var i = 0; i < primitiveGroups.Length; i++)
									{
										var primitiveGroup = primitiveGroups[i];
										System.Diagnostics.Debug.Assert(primitiveGroup.Type == NvTriStripDotNet.PrimitiveType.TriangleStrip);

										var stripIndices = new ushort[primitiveGroup.Indices.Length];
										List<UV> stripuv = new List<UV>();
										for (var j = 0; j < primitiveGroup.Indices.Length; j++)
										{
											var vertexIndex = primitiveGroup.Indices[j];
											stripIndices[j] = vertexIndex;
											if (hasUV)
												stripuv.Add(uvmap[vertexIndex]);
										}

										polys.Add(new Strip(stripIndices, false));
										if (hasUV)
											us.Add(stripuv);
									}
								}
								break;
							case Basic_PolyType.NPoly:
							case Basic_PolyType.Strips:
								if (hasUV)
									us = new List<List<UV>>();
								foreach (Strip poly in mesh.Poly.Cast<Strip>())
								{
									List<UV> stripuv = new List<UV>();
									ushort[] inds = (ushort[])poly.Indexes.Clone();
									for (int i = 0; i < poly.Indexes.Length; i++)
									{
										inds[i] = (ushort)cache.AddUnique(new CachedVertex(
											basatt.Vertex[poly.Indexes[i]],
											basatt.Normal[poly.Indexes[i]],
											hasVColor ? mesh.VColor[currentstriptotal] : Color.White));
										if (hasUV)
											stripuv.Add(mesh.UV[currentstriptotal]);
										++currentstriptotal;
									}

									polys.Add(new Strip(inds, poly.Reversed));
									if (hasUV)
										us.Add(stripuv);
								}
								break;
						}
						strips.Add(polys);
						uvs.Add(us);
					}
					foreach (var item in cache)
					{
						vcnk.Vertices.Add(item.vertex);
						if (hasnormal)
							vcnk.Normals.Add(item.normal);
						if (hasvcolor)
							vcnk.Diffuse.Add(item.color);
					}
					vcnk.VertexCount = (ushort)cache.Count;
					switch (vcnk.Type)
					{
						case ChunkType.Vertex_Vertex:
							vcnk.Size = (ushort)(vcnk.VertexCount * 3 + 1);
							break;
						case ChunkType.Vertex_VertexDiffuse8:
							vcnk.Size = (ushort)(vcnk.VertexCount * 4 + 1);
							break;
						case ChunkType.Vertex_VertexNormal:
							vcnk.Size = (ushort)(vcnk.VertexCount * 6 + 1);
							break;
					}
					cnkatt.Vertex.Add(vcnk);
					for (int i = 0; i < basatt.Mesh.Count; i++)
					{
						NJS_MESHSET mesh = basatt.Mesh[i];
						NJS_MATERIAL mat = null;
						if (basatt.Material != null && mesh.MaterialID < basatt.Material.Count)
						{
							mat = basatt.Material[mesh.MaterialID];
							cnkatt.Poly.Add(new PolyChunkTinyTextureID()
							{
								ClampU = mat.ClampU,
								ClampV = mat.ClampV,
								FilterMode = mat.FilterMode,
								FlipU = mat.FlipU,
								FlipV = mat.FlipV,
								SuperSample = mat.SuperSample,
								TextureID = (ushort)mat.TextureID
							});
							cnkatt.Poly.Add(new PolyChunkMaterial()
							{
								SourceAlpha = mat.SourceAlpha,
								DestinationAlpha = mat.DestinationAlpha,
								Diffuse = mat.DiffuseColor,
								Specular = mat.SpecularColor,
								SpecularExponent = (byte)mat.Exponent
							});
						}
						PolyChunkStrip strip;
						if (mesh.UV != null)
							strip = new PolyChunkStrip(ChunkType.Strip_StripUVN);
						else
							strip = new PolyChunkStrip(ChunkType.Strip_Strip);
						if (mat != null)
						{
							strip.IgnoreLight = mat.IgnoreLighting;
							strip.IgnoreSpecular = mat.IgnoreSpecular;
							strip.UseAlpha = mat.UseAlpha;
							strip.DoubleSide = mat.DoubleSided;
							strip.FlatShading = mat.FlatShading;
							strip.EnvironmentMapping = mat.EnvironmentMap;
						}
						for (int i1 = 0; i1 < strips[i].Count; i1++)
						{
							Strip item = strips[i][i1];
							UV[] uv2 = null;
							if (mesh.UV != null)
								uv2 = uvs[i][i1].ToArray();
							strip.Strips.Add(new PolyChunkStrip.Strip(item.Reversed, item.Indexes, uv2, null));
						}
						cnkatt.Poly.Add(strip);
					}
				}
				newcollist.Add(newcol);
			}
			if ((col.SurfaceFlags & ~SurfaceFlags.Visible) != 0)
			{
				int newflags = col.Flags & 0xF;
				if (col.SurfaceFlags.HasFlag(SurfaceFlags.Diggable))
					newflags |= 0x20;
				if (col.SurfaceFlags.HasFlag(SurfaceFlags.Unclimbable))
					newflags |= 0x80;
				if (col.SurfaceFlags.HasFlag(SurfaceFlags.Hurt))
					newflags |= 0x400;
				if (col.SurfaceFlags.HasFlag(SurfaceFlags.CannotLand))
					newflags |= 0x1000;
				col.Flags = newflags;
				newcollist.Add(col);
			}
		}

		private static ObjectDefinition CompileObjectDefinition(ObjectData defgroup, out bool errorOccured, out string errorText)
		{
			ObjectDefinition def;
			errorOccured = false;
			errorText = "";

			string codeType = defgroup.CodeType;
			string dllfile = Path.Combine("dllcache", codeType + ".dll");
			DateTime modDate = DateTime.MinValue;
			if (File.Exists(dllfile)) modDate = File.GetLastWriteTime(dllfile);
			string fp = defgroup.CodeFile.Replace('/', Path.DirectorySeparatorChar);
			if (modDate >= File.GetLastWriteTime(fp))
			{
				def =
					(ObjectDefinition)
						Activator.CreateInstance(
							Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile))
								.GetType(codeType));
			}
			else
			{
				string ext = Path.GetExtension(fp);
				CodeDomProvider pr = null;
				switch (ext.ToLowerInvariant())
				{
					case ".cs":
						pr = new Microsoft.CSharp.CSharpCodeProvider();
						break;
					case ".vb":
						pr = new Microsoft.VisualBasic.VBCodeProvider();
						break;
				}
				if (pr != null)
				{
					// System, System.Core, System.Drawing, SharpDX, SharpDX.Mathematics, SharpDX.Direct3D9,
					// SADXLVL2, SAModel, SAModel.Direct3D, SA Tools, SAEditorCommon
					CompilerParameters para =
						new CompilerParameters(new string[]
						{
							"System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof(SharpDX.Mathematics.Interop.RawBool)).Location,
							Assembly.GetAssembly(typeof(Vector3)).Location, Assembly.GetAssembly(typeof(Device)).Location,
							Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof(LandTable)).Location,
							Assembly.GetAssembly(typeof(EditorCamera)).Location, Assembly.GetAssembly(typeof(SA1LevelAct)).Location,
							Assembly.GetAssembly(typeof(ObjectDefinition)).Location
						})
						{
							GenerateExecutable = false,
							GenerateInMemory = false,
							IncludeDebugInformation = true,
							OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile)
						};
					CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
					if (res.Errors.HasErrors)
					{
						string errors = null;
						foreach (CompilerError item in res.Errors)
							errors += String.Format("\n\n{0}, {1}: {2}", item.Line, item.Column, item.ErrorText);

						errorText = errors;
						errorOccured = true;

						def = new DefaultObjectDefinition();
					}
					else
					{
						def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(codeType));
					}
				}
				else
				{
					def = new DefaultObjectDefinition();
					errorText = "Code DOM Provider was null";
					errorOccured = true;
				}
			}

			return def;
		}
	}

	class CachedVertex : IEquatable<CachedVertex>
	{
		public Vertex vertex;
		public Vertex normal;
		public Color color;
		public UV uv;

		public CachedVertex(Vertex v, Vertex n, Color c)
		{
			vertex = v;
			normal = n;
			color = c;
		}

		public CachedVertex(Vertex v, Vertex n, Color c, UV u)
		{
			vertex = v;
			normal = n;
			color = c;
			uv = u;
		}

		public bool Equals(CachedVertex other)
		{
			if (!vertex.Equals(other.vertex)) return false;
			if (!normal.Equals(other.normal)) return false;
			if (!color.Equals(other.color)) return false;
			if (uv == null && other.uv != null) return false;
			if (other.uv == null) return false;
			if (!uv.Equals(other.uv)) return false;
			return true;
		}
	}

	class PalmtreeData
	{
		public byte id;
		public Vertex position;
		public Rotation rotation;

		public PalmtreeData(byte id, Vertex position, Rotation rotation)
		{
			this.id = id;
			this.position = position;
			this.rotation = rotation;
		}

		public override string ToString()
		{
			return $"{{ {id}, {position.ToStruct()}, {rotation.X.ToCHex()}, {rotation.Y.ToCHex()}, {rotation.Z.ToCHex()} }}";
		}
	}
}
