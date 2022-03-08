
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Data{
	//Document URL: https://spreadsheets.google.com/feeds/worksheets/1-hc5rL1GmgkF1oiHVUVfBr7OzFXQLsR0gYr0-SnzVSU/public/basic?alt=json-in-script

	//Sheet SheetMeasurements
	public static DataTypes.SheetMeasurements measurements = new DataTypes.SheetMeasurements();
	//Sheet SheetLandmark
	public static DataTypes.SheetLandmark landmark = new DataTypes.SheetLandmark();
	static Data(){
		//Static constructor that initialises each sheet data
		measurements.Init(); landmark.Init(); 
	}
}


namespace DataTypes{
	public class Measurements{
		public string id;
		public string type;
		public DataTypes.Landmark[] points;

		public Measurements(){}

		public Measurements(string id, string type, DataTypes.Landmark[] points){
			this.id = id;
			this.type = type;
			this.points = points;
		}
	}
	public class SheetMeasurements: IEnumerable{
		public System.DateTime updated = new System.DateTime(2021,6,22,5,50,39);
		public readonly string[] labels = new string[]{"id","type","landmark[] points"};
		private Measurements[] _rows = new Measurements[3];
		public void Init() {
			_rows = new Measurements[]{
					new Measurements("dummyMeasure1","distance",new DataTypes.Landmark[]{ null }),
					new Measurements("dummyMeasure2","angle2d",new DataTypes.Landmark[]{ null }),
					new Measurements("dummyMeasure3","angle3d",new DataTypes.Landmark[]{ null })
				};
		}
			
		public IEnumerator GetEnumerator(){
			return new SheetEnumerator(this);
		}
		private class SheetEnumerator : IEnumerator{
			private int idx = -1;
			private SheetMeasurements t;
			public SheetEnumerator(SheetMeasurements t){
				this.t = t;
			}
			public bool MoveNext(){
				if (idx < t._rows.Length - 1){
					idx++;
					return true;
				}else{
					return false;
				}
			}
			public void Reset(){
				idx = -1;
			}
			public object Current{
				get{
					return t._rows[idx];
				}
			}
		}
		/// <summary>
		/// Length of rows of this sheet
		/// </summary>
		public int Length{ get{ return _rows.Length; } }
		/// <summary>
		/// Access row item by index
		/// </summary>
		public Measurements this[int index]{
			get{
				return _rows[index];
			}
		}
		/// <summary>
		/// Access row item by first culumn string identifier
		/// </summary>
		public Measurements this[string id]{
			get{
				for (int i = 0; i < _rows.Length; i++) {
					if( _rows[i].id == id){ return _rows[i]; }
				}
				return null;
			}
		}
		/// <summary>
		/// Does an item exist with the following key?
		/// </summary>
		public bool ContainsKey(string key){
			for (int i = 0; i < _rows.Length; i++) {
				if( _rows[i].id == key){ return true; }
			}
			return false;
		}
		/// <summary>
		/// List of items
		/// </summary>
		/// <returns>Returns the internal array of items.</returns>
		public Measurements[] ToArray(){
			return _rows;
		}
		/// <summary>
		/// Random item
		/// </summary>
		/// <returns>Returns a random item.</returns>
		public Measurements Random() {
			return _rows[ UnityEngine.Random.Range(0, _rows.Length) ];
		}
		//Specific Items

		public Measurements dummyMeasure1{	get{ return _rows[0]; } }
		public Measurements dummyMeasure2{	get{ return _rows[1]; } }
		public Measurements dummyMeasure3{	get{ return _rows[2]; } }

	}
}
namespace DataTypes{
	public class Landmark{
		public string id;
		public enum eReference{
			femur,
			tibia,
		}
		public eReference reference;
		public Color colour;

		public Landmark(){}

		public Landmark(string id, Landmark.eReference reference, Color colour){
			this.id = id;
			this.reference = reference;
			this.colour = colour;
		}
	}
	public class SheetLandmark: IEnumerable{
		public System.DateTime updated = new System.DateTime(2021,6,22,5,50,40);
		public readonly string[] labels = new string[]{"id","reference","colour"};
		private Landmark[] _rows = new Landmark[22];
		public void Init() {
			_rows = new Landmark[]{
					new Landmark("PCLOrigin",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("femoralCenter",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("greaterTrochanter",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("hipCenter",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("lateralCondyle",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("lateralEpicondyle",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("lateralPosteriorCondyle",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("medialCondyle",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("medialEpicondyle",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("medialPosteriorCondyle",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("medialSulcus",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("midfemurCenter",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("whitesideReference",Landmark.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmark("LCLInsertion",Landmark.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmark("MCLInsertion",Landmark.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmark("PCLInsertion",Landmark.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmark("Tubercle",Landmark.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmark("lateralEdge",Landmark.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmark("lateralMalleolus",Landmark.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmark("lateralWell",Landmark.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmark("medialMalleolus",Landmark.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmark("medialWell",Landmark.eReference.tibia,new Color(1f,0f,0f,1f))
				};
		}
			
		public IEnumerator GetEnumerator(){
			return new SheetEnumerator(this);
		}
		private class SheetEnumerator : IEnumerator{
			private int idx = -1;
			private SheetLandmark t;
			public SheetEnumerator(SheetLandmark t){
				this.t = t;
			}
			public bool MoveNext(){
				if (idx < t._rows.Length - 1){
					idx++;
					return true;
				}else{
					return false;
				}
			}
			public void Reset(){
				idx = -1;
			}
			public object Current{
				get{
					return t._rows[idx];
				}
			}
		}
		/// <summary>
		/// Length of rows of this sheet
		/// </summary>
		public int Length{ get{ return _rows.Length; } }
		/// <summary>
		/// Access row item by index
		/// </summary>
		public Landmark this[int index]{
			get{
				return _rows[index];
			}
		}
		/// <summary>
		/// Access row item by first culumn string identifier
		/// </summary>
		public Landmark this[string id]{
			get{
				for (int i = 0; i < _rows.Length; i++) {
					if( _rows[i].id == id){ return _rows[i]; }
				}
				return null;
			}
		}
		/// <summary>
		/// Does an item exist with the following key?
		/// </summary>
		public bool ContainsKey(string key){
			for (int i = 0; i < _rows.Length; i++) {
				if( _rows[i].id == key){ return true; }
			}
			return false;
		}
		/// <summary>
		/// List of items
		/// </summary>
		/// <returns>Returns the internal array of items.</returns>
		public Landmark[] ToArray(){
			return _rows;
		}
		/// <summary>
		/// Random item
		/// </summary>
		/// <returns>Returns a random item.</returns>
		public Landmark Random() {
			return _rows[ UnityEngine.Random.Range(0, _rows.Length) ];
		}
		//Specific Items

		public Landmark pCLOrigin{	get{ return _rows[0]; } }
		public Landmark femoralCenter{	get{ return _rows[1]; } }
		public Landmark greaterTrochanter{	get{ return _rows[2]; } }
		public Landmark hipCenter{	get{ return _rows[3]; } }
		public Landmark lateralCondyle{	get{ return _rows[4]; } }
		public Landmark lateralEpicondyle{	get{ return _rows[5]; } }
		public Landmark lateralPosteriorCondyle{	get{ return _rows[6]; } }
		public Landmark medialCondyle{	get{ return _rows[7]; } }
		public Landmark medialEpicondyle{	get{ return _rows[8]; } }
		public Landmark medialPosteriorCondyle{	get{ return _rows[9]; } }
		public Landmark medialSulcus{	get{ return _rows[10]; } }
		public Landmark midfemurCenter{	get{ return _rows[11]; } }
		public Landmark whitesideReference{	get{ return _rows[12]; } }
		public Landmark lCLInsertion{	get{ return _rows[13]; } }
		public Landmark mCLInsertion{	get{ return _rows[14]; } }
		public Landmark pCLInsertion{	get{ return _rows[15]; } }
		public Landmark tubercle{	get{ return _rows[16]; } }
		public Landmark lateralEdge{	get{ return _rows[17]; } }
		public Landmark lateralMalleolus{	get{ return _rows[18]; } }
		public Landmark lateralWell{	get{ return _rows[19]; } }
		public Landmark medialMalleolus{	get{ return _rows[20]; } }
		public Landmark medialWell{	get{ return _rows[21]; } }

	}
}