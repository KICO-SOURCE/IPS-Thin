using System.Text;
using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace Assets.CaseFile
{
    public class MeshGeometryFunctions
    {
        private static string _header;
        private static readonly Regex VertexRegex = new Regex(@"vertex\s*(\S*)\s*(\S*)\s*(\S*)", RegexOptions.Compiled);

        internal static Mesh GetXmlMeshGeometry3D(XmlTextReader reader)
        {
            try
            {
                string currentNode = reader.Name;
                byte[] buffer = new byte[1000];
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms, Encoding.Default, true))
                    {
                        do
                        {
                            int cnt = reader.ReadBase64(buffer, 0, 1000);
                            bw.Write(buffer, 0, cnt);
                        } while (reader.Name == currentNode);
                    }
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    BinaryReader br = null;
                    try
                    {
                        br = new BinaryReader(ms);
                        try
                        {
                            br.BaseStream.Seek(0, SeekOrigin.Begin);

                            bool headerCheck = false;
                            int headerSize = 0;
                            int nextChar;
                            while (headerCheck == false)
                            {
                                nextChar = br.Read();
                                if (nextChar == -1) throw new Exception("Invalid header in Mesh file");
                                else if (nextChar == 0)
                                {
                                    headerSize++;
                                    headerCheck = true;
                                }
                                else
                                {
                                    headerSize++;
                                    if (headerSize == 256) headerCheck = true;
                                }
                            }
                            br.BaseStream.Seek(0, SeekOrigin.Begin);
                            br.BaseStream.Position = headerSize;
                            int fileFlags = (int)br.ReadUInt32();
                            string fileVersion = (string)br.ReadString();
                            if (fileVersion != "KICOdatV1") throw new Exception("Unsupport Mesh File version.");
                            int blockCount = (int)br.ReadUInt32();

                            int fFlag = (int)br.ReadUInt32();
                            int nodeCount = (int)br.ReadUInt32();
                            int triCount = (int)br.ReadUInt32();
                            if (blockCount <= 0) throw new Exception("Invalid block count found in file.");
                            if (nodeCount <= 0) throw new Exception("Invalid node count found in file.");
                            if (triCount <= 0) throw new Exception("Invalid triangle count found in file.");
                            bool asFloat;
                            if ((fileFlags & 1) > 0)
                                asFloat = true;
                            else
                                asFloat = false;

                            int ndOffset = 0;
                            List<Vector3> points = new List<Vector3>();
                            List<int> indeces = new List<int>();
                            List<Vector3> normals = new List<Vector3>();
                            bool hasnormals = true;
                            for (int blk = 0; blk < blockCount; blk++)
                            {
                                int blockFlags = (int)br.ReadUInt32();
                                int blockNodeCount = (int)br.ReadUInt32();
                                int blockTriCount = (int)br.ReadUInt32();
                                bool blockWithVectors;
                                if ((blockFlags & 1) > 0)
                                    blockWithVectors = true;
                                else
                                    blockWithVectors = false;
                                float x, y, z, I, j, k;

                                for (int n = 0; n < blockNodeCount; n++)
                                {
                                    if (asFloat)
                                    {
                                        x = (float)br.ReadSingle();
                                        y = (float)br.ReadSingle();
                                        z = (float)br.ReadSingle();
                                    }
                                    else
                                    {
                                        x = (float)br.ReadDouble();
                                        y = (float)br.ReadDouble();
                                        z = (float)br.ReadDouble();
                                    }
                                    if (blockWithVectors)
                                    {
                                        if (asFloat)
                                        {
                                            I = (float)br.ReadSingle();
                                            j = (float)br.ReadSingle();
                                            k = (float)br.ReadSingle();
                                        }
                                        else
                                        {
                                            I = (float)br.ReadDouble();
                                            j = (float)br.ReadDouble();
                                            k = (float)br.ReadDouble();
                                        }
                                        normals.Add(new Vector3(I, j, k));
                                    }
                                    else
                                    {
                                        hasnormals = false;
                                    }
                                    //TODO: This settings may need to enabled in future
                                    //if (ApplicationSettings.SwitchCoordinateSystem)
                                    //{
                                    //    points.Add(new Vector3(-y, z, x));
                                    //}
                                    //else
                                    {
                                        points.Add(new Vector3(x, y, z));
                                    }
                                }
                                int node1Index;
                                int node2Index;
                                int node3Index;
                                for (int t = 0; t < blockTriCount; t++)
                                {
                                    if (blockNodeCount < 65536)
                                    {
                                        node1Index = (int)br.ReadUInt16();
                                        node2Index = (int)br.ReadUInt16();
                                        node3Index = (int)br.ReadUInt16();
                                    }
                                    else
                                    {
                                        node1Index = (int)br.ReadUInt32();
                                        node2Index = (int)br.ReadUInt32();
                                        node3Index = (int)br.ReadUInt32();
                                    }
                                    //TODO: This settings may need to enabled in future
                                    //if (ApplicationSettings.SwitchCoordinateSystem)
                                    //{
                                    //    indeces.Add(node3Index + ndOffset);
                                    //    indeces.Add(node2Index + ndOffset);
                                    //    indeces.Add(node1Index + ndOffset);
                                    //}
                                    //else
                                    {
                                        indeces.Add(node1Index + ndOffset);
                                        indeces.Add(node2Index + ndOffset);
                                        indeces.Add(node3Index + ndOffset);
                                    }
                                }
                                ndOffset += blockNodeCount;
                                short blockVersion = (short)br.ReadUInt16();

                            }
                            if (!hasnormals)
                            {
                                normals = CalculateNormals(points, indeces);
                                hasnormals = true;
                            }

                            Mesh model = new Mesh
                            {
                                indexFormat = points.Count <= 65536 ? IndexFormat.UInt16 : IndexFormat.UInt32
                            };
                            model.vertices = points.ToArray();
                            model.triangles = indeces.ToArray();

                            if (hasnormals)
                                model.normals = normals.ToArray();
                            return model;
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("Error reading Mesh file.\n{0}", e.Message));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        if (br != null)
                        {
                            br.Close();
                            br = null;
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                return null;
            }
            finally
            {
            }
        }

        public static List<Vector3> CalculateNormals(IList<Vector3> positions, IList<int> triangles)
        {
            var normals = new List<Vector3>();
            for (int i = 0; i < positions.Count; i++)
            {
                normals.Add(new Vector3());
            }

            for (int i = 0; i < triangles.Count; i += 3)
            {
                int index0 = triangles[i];
                int index1 = triangles[i + 1];
                int index2 = triangles[i + 2];
                var p0 = positions[index0];
                var p1 = positions[index1];
                var p2 = positions[index2];
                Vector3 u = p1 - p0;
                Vector3 v = p2 - p0;
                Vector3 w = Vector3.Cross(u, v);
                w.Normalize();
                normals[index0] += w;
                normals[index1] += w;
                normals[index2] += w;
            }

            for (int i = 0; i < normals.Count; i++)
            {
                var w = normals[i];
                w.Normalize();
                normals[i] = w;
            }
            return normals;
        }
    }
}