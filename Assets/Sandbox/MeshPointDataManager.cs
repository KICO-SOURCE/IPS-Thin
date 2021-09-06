using Assets.CaseFile;
using Assets.Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Sandbox
{
    public class MeshPointDataManager : MonoBehaviour
    {

        public GameObject PointList;
        public GameObject PointListTemplate;
        public GameObject viewPrefab;
        public GameObject row;
        public List<GameObject> rowList = new List<GameObject>();
        private const string parentTag = "ViewportArea";


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
                rowList.Add(row);

                row.SetActive(true);
            }
            
        }

    }
}
