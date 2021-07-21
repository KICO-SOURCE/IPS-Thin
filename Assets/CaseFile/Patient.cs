using System;

namespace Assets.CaseFile
{
    public class Patient
    {
        public int PatientId { get; set; }

        public int SurgeryId { get; set; }

        public string PatientName { get; set; }

        public string PatientFirstName { get; set; }

        public string PatientLastName { get; set; }

        public string CaseNumber { get; set; }

        public string Leftright { get; set; }

        public string SurgeonName { get; set; }

        public string Dob { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime SurgeryDate { get; set; }

        public DateTime DateOfScan { get; set; }

        public string Mrn { get; set; }

        public int Gender { get; set; }

        public int Hospital { get; set; }

        public string SegmentationPath { get; set; }

        public DateTime PostOpDateOfScan { get; set; }

        public string PostOpCaseCode { get; set; }

        public Patient()
        {
        }

        public string ToString()
        {
            return $"Patient ID: {PatientId}\n" +
                   $"Surgery ID: {SurgeryId}\n" +
                   $"Name: {PatientFirstName} {PatientLastName}\n" +
                   $"Case Number: {CaseNumber}\n" +
                   $"Side: {Leftright}\n" +
                   $"Surgeon: {SurgeonName}\n" +
                   $"Date of birth: {Dob}\n" +
                   $"Date of creation: {CreationDate.ToString()}\n" +
                   $"Date of surgery: {SurgeryDate.ToString()}\n" +
                   $"Date of scan: {DateOfScan.ToString()}\n" +
                   $"MRN: {Mrn}" +
                   $"Gender: {Gender}\n" +
                   $"Hospital: {Hospital}\n" +
                   $"Segmentation path: {SegmentationPath}\n" +
                   $"PostOp case code: {PostOpCaseCode}\n" +
                   $"PostOp date of scan: {PostOpDateOfScan.ToString()}";
        }
    }
}
