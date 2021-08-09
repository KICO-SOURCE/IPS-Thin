using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._MUTUAL.Viewport.Table
{

    #region Constructor

    /// <summary>
    /// Create new CellData instance.
    /// </summary>
    public class CellData
    {
        public string CellText;
        public Color TextColor;
        public int FontSize;
        public Color CellBackground;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Create new TableData instance.
    /// </summary>
    public class TableData
    {
        public int RowCount;
        public int ColumnCount;
        public Color HeaderBackground;
        public int HeaderTextSize;
        public Color HeaderTextColor;
        public string HeaderText;
        public int HeaderTextAreaHeight;
        public int HeaderTextAreaWidth;

        public List<CellData> cellData;
    }

    #endregion
}
