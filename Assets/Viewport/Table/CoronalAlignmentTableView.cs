using Assets.Viewport;
using Assets.Viewport.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Viewport
{
    public class CoronalAlignmentTableView : IView
    {
        #region Private Constants

        private const string ViewportPrefabPath = "Prefabs/TableView";
        private const string parentTag = "ViewportContainer(Clone)/ViewportArea";
        private const string headerparentTag = "HeaderArea";
        private const string tableparentTag = "TableArea";

        #endregion

        #region Private Members

        private GameObject parent;
        private GameObject viewPrefab;
        private GameObject CoronalPlaneDataTable;

        #endregion

        #region Public Properties

        /// <summary>
        /// Parent element
        /// </summary>
        public GameObject Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// View position
        /// </summary>
        public Vector2 Postion { get; set; }

        /// <summary>
        /// Viewport size
        /// </summary>
        public Vector2 Size { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Create new TableView instance.
        /// </summary>
        public CoronalAlignmentTableView()
        {
            viewPrefab = Resources.Load<GameObject>(ViewportPrefabPath);
        }

        #endregion

        /// <summary>
        /// Create the view.
        /// </summary>
        public void CreateView()
        {
            // Creating coronal plane alignment of the knee data and atble view
            var tableData = CreateCoronalAlignmentKneeTableData();
            var cellData = CreateCoronalAlignmentKneeCellData(tableData.ColumnCount, tableData.RowCount);
            CreateCoronalAlignmentKneeTable(cellData, tableData);
        }

        /// <summary>
        /// Create new TableView instance.
        /// </summary>
        private List<CellData> CreateCoronalAlignmentKneeCellData(int columnCount, int rowCount)
        {
            CellData cellData;
            List<CellData> cellDataList = new List<CellData>();
            for (int i = 0; i < columnCount * rowCount; i++)
            {
                cellData = new CellData();
                cellData.CellBackground = Color.white;
                cellData.FontSize = 22;
                cellData.TextColor = Color.black;
                cellData.CellText = "SampleText" + i.ToString();

                cellDataList.Add(cellData);
            }

            return cellDataList;
        }

        /// <summary>
        /// Create new TableView instance.
        /// </summary>
        private TableData CreateCoronalAlignmentKneeTableData()
        {
            TableData tableData = new TableData();

            tableData.ColumnCount = 2;
            tableData.RowCount = 5;
            tableData.HeaderBackground = Color.white;
            tableData.HeaderTextColor = Color.black;
            tableData.HeaderTextSize = 22;
            tableData.HeaderTextAreaHeight = 50;
            tableData.HeaderTextAreaWidth = 400;
            tableData.HeaderText = "Coronal Plane Alignment of the Knee";

            return tableData;
        }

        /// <summary>
        /// Create new TableView instance.
        /// </summary>
        private void CreateCoronalAlignmentKneeTable(List<CellData> cellDatas, TableData tableData)
        {
            // Table object creration
            parent = ViewportContainer.Instance.Parent.transform.Find(parentTag).gameObject;
            CoronalPlaneDataTable = new GameObject("CoronalPlaneDataTable");
            CoronalPlaneDataTable = UnityEngine.Object.Instantiate(viewPrefab, parent.transform);

            //Header panel creation
            var HeaderPanel = CoronalPlaneDataTable.transform.Find("HeaderArea").gameObject;
            HeaderPanel.AddComponent<CanvasRenderer>();
            var headerOutline = HeaderPanel.AddComponent<Outline>();
            headerOutline.effectDistance = new Vector2(-2.5f, -2.5f);
            var image = HeaderPanel.AddComponent<Image>();
            image.color = tableData.HeaderBackground;

            GameObject headerTextObj = new GameObject("HeaderText");
            var headerTextRect = headerTextObj.AddComponent<RectTransform>();
            headerTextRect.anchorMin = new Vector2(0, 0);
            headerTextRect.anchorMax = new Vector2(1, 1);
            headerTextRect.offsetMin = new Vector2(0, 0);
            headerTextRect.offsetMax = new Vector2(0, 0);
            headerTextObj.AddComponent<CanvasRenderer>();
            var headerText = headerTextObj.AddComponent<TextMeshProUGUI>();
            headerText.text = tableData.HeaderText;
            headerText.color = tableData.HeaderTextColor;
            headerText.alignment = TextAlignmentOptions.Center;
            headerText.fontSize = tableData.HeaderTextSize;
            headerTextObj.transform.SetParent(HeaderPanel.transform, false);

            // Setting table positions
            var cellheight = 40;
            var tableGap = 14;

            // [ left - bottom ]
            CoronalPlaneDataTable.gameObject.GetComponent<RectTransform>().offsetMin =
                                                        new Vector2(tableGap, parent.GetComponent<RectTransform>().rect.height -
                                                        (cellheight * tableData.RowCount + HeaderPanel.GetComponent<RectTransform>().rect.height - tableGap));
            // [ right - top ]
            CoronalPlaneDataTable.gameObject.GetComponent<RectTransform>().offsetMax =
                                                        new Vector2(-(parent.GetComponent<RectTransform>().rect.width / tableData.ColumnCount - tableGap),
                                                        -(tableGap + cellheight * 5));

            var cellWidth = CoronalPlaneDataTable.GetComponent<RectTransform>().rect.width / tableData.ColumnCount;

            // Creating grid layout (table layout)
            var LayoutParent = CoronalPlaneDataTable.transform.Find("TableArea").gameObject;
            LayoutParent.AddComponent<CanvasRenderer>();
            var gridLayoutGroup = LayoutParent.AddComponent<GridLayoutGroup>();
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellheight);
            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            LayoutParent.transform.SetParent(CoronalPlaneDataTable.transform, false);

            var cellCount = tableData.ColumnCount * tableData.RowCount;
            // Creating cell and adding to table
            for (int cellNo = 0; cellNo < cellCount; cellNo++)
            {
                // Creating cell structure
                GameObject cellObj = new GameObject("Cell" + cellNo.ToString());
                cellObj.AddComponent<CanvasRenderer>();
                cellObj.AddComponent<RectTransform>();
                var canvas = cellObj.AddComponent<Canvas>();
                var cellOutline = cellObj.AddComponent<Outline>();
                cellOutline.effectDistance = new Vector2(-2.5f, -2.5f);
                var cellBackGround = cellObj.AddComponent<Image>();
                cellBackGround.color = cellDatas[cellNo].CellBackground;

                // Adding text to cell
                GameObject tabletextobj = new GameObject("CellText" + cellNo.ToString());
                tabletextobj.AddComponent<RectTransform>();
                tabletextobj.AddComponent<CanvasRenderer>();
                var cellText = tabletextobj.AddComponent<TextMeshProUGUI>();
                cellText.text = cellDatas[cellNo].CellText;
                cellText.color = cellDatas[cellNo].TextColor;
                cellText.alignment = TextAlignmentOptions.Center;
                cellText.fontSize = cellDatas[cellNo].FontSize;
                // Adding cell text under cell
                cellText.transform.SetParent(cellObj.transform, false);
                // Adding cell object under the table parent
                cellObj.transform.SetParent(LayoutParent.transform, false);
            }


        }

        /// <summary>
        /// Activates the view.
        /// </summary>
        public void Activate()
        {

            CoronalPlaneDataTable.SetActive(true);
        }

        /// <summary>
        /// Deactivates the view.
        /// </summary>
        public void Deactivate()
        {
            CoronalPlaneDataTable.SetActive(false);
        }
    }
}
