using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;
using System.Xml;
using Assets.CaseFile.Components;

namespace Assets.CaseFile
{
    public class ComponentLoader
    {
        private const string WebServerBaseUrl = @"http://360ksws.com/360KicoUnityQANavService/";
        private bool LibraryLoaded;
        internal Dictionary<Implant, int> Components { get; private set; }
        protected MonoBehaviour CoroutineHelper
        {
            get
            {
                return SceneManager.GetActiveScene()
                    .GetRootGameObjects()
                    .FirstOrDefault(g => g.GetComponents<MonoBehaviour>().Count() > 0)?
                    .GetComponents<MonoBehaviour>()
                    .FirstOrDefault();
            }
        }

        public void LoadLibrary()
        {
            Components = new Dictionary<Implant, int>();
            var componentLibraryRequest = LoadComponentLibrary(ReadComponentLibrary);
            CoroutineHelper.StartCoroutine(componentLibraryRequest);
        }

        protected IEnumerator LoadComponentLibrary(Action<byte[]> callback)
        {
            byte[] results = null;
            using (UnityWebRequest www = UnityWebRequest.Get(WebServerBaseUrl + "api/KICOCADWebAPIService"))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    results = www.downloadHandler.data;
                }

                if (callback != null)
                {
                    callback(results);
                }
            }
        }

        protected void ReadComponentLibrary(byte[] datFileBytes)
        {
            Components.Clear();
            Components = ReadTheLibrary(datFileBytes);
            LibraryLoaded = true;
        }

        protected T GetComponent<T>(string size, string variant, string brand) where T : Implant
        {
            foreach (var v in Components)
            {
                if (brand == v.Key.Brand && size == v.Key.Size && variant == v.Key.Variant && v.Key.GetType() == typeof(T))
                {
                    return v.Key as T;
                }
            }
            return null;
        }

        protected virtual void OnComponentLoading()
        {
        }

        public void LoadDatFile(int index, ComponentType type, string brand, string side, string variant, string size, Action<int, ComponentType, Implant> onloadCallback)
        {
            OnComponentLoading();
            var resultEnum = this.GetDatFileData(index, type, brand, side, variant, size, onloadCallback);
            CoroutineHelper.StartCoroutine(resultEnum);
        }

        private IEnumerator GetDatFileData(int index, ComponentType type, string brand, string side, string variant, string size, Action<int, ComponentType, Implant> onloadCallback)
        {
            byte[] results = null;
            if (type == ComponentType.PelvisLiner)
            {
                DataEncryption dataEncryption = new DataEncryption();
                variant = dataEncryption.EncryptString(variant);
            }

            if (brand == "Apex" && type == ComponentType.Femur)
            {
                System.Text.Encoding encoding = Encoding.ASCII;
                Byte[] stringBytes = encoding.GetBytes(size);
                StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
                foreach (byte b in stringBytes)
                {
                    sbBytes.AppendFormat("{0:X2}", b);
                }
                size = sbBytes.ToString();
            }

            var url = WebServerBaseUrl + "api/KICOCADWebAPIService?type=" + GetComponentType(type) +
                "&brand=" + brand + "&Variant=" + variant + "&side=" + side + "&size=" + size;
            //Debug.Log(url);

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    results = www.downloadHandler.data;
                }

                ReadImplantDatFile(index, type, results, onloadCallback);
            }
            yield return null;
        }

        private static Dictionary<Implant, int> ReadTheLibrary(byte[] datFileContent)
        {
            Dictionary<Implant, int> componentlibrary = new Dictionary<Implant, int>();
            var path = System.IO.Path.GetTempFileName();
            File.WriteAllBytes(path, datFileContent);

            if (System.IO.File.Exists(path))
            {
                using (XmlTextReader reader = new XmlTextReader(path))
                {
                    int count = 1;

                    reader.Read();
                    while (!reader.EOF)
                    {
                        if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "Component")
                        {
                            string type = reader.GetAttribute("Type");
                            string brand = reader.GetAttribute("Brand");
                            string variant = reader.GetAttribute("Variant");
                            string size = reader.GetAttribute("Size");
                            string side = reader.GetAttribute("Side");

                            Implant comp = null;

                            if (type == "Femur Component") comp = new FemurComponent(brand, variant, size, side, null, null);
                            else if (type == "Patella Component") comp = new PatellaComponent(brand, variant, size, "", null, null);
                            else if (type == "Tibia Insert") comp = new TibiaInsert(brand, variant, size, side, null, null);
                            else if (type == "Tibia Tray")
                            {
                                if (brand == "Saiph" || brand == "Attune" || brand == "Madison" || brand == "BalanSys")
                                {
                                    comp = new TibiaTray(brand, variant, size, side, null, null);
                                }
                                else
                                {
                                    comp = new TibiaTray(brand, brand, size, variant, null, null);
                                }
                            }
                            else if (type == PelvisCup.ComponentTypeName) comp = new PelvisCup(brand, variant, size, side, null, null);
                            else if (type == PelvisLiner.ComponentTypeName) comp = new PelvisLiner(brand, variant, size, side, null, null);
                            else if (type == FemurStem.ComponentTypeName) comp = new FemurStem(brand, variant, size, side, null, null);
                            else if (type == FemurHead.ComponentTypeName) comp = new FemurHead(brand, variant, size, side, null, null);
                            if (comp != null) componentlibrary.Add(comp, count);

                            count++;
                        }
                        if (!reader.Read()) break;
                    }
                }
                File.Delete(path);
            }

            return componentlibrary;
        }

        private void ReadImplantDatFile(int index, ComponentType type, byte[] datFileBytes, Action<int, ComponentType, Implant> onloadCallback)
        {
            Implant implant = null;

            switch(type)
            {
                case ComponentType.Femur:
                    implant = new FemurComponent();
                    break;
                case ComponentType.TibiaTray:
                    implant = new TibiaTray();
                    break;
                case ComponentType.TibiaInsert:
                    implant = new TibiaInsert();
                    break;
                case ComponentType.Patella:
                    implant = new PatellaComponent();
                    break;
                case ComponentType.PelvisCup:
                    implant = new PelvisCup();
                    break;
                case ComponentType.PelvisLiner:
                    implant = new PelvisLiner();
                    break;
                case ComponentType.FemurHead:
                    implant = new FemurHead();
                    break;
                case ComponentType.FemurStem:
                    implant = new FemurStem();
                    break;
                default:
                    break;
            }

            if(implant != null)
            {
                implant.ReadFromFixedpath(datFileBytes);
                onloadCallback?.Invoke(index, type, implant);
            }
        }

        private string GetComponentType(ComponentType componentType)
        {
            switch (componentType)
            {
                case ComponentType.Femur:
                    return "Femur Component";
                case ComponentType.TibiaInsert:
                    return "Tibia Insert";
                case ComponentType.TibiaTray:
                    return "Tibia Tray";
                case ComponentType.Patella:
                    return "Patella Component";
                case ComponentType.PelvisCup:
                    return "Pelvis Cup";
                case ComponentType.PelvisLiner:
                    return "Pelvis Liner";
                case ComponentType.FemurHead:
                    return "Femur Head";
                case ComponentType.FemurStem:
                    return "Femur Stem";
                default:
                    return "";
            }
        }
    }
}