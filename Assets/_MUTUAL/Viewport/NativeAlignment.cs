using Assets.CaseFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._MUTUAL.Viewport
{
    #region Constructor

    /// <summary>
    /// Create new NativeAlignment instance.
    /// </summary>
    public class NativeAlignment : Viewport
    {
        public NativeAlignment(Patient patient)
        {
            Views.Add(new TableView() { Postion = new Vector2(0, 0), Size = new Vector2(50, 50) });
            Views.Add(new ImageView() { Postion = new Vector2(0, 0), Size = new Vector2(50, 50) });
        }

    }

    #endregion
}
