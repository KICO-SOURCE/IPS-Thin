using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Sandbox
{
    /// <summary>
    /// Mesh vertices population manager
    /// </summary>
    public class MeshPointDataManager : MonoBehaviour
    {
        #region Private Fields
        int Index = 0;

        #endregion

        #region Public Fields

        public GameObject PointList;
        public GameObject PointListTemplate;
        public GameObject viewPrefab;
        public GameObject row;

        #endregion

        #region Private Methods

        /// <summary>
        /// Sort mesh vertices by x
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetMeshVerticesSortByX(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.OrderBy(p => p.x).ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// Sort mesh vertices by y
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetMeshVerticesSortByY(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.OrderBy(p => p.y).ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// Sort mesh vertices by z
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetMeshVerticesSortByZ(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.OrderBy(p => p.z).ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// First 100 mesh vertices
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetFirst100MeshVerticesList(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
                if (Index == 100) break;
            }
        }


        /// <summary>
        /// First N mesh vertices
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetFirstNMeshVerticesList(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            int n = 250;
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
                if (Index == n) break;
            }
        }


        /// <summary>
        /// Loaded bone vertices list
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetLoadedBoneMeshVerticesList(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            int Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// Optional bone vertices list
        /// </summary>
        /// <param name="mesh">optional bone mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetOptionalBoneMeshVerticesList(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// Filter the mesh vertices
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetFilteredMeshVerticesList(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            int startIndx = 100;
            int endIndx = 200;
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            Index = 0;
            foreach (var point in points)
            {
                if (Index > startIndx && Index < endIndx)
                {
                    meshPoints.Add(Index, point);
                }
                Index++;
            }
        }

        #endregion


        /// <summary>
        /// Display the mesh vertices list in SAndBox UI
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        public void ShowMeshVerticesList(GameObject uiParent, GameObject mesh)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/MeshVertexList");
            viewPrefab = UnityEngine.Object.Instantiate(prefab, uiParent.transform);

            //// [ left - bottom ]
            //viewPrefab.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(uiParent.GetComponent<RectTransform>().rect.height, 0);
            //// [ right - top ]
            //viewPrefab.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0,0);

            var viewport = viewPrefab.transform.Find("Viewport").gameObject;
            var content = viewport.transform.Find("Content").gameObject;
            PointList = content.transform.Find("PointList").gameObject;
            GameObject point = UnityEngine.Object.Instantiate(PointList, content.transform);
            PointListTemplate = PointList.transform.Find("PointListTemplate").gameObject;

            var filter = mesh.GetComponent<MeshFilter>();
            Mesh sampleMesh = filter.mesh;

            for (int i = 0; i< 100/*sampleMesh.vertices.Count()*/; i++)
            {
                Vector3 MeshPoint = new Vector3(1, 2, 3);
                row = UnityEngine.Object.Instantiate(PointListTemplate, point.transform);
                row.transform.localPosition = new Vector3(0, i * -50, 0);

                var cell = row.transform.Find("NO");
                var text = cell.GetComponent<TextMeshProUGUI>();
                text.text = i.ToString();
                text.color = Color.black;
                text.fontSize = 15;

                cell = row.transform.Find("X");
                var text1 = cell.GetComponent<TextMeshProUGUI>();
                text1.text = sampleMesh.vertices[i].x.ToString();
                text1.color = Color.black;
                text1.fontSize = 15;

                cell = row.transform.Find("Y");
                var text2 = cell.GetComponent<TextMeshProUGUI>();
                text2.text = sampleMesh.vertices[i].y.ToString();
                text2.color = Color.black;
                text2.fontSize = 15;

                cell = row.transform.Find("Z");
                var text3 = cell.GetComponent<TextMeshProUGUI>();
                text3.text = sampleMesh.vertices[i].z.ToString();
                text3.color = Color.black;
                text3.fontSize = 15;

                row.SetActive(true);
            }

        }

    }
}
