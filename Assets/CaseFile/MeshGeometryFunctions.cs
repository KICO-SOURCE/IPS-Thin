using System.Text;
using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

namespace Assets.CaseFile
{
    public class MeshGeometryFunctions
    {
        private static string _header;
        private static readonly Regex VertexRegex = new Regex(@"vertex\s*(\S*)\s*(\S*)\s*(\S*)", RegexOptions.Compiled);

        internal static Mesh GetXmlMeshGeometry3D(XmlTextReader reader,
                                    bool switchCoordinateSystem = true)
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
                                    if (switchCoordinateSystem)
                                    {
                                        points.Add(new Vector3(-y, z, x));
                                    }
                                    else
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
                                    if (switchCoordinateSystem)
                                    {
                                        indeces.Add(node3Index + ndOffset);
                                        indeces.Add(node2Index + ndOffset);
                                        indeces.Add(node1Index + ndOffset);
                                    }
                                    else
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
                        Debug.Log(ex.Message);
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
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
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

        /// <summary>
        /// Read the stl file
        /// </summary>
        /// <param name="fileName">File path</param>
        /// <returns>Return mesh data.</returns>
        public static MeshData ReadStl(string fileName)
        {
            if (!System.IO.File.Exists(fileName)) return null;
            MeshData m = null;

            try
            {
                using (Stream stream = File.OpenRead(fileName))
                {
                    m = ReadStlascii(stream);

                }
            }
            catch
            {
            }

            if (m != null && m.vertices.Count > 0) return m;


            try
            {
                using (Stream stream = File.OpenRead(fileName))
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, true))
                    {
                        m = ReadStlBinary(reader);
                    }
                }
            }
            catch
            {
            }



            return m;
        }

        /// <summary>
        /// Read ASCII stl files.
        /// </summary>
        /// <param name="stream">Memory stream</param>
        /// <returns>Return mesh data.</returns>
        public static MeshData ReadStlascii(Stream stream)
        {
            MeshData output = new MeshData();
            output.indexFormat = IndexFormat.UInt32;
            var reader = new StreamReader(stream);
            List<Vector3> vertices = new List<Vector3>();
            try
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        continue;
                    }

                    line = line.Trim();

                    if (line.Length == 0 || /*line.StartsWith("\0") ||*/ line.StartsWith("#") || line.StartsWith("!")
                        || line.StartsWith("$"))
                    {
                        continue;
                    }

                    string id, values;
                    ParseLine(line, out id, out values);
                    switch (id)
                    {
                        case "solid":
                            _header = values.Trim();
                            break;
                        case "facet":
                            List<Vector3> pts = ReadFace(reader, values);
                            foreach (var v in pts) vertices.Add(v);
                            break;
                        case "endsolid":
                            break;
                    }
                }
                output.vertices = vertices;
            }
            catch
            {

            }

            try
            {
                List<int> triangles = new List<int>();
                if (output.vertices.Count > 3)
                {
                    for (int i = 0; i < output.vertices.Count; i++)
                    {
                        triangles.Add(i);
                    }
                    output.triangles = triangles;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            return output;
        }

        /// <summary>
        /// Read the binary stl.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <returns>Return mesh data.</returns>
        public static MeshData ReadStlBinary(BinaryReader reader)
        {
            if (reader == null)
                return null;

            MeshData stl = new MeshData();
            stl.indexFormat = IndexFormat.UInt32;

            try
            {
                byte[] buffer = new byte[80];


                //Read (and ignore) the header and number of triangles.
                buffer = reader.ReadBytes(80);
                int numberoftris = (int)reader.ReadInt32();

                stl.vertices = new List<Vector3>();
                stl.triangles = new List<int>();

                int count = 0;

                //Read each facet until the end of the stream. Stop when the end of the stream is reached.
                while ((reader.BaseStream.Position != reader.BaseStream.Length))
                {

                    float ni = reader.ReadSingle();
                    float nj = reader.ReadSingle();
                    float nk = reader.ReadSingle();
                    float x1 = reader.ReadSingle();
                    float y1 = reader.ReadSingle();
                    float z1 = reader.ReadSingle();
                    float x2 = reader.ReadSingle();
                    float y2 = reader.ReadSingle();
                    float z2 = reader.ReadSingle();
                    float x3 = reader.ReadSingle();
                    float y3 = reader.ReadSingle();
                    float z3 = reader.ReadSingle();
                    byte[] boolbuff = reader.ReadBytes(2);

                    stl.vertices.Add(new Vector3(x1, y1, z1));
                    stl.vertices.Add(new Vector3(x2, y2, z2));
                    stl.vertices.Add(new Vector3(x3, y3, z3));
                    stl.triangles.Add(count);
                    stl.triangles.Add(count + 1);
                    stl.triangles.Add(count + 2);
                    count += 3;
                }
            }
            catch (Exception err)
            {
                Debug.Log(err.Message);
                //  throw new Exception(string.Format("Error reading STL file.\n{0}", err.Message));
            }

            return stl;
        }

        /// <summary>
        /// Parse the file data line by line
        /// </summary>
        /// <param name="line">Line to parse.</param>
        /// <param name="id">Id of the line.</param>
        /// <param name="values">Parsed line data</param>
        private static void ParseLine(string line, out string id, out string values)
        {
            line = line.Trim();
            int idx = line.IndexOf(' ');
            if (idx == -1)
            {
                id = line;
                values = string.Empty;
            }
            else
            {
                id = line.Substring(0, idx).ToLower();
                values = line.Substring(idx + 1);
            }
        }

        /// <summary>
        /// Read the face data from the stl data
        /// </summary>
        /// <param name="reader">Stream reader</param>
        /// <param name="normal">Facet normal</param>
        /// <returns>Face data</returns>
        private static List<Vector3> ReadFace(StreamReader reader, string normal)
        {

            List<Vector3> points = new List<Vector3>();
            ReadLine(reader, "outer");
            while (true)
            {
                var line = reader.ReadLine();
                Vector3 point;
                if (TryParseVertex(line, out point))
                {
                    points.Add(point);
                    continue;
                }

                string id, values;
                ParseLine(line, out id, out values);

                if (id == "endloop")
                {
                    break;
                }
            }

            ReadLine(reader, "endfacet");


            return points;
        }

        /// <summary>
        /// Read the line data from the stl file.
        /// </summary>
        /// <param name="reader">Stream reader</param>
        /// <param name="token">String comparison token</param>
        private static void ReadLine(StreamReader reader, string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var line = reader.ReadLine();
            string id, values;
            ParseLine(line, out id, out values);

            if (!string.Equals(token, id, StringComparison.OrdinalIgnoreCase))
            {
                //throw new FileFormatException("Unexpected line.");
                throw new ArgumentNullException("Unexpected line");
            }
        }

        /// <summary>
        /// Parse vertex data.
        /// </summary>
        /// <param name="line">Input vertex data</param>
        /// <param name="point">vertex point.</param>
        /// <returns>Parse status</returns>
        private static bool TryParseVertex(string line, out Vector3 point)
        {
            var match = VertexRegex.Match(line);
            if (!match.Success)
            {
                point = new Vector3();
                return false;
            }

            float x = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            float y = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            float z = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            point = new Vector3(x, y, z);
            return true;
        }

        /// <summary>
        /// Calculate the normal for the given facet
        /// </summary>
        /// <param name="p0">Facet point</param>
        /// <param name="p1">Facet point</param>
        /// <param name="p2">Facet point</param>
        /// <returns>Normal</returns>
        public static Vector3 CalculateFacetNormal(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 u = p1 - p0;
            Vector3 v = p2 - p0;
            Vector3 n = Vector3.Cross(u, v);
            n.Normalize();
            return n;
        }

    }

    public class MeshData
    {
        public IndexFormat indexFormat;
        public List<Vector3> vertices;
        public List<int> triangles;
        internal List<Vector3> facetNormals;
        public List<Vector3> normals;
        public List<Vector2> uv;

        public MeshData()
        {

        }

        public MeshData(Mesh mesh)
        {
            indexFormat = mesh.indexFormat;
            vertices = mesh.vertices.ToList();
            triangles = mesh.triangles.ToList();
            normals = mesh.normals?.ToList();
            uv = mesh.uv?.ToList();
        }

        internal void GenerateFacetNormals()
        {
            facetNormals = new List<Vector3>(triangles.Count / 3);
            Vector3 normal;
            for (int i = 0; i < triangles.Count; i += 3)
            {
                normal = MeshGeometryFunctions.CalculateFacetNormal(
                        vertices[triangles[i]],
                        vertices[triangles[i + 1]],
                        vertices[triangles[i + 2]]
                    );
                facetNormals.Add(normal);
            }
        }

        internal void ApplyTransform(Transform transform)
        {
            vertices = vertices.Select(transform.TransformPoint).ToList();
            if (facetNormals?.Count > 0)
            {
                facetNormals = facetNormals.Select(transform.TransformVector).ToList();
            }
            if (normals?.Count > 0)
            {
                normals = normals.Select(transform.TransformVector).ToList();
            }
        }

        internal MeshData Clone()
        {
            return new MeshData
            {
                indexFormat = this.indexFormat,
                vertices = this.vertices.ToList(),
                triangles = this.triangles.ToList(),
                normals = this.normals?.ToList(),
                uv = this.uv?.ToList()
            };
        }

        internal Mesh ToMesh()
        {
            var mesh = new Mesh
            {
                indexFormat = vertices.Count <= 65536 ? IndexFormat.UInt16 : IndexFormat.UInt32
            };
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = normals?.ToArray();
            mesh.uv = uv?.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}