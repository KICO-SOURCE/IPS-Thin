
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataModels{
	//Document URL: https://spreadsheets.google.com/feeds/worksheets/1AHaaf_KaUXW4XySWLxf6g4VP3jO-78HVUWkQnpvYxvE/public/basic?alt=json-in-script

	//Sheet SheetLines
	public static DataModelsTypes.SheetLines lines = new DataModelsTypes.SheetLines();
	//Sheet SheetMeasurements
	public static DataModelsTypes.SheetMeasurements measurements = new DataModelsTypes.SheetMeasurements();
	//Sheet SheetLandmarks
	public static DataModelsTypes.SheetLandmarks landmarks = new DataModelsTypes.SheetLandmarks();
	static DataModels(){
		//Static constructor that initialises each sheet data
		lines.Init(); measurements.Init(); landmarks.Init(); 
	}
}


namespace DataModelsTypes{
	public class Lines{
		public string id;
		public enum eReference{
			femur,
			tibia,
		}
		public eReference reference;
		public DataModelsTypes.Landmarks[] points;
		public Color color;

		public Lines(){}

		public Lines(string id, Lines.eReference reference, DataModelsTypes.Landmarks[] points, Color color){
			this.id = id;
			this.reference = reference;
			this.points = points;
			this.color = color;
		}
	}
	public class SheetLines: IEnumerable{
		public System.DateTime updated = new System.DateTime(2021,7,15,16,57,7);
		public readonly string[] labels = new string[]{"id","reference","landmarks[] points","color"};
		private Lines[] _rows = new Lines[4];
		public void Init() {
			_rows = new Lines[]{
					new Lines("Line1",Lines.eReference.femur,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.femoralCenter, DataModels.landmarks.hipCenter },new Color(1f,0f,0f,1f)),
					new Lines("Line2",Lines.eReference.femur,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.lateralEpicondyle, DataModels.landmarks.medialSulcus },new Color(1f,0f,0f,1f)),
					new Lines("Line3",Lines.eReference.tibia,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.lclInsertion, DataModels.landmarks.mclInsertion },new Color(1f,0f,0f,1f)),
					new Lines("Line4",Lines.eReference.femur,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.lateralPosteriorCondyle, DataModels.landmarks.medialPosteriorCondyle },new Color(1f,0f,0f,1f))
				};
		}
			
		public IEnumerator GetEnumerator(){
			return new SheetEnumerator(this);
		}
		private class SheetEnumerator : IEnumerator{
			private int idx = -1;
			private SheetLines t;
			public SheetEnumerator(SheetLines t){
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
		public Lines this[int index]{
			get{
				return _rows[index];
			}
		}
		/// <summary>
		/// Access row item by first culumn string identifier
		/// </summary>
		public Lines this[string id]{
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
		public Lines[] ToArray(){
			return _rows;
		}
		/// <summary>
		/// Random item
		/// </summary>
		/// <returns>Returns a random item.</returns>
		public Lines Random() {
			return _rows[ UnityEngine.Random.Range(0, _rows.Length) ];
		}
		//Specific Items

		public Lines line1{	get{ return _rows[0]; } }
		public Lines line2{	get{ return _rows[1]; } }
		public Lines line3{	get{ return _rows[2]; } }
		public Lines line4{	get{ return _rows[3]; } }

	}
}
namespace DataModelsTypes{
	public class Measurements{
		public string id;
		public enum eType{
			pointToPointDistance,
			pointToPlaneDistance,
			angleMeasure,
			angleToPlane,
			lineBestFit,
			midpointMeasure,
		}
		public eType type;
		public DataModelsTypes.Landmarks[] points;

		public Measurements(){}

		public Measurements(string id, Measurements.eType type, DataModelsTypes.Landmarks[] points){
			this.id = id;
			this.type = type;
			this.points = points;
		}
	}
	public class SheetMeasurements: IEnumerable{
		public System.DateTime updated = new System.DateTime(2021,7,15,16,57,7);
		public readonly string[] labels = new string[]{"id","type","landmarks[] points"};
		private Measurements[] _rows = new Measurements[7];
		public void Init() {
			_rows = new Measurements[]{
					new Measurements("Measure1",Measurements.eType.pointToPointDistance,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.femoralCenter, DataModels.landmarks.pclOrigin }),
					new Measurements("Measure2",Measurements.eType.pointToPlaneDistance,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.femoralCenter, DataModels.landmarks.pclOrigin, DataModels.landmarks.lateralEpicondyle, DataModels.landmarks.lateralPosteriorCondyle }),
					new Measurements("Measure3",Measurements.eType.angleMeasure,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.femoralCenter, DataModels.landmarks.pclOrigin, DataModels.landmarks.lateralEpicondyle, DataModels.landmarks.lateralPosteriorCondyle }),
					new Measurements("Measure4",Measurements.eType.pointToPointDistance,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.femoralCenter, DataModels.landmarks.hipCenter }),
					new Measurements("Measure5",Measurements.eType.angleToPlane,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.femoralCenter, DataModels.landmarks.pclOrigin, DataModels.landmarks.lateralEpicondyle, DataModels.landmarks.lateralPosteriorCondyle }),
					new Measurements("Measure6",Measurements.eType.lineBestFit,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.femoralCenter, DataModels.landmarks.pclOrigin, DataModels.landmarks.lateralEpicondyle, DataModels.landmarks.lateralPosteriorCondyle }),
					new Measurements("Measure7",Measurements.eType.midpointMeasure,new DataModelsTypes.Landmarks[]{ DataModels.landmarks.femoralCenter, DataModels.landmarks.hipCenter })
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

		public Measurements measure1{	get{ return _rows[0]; } }
		public Measurements measure2{	get{ return _rows[1]; } }
		public Measurements measure3{	get{ return _rows[2]; } }
		public Measurements measure4{	get{ return _rows[3]; } }
		public Measurements measure5{	get{ return _rows[4]; } }
		public Measurements measure6{	get{ return _rows[5]; } }
		public Measurements measure7{	get{ return _rows[6]; } }

	}
}
namespace DataModelsTypes{
	public class Landmarks{
		public string id;
		public enum eReference{
			femur,
			tibia,
		}
		public eReference reference;
		public Color color;

		public Landmarks(){}

		public Landmarks(string id, Landmarks.eReference reference, Color color){
			this.id = id;
			this.reference = reference;
			this.color = color;
		}
	}
	public class SheetLandmarks: IEnumerable{
		public System.DateTime updated = new System.DateTime(2021,7,15,16,57,8);
		public readonly string[] labels = new string[]{"id","reference","color"};
		private Landmarks[] _rows = new Landmarks[15];
		public void Init() {
			_rows = new Landmarks[]{
					new Landmarks("pclOrigin",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("femoralCenter",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("greaterTrochanter",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("hipCenter",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("lateralCondyle",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("lateralEpicondyle",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("lateralPosteriorCondyle",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("medialCondyle",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("medialEpicondyle",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("medialPosteriorCondyle",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("medialSulcus",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("midfemurCenter",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("whitesideReference",Landmarks.eReference.femur,new Color(1f,0f,0f,1f)),
					new Landmarks("lclInsertion",Landmarks.eReference.tibia,new Color(1f,0f,0f,1f)),
					new Landmarks("mclInsertion",Landmarks.eReference.tibia,new Color(1f,0f,0f,1f))
				};
		}
			
		public IEnumerator GetEnumerator(){
			return new SheetEnumerator(this);
		}
		private class SheetEnumerator : IEnumerator{
			private int idx = -1;
			private SheetLandmarks t;
			public SheetEnumerator(SheetLandmarks t){
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
		public Landmarks this[int index]{
			get{
				return _rows[index];
			}
		}
		/// <summary>
		/// Access row item by first culumn string identifier
		/// </summary>
		public Landmarks this[string id]{
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
		public Landmarks[] ToArray(){
			return _rows;
		}
		/// <summary>
		/// Random item
		/// </summary>
		/// <returns>Returns a random item.</returns>
		public Landmarks Random() {
			return _rows[ UnityEngine.Random.Range(0, _rows.Length) ];
		}
		//Specific Items

		public Landmarks pclOrigin{	get{ return _rows[0]; } }
		public Landmarks femoralCenter{	get{ return _rows[1]; } }
		public Landmarks greaterTrochanter{	get{ return _rows[2]; } }
		public Landmarks hipCenter{	get{ return _rows[3]; } }
		public Landmarks lateralCondyle{	get{ return _rows[4]; } }
		public Landmarks lateralEpicondyle{	get{ return _rows[5]; } }
		public Landmarks lateralPosteriorCondyle{	get{ return _rows[6]; } }
		public Landmarks medialCondyle{	get{ return _rows[7]; } }
		public Landmarks medialEpicondyle{	get{ return _rows[8]; } }
		public Landmarks medialPosteriorCondyle{	get{ return _rows[9]; } }
		public Landmarks medialSulcus{	get{ return _rows[10]; } }
		public Landmarks midfemurCenter{	get{ return _rows[11]; } }
		public Landmarks whitesideReference{	get{ return _rows[12]; } }
		public Landmarks lclInsertion{	get{ return _rows[13]; } }
		public Landmarks mclInsertion{	get{ return _rows[14]; } }

	}
}