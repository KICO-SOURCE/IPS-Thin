using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.ComponentModel;
using Assets.CaseFile.Enums;

namespace Assets.CaseFile
{
    public class BaseProject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _patientId;
        private int _surgeryId;
        private string _patientName;
        private string patientFirstName;
        private string patientLastName;
        private string caseNumber;
        private Leftright _leftright;
        private ImplantType _implantType;
        private ImplantType _oldImplantType;
        private string surgeonName;
        private string dob;
        private DateTime creationDate;
        private DateTime surgeryDate;
        private DateTime dateOfScan;
        private string mrn;
        private int gender;
        private int hospital;
        private string segmentationPath;
        private DateTime postOpDateOfScan;
        private string postOpCaseCode;

        public int PatientId
        {
            get
            {
                return _patientId;
            }
            protected set
            {
                _patientId = value;
                OnPropertyChanged(nameof(PatientId));
            }
        }
        public int SurgeryId
        {
            get
            {
                return _surgeryId;
            }
            protected set
            {
                _surgeryId = value;
                OnPropertyChanged(nameof(SurgeryId));
            }
        }
        public string PatientName
        {
            get => _patientName;
            protected set
            {
                _patientName = value;
                OnPropertyChanged(nameof(PatientName));
            }
        }
        public string PatientFirstName
        {
            get => patientFirstName;
            protected set
            {
                patientFirstName = value;
                OnPropertyChanged(nameof(PatientFirstName));
            }
        }
        public string PatientLastName
        {
            get => patientLastName;
            protected set
            {
                patientLastName = value;
                OnPropertyChanged(nameof(PatientLastName));
            }
        }
        public string CaseNumber
        {
            get => caseNumber;
            protected set
            {
                caseNumber = value;
                OnPropertyChanged(nameof(CaseNumber));
            }
        }

        internal Leftright leftright
        {
            get => _leftright;
            set
            {
                _leftright = value;
                OnPropertyChanged(nameof(Leftright));
            }
        }
        internal ImplantType implantType
        {
            get => _implantType;
            set
            {
                _implantType = value;
                OnPropertyChanged(nameof(implantType));
            }
        }
        internal ImplantType oldImplantType
        {
            get => _oldImplantType;
            set
            {
                _oldImplantType = value;
                OnPropertyChanged(nameof(oldImplantType));
            }
        }

        public string SurgeonName
        {
            get => surgeonName;
            protected set
            {
                surgeonName = value;
                OnPropertyChanged(nameof(SurgeonName));
            }
        }
        public string Dob
        {
            get => dob;
            protected set
            {
                dob = value;
                OnPropertyChanged(nameof(Dob));
            }
        }
        public DateTime CreationDate
        {
            get => creationDate;
            protected set
            {
                creationDate = value;
                OnPropertyChanged(nameof(CreationDate));
            }
        }
        public DateTime SurgeryDate
        {
            get => surgeryDate;
            protected set
            {
                surgeryDate = value;
                OnPropertyChanged(nameof(SurgeryDate));
            }
        }
        public DateTime DateOfScan
        {
            get => dateOfScan;
            protected set
            {
                dateOfScan = value;
                OnPropertyChanged(nameof(DateOfScan));
            }
        }
        public string Mrn
        {
            get => mrn;
            protected set
            {
                mrn = value;
                OnPropertyChanged(nameof(Mrn));
            }
        }
        public int Gender
        {
            get => gender;
            protected set
            {
                gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }
        public int Hospital
        {
            get => hospital;
            protected set
            {
                hospital = value;
                OnPropertyChanged(nameof(Hospital));
            }
        }
        public string SegmentationPath
        {
            get => segmentationPath;
            protected set
            {
                segmentationPath = value;
                OnPropertyChanged(nameof(SegmentationPath));
            }
        }
        public DateTime PostOpDateOfScan
        {
            get => postOpDateOfScan;
            protected set
            {
                postOpDateOfScan = value;
                OnPropertyChanged(nameof(PostOpDateOfScan));
            }
        }
        public string PostOpCaseCode
        {
            get => postOpCaseCode;
            protected set
            {
                postOpCaseCode = value;
                OnPropertyChanged(nameof(PostOpCaseCode));
            }
        }

        Dictionary<string, Vector3> FemurLandMarks { get; set; }
        public List<KeyLandMarkers> LMReplacedCoord { get; protected set; }
        public List<LandmarkList> LMList { get; protected set; }
        protected int _selectedhistoryversion = -1;

        /// <summary>
        /// Selected alignment type.(0-PreOp, 1-PostOp, 2-PostOpNP)
        /// </summary>
        internal int CurrentAlignmentType
        {
            get
            {
                return 0;
            }
        }

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        internal bool DecryptTheProject(byte[] source, out string filePathOut)
        {
            filePathOut = System.IO.Path.GetTempFileName();
            DataEncryption encryptionFile = new DataEncryption();
            bool chk = encryptionFile.DecryptEncryptFile(source, filePathOut);

            return chk;
        }
        
        protected string ModifyFormat(string dateString)
        {
            try
            {

                DateTime date = DateTime.ParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                return dateString;
            }
            catch
            {
                try
                {
                    DateTime date = ParseDate(dateString);
                    return date.ToString("dd-MM-yyyy");
                }
                catch
                {
                    return dateString;
                }
            }
        }

        protected DateTime ParseDate(string dateString)
        {
            DateTime date = DateTime.Now;
            if (string.IsNullOrEmpty(dateString)) return date;
            string[] formats = { "dd/MM/yyyy h:m:s tt","MM/dd/yyyy h:m:s tt",
                                 "d/M/yyyy h:m:s tt", "M/d/yyyy h:m:s tt",
                                 "dd-MM-yyyy h:m:s tt", "MM-dd-yyyy h:m:s tt",
                                 "d-M-yyyy h:m:s tt", "M-d-yyyy h:m:s tt",
                                 "dd-MM-yyyy", "MM-dd-yyyy", "dd-MMM-yyyy",
                                 "dd/MM/yyyy", "MM/dd/yyyy", "dd/MMM/yyyy" };

            for (int i = 0; i < formats.Length; i++)
            {
                try
                {
                    date = DateTime.ParseExact(dateString, formats[i], CultureInfo.InvariantCulture);
                    break;
                }
                catch
                {
                    if (formats.Length - 1 != i) continue;
                    try
                    {
                        date = DateTime.Parse(dateString);
                    }
                    catch
                    {
                        return date;
                    }
                }
            }
            return date;
        }
    }
}