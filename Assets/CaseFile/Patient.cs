using Assets.CaseFile.Enums;
using System;
using System.ComponentModel;

namespace Assets.CaseFile
{
    public class Patient : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _patientId;
        private int _surgeryId;
        private string _patientName;
        private string patientFirstName;
        private string patientLastName;
        private string caseNumber;
        private Leftright _leftright;
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
            set
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
            set
            {
                _surgeryId = value;
                OnPropertyChanged(nameof(SurgeryId));
            }
        }

        public string PatientName
        {
            get => _patientName;
            set
            {
                _patientName = value;
                OnPropertyChanged(nameof(PatientName));
            }
        }

        public string PatientFirstName
        {
            get => patientFirstName;
            set
            {
                patientFirstName = value;
                OnPropertyChanged(nameof(PatientFirstName));
            }
        }

        public string PatientLastName
        {
            get => patientLastName;
            set
            {
                patientLastName = value;
                OnPropertyChanged(nameof(PatientLastName));
            }
        }

        public string CaseNumber
        {
            get => caseNumber;
            set
            {
                caseNumber = value;
                OnPropertyChanged(nameof(CaseNumber));
            }
        }

        public Leftright Leftright
        {
            get => _leftright;
            set
            {
                _leftright = value;
                OnPropertyChanged(nameof(Leftright));
            }
        }

        public string SurgeonName
        {
            get => surgeonName;
            set
            {
                surgeonName = value;
                OnPropertyChanged(nameof(SurgeonName));
            }
        }

        public string Dob
        {
            get => dob;
            set
            {
                dob = value;
                OnPropertyChanged(nameof(Dob));
            }
        }

        public DateTime CreationDate
        {
            get => creationDate;
            set
            {
                creationDate = value;
                OnPropertyChanged(nameof(CreationDate));
            }
        }

        public DateTime SurgeryDate
        {
            get => surgeryDate;
            set
            {
                surgeryDate = value;
                OnPropertyChanged(nameof(SurgeryDate));
            }
        }

        public DateTime DateOfScan
        {
            get => dateOfScan;
            set
            {
                dateOfScan = value;
                OnPropertyChanged(nameof(DateOfScan));
            }
        }

        public string Mrn
        {
            get => mrn;
            set
            {
                mrn = value;
                OnPropertyChanged(nameof(Mrn));
            }
        }

        public int Gender
        {
            get => gender;
            set
            {
                gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        public int Hospital
        {
            get => hospital;
            set
            {
                hospital = value;
                OnPropertyChanged(nameof(Hospital));
            }
        }

        public string SegmentationPath
        {
            get => segmentationPath;
            set
            {
                segmentationPath = value;
                OnPropertyChanged(nameof(SegmentationPath));
            }
        }

        public DateTime PostOpDateOfScan
        {
            get => postOpDateOfScan;
            set
            {
                postOpDateOfScan = value;
                OnPropertyChanged(nameof(PostOpDateOfScan));
            }
        }

        public string PostOpCaseCode
        {
            get => postOpCaseCode;
            set
            {
                postOpCaseCode = value;
                OnPropertyChanged(nameof(PostOpCaseCode));
            }
        }

        public Patient()
        {
        }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
