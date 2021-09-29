using Assets.CaseFile;
using Assets.Viewport.Table;
using Ips.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Viewport
{
    public class AnatomicalTableView : IView
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
        private GameObject AnatomicalTable;

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
        public AnatomicalTableView()
        {
            viewPrefab = Resources.Load<GameObject>(ViewportPrefabPath);
        }

        #endregion

        /// <summary>
        /// Create the view.
        /// </summary>
        public void CreateView()
        {
            // Creating anamtomical measurement table data and table view
            var tableData = CreateanatomicTableData();
            var cellData = CreateAnatomicalCellData(tableData.ColumnCount, tableData.RowCount);
            CreateAnatomicMeasurementsTable(cellData, tableData);
        }

        private List<CellData> CreateAnatomicalCellData(int columnCount, int rowCount)
        {
            GameObject obj = new GameObject();

            double flexion = MeasurementUtils.MeasureFlexion(obj.transform);
            var feValue= Math.Abs(Math.Round(flexion)).ToString();

            double varus = MeasurementUtils.MeasureVarusValgus(obj.transform);
            var varusValue= Math.Abs(Math.Round(varus)).ToString();

            double axis = MeasurementUtils.MeasureAnatomicalToMechanicalAngle(obj.transform);
            var anatomicalAxis= Math.Abs(Math.Round(axis)).ToString();

            var feLabel = flexion < 0 ? " Extension" : " Flexion";
            string vvLabel;
            if (Patient.Instance.Leftright.ToLower() == "left")
            {
                if (varus > 0)
                {
                    vvLabel = "Varus";
                }
                else
                {
                    vvLabel = "Valgus";
                }
            }
            else
            {
                if (varus < 0)
                {
                    vvLabel = "Varus";
                }
                else
                {
                    vvLabel = "Valgus";
                }
            }

            CellData cellData;
            List<CellData> cellDataList = new List<CellData>();
            cellData = new CellData();
            cellData.TextColor = Color.red;
            cellData.CellText = "Coronal";
            cellDataList.Add(cellData);

            cellData = new CellData();
            cellData.TextColor = Color.red;
            cellData.CellText = varusValue + "° "+vvLabel;
            cellDataList.Add(cellData);

            cellData = new CellData();
            cellData.TextColor = Color.red;
            cellData.CellText = "Sagittal";
            cellDataList.Add(cellData);

            cellData = new CellData();
            cellData.TextColor = Color.red;
            cellData.CellText = feValue + "° " + feLabel;
            cellDataList.Add(cellData);

            cellData = new CellData();
            cellData.TextColor = Color.red;
            cellData.CellText = "Anatomic Axis";
            cellDataList.Add(cellData);

            cellData = new CellData();
            cellData.TextColor = Color.red;
            cellData.CellText = anatomicalAxis + "° to the Mechanical Axis";
            cellDataList.Add(cellData);
            return cellDataList;
        }


        /// <summary>
        /// Create new TableView instance.
        /// </summary>
        private TableData CreateanatomicTableData()
        {
            TableData tableData = new TableData();
            tableData.cellData = new List<CellData>();

            tableData.ColumnCount = 2;
            tableData.RowCount = 3;
            tableData.HeaderBackground = Color.white;
            tableData.HeaderTextColor = Color.black;
            tableData.HeaderTextSize = 22;
            tableData.HeaderTextAreaHeight = 50;
            tableData.HeaderTextAreaWidth = 400;
            tableData.HeaderText = "Anatomic Measurements";

            return tableData;
        }


        /// <summary>
        /// Create new TableView instance.
        /// </summary>
        private void CreateAnatomicMeasurementsTable(List<CellData> cellDatas, TableData tableData)
        {
            // Table object creration
            parent = ViewportContainer.Instance.Parent.transform.Find(parentTag).gameObject;
            AnatomicalTable = new GameObject("AnatomicalTable");
            AnatomicalTable = UnityEngine.Object.Instantiate(viewPrefab, parent.transform);

            //Header panel creation
            var HeaderPanel = AnatomicalTable.transform.Find("HeaderArea").gameObject;
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
            AnatomicalTable.gameObject.GetComponent<RectTransform>().offsetMin =
                                                        new Vector2(tableGap, parent.GetComponent<RectTransform>().rect.height -
                                                        (cellheight * tableData.RowCount + HeaderPanel.GetComponent<RectTransform>().rect.height - tableGap));
            // [ right - top ]
            AnatomicalTable.gameObject.GetComponent<RectTransform>().offsetMax =
                                                        new Vector2(-(parent.GetComponent<RectTransform>().rect.width / tableData.ColumnCount - tableGap), -tableGap);
            // Table cell creation
            var cellWidth = AnatomicalTable.GetComponent<RectTransform>().rect.width / tableData.ColumnCount;

            // Creating grid layout (table layout)
            var LayoutParent = AnatomicalTable.transform.Find("TableArea").gameObject;
            LayoutParent.AddComponent<CanvasRenderer>();
            var gridLayoutGroup = LayoutParent.AddComponent<GridLayoutGroup>();
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellheight);
            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            LayoutParent.transform.SetParent(AnatomicalTable.transform, false);

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
                cellBackGround.color = Color.white;

                // Adding text to cell
                GameObject tabletextobj = new GameObject("CellText" + cellNo.ToString());
                tabletextobj.AddComponent<RectTransform>();
                tabletextobj.AddComponent<CanvasRenderer>();
                var cellText = tabletextobj.AddComponent<TextMeshProUGUI>();
                cellText.text = cellDatas[cellNo].CellText;
                cellText.color = Color.red;
                cellText.alignment = TextAlignmentOptions.Center;
                cellText.fontSize = 20;
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

            AnatomicalTable.SetActive(true);
        }

        /// <summary>
        /// Deactivates the view.
        /// </summary>
        public void Deactivate()
        {
            AnatomicalTable.SetActive(false);
        }
    }
}
