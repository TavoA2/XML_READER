using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SGM_WEBAPI.Models
{
    [Serializable]
    [XmlRoot("tss_loan_request")]
    public class LoanRequest
    {
        [XmlElement("data")]
        public List<DataItem> DataItems { get; set; }

        [XmlArray("income_amount")]
        [XmlArrayItem("item")]
        public List<IncomeItem> IncomeAmount { get; set; } = new List<IncomeItem>();

        [XmlElement("employer_length_months")]
        public int EmployerLengthMonths { get; set; }

        public int LoanAmountDesired { get; set; }

        public string HomeState { get; set; }

        public string IncomeFrequency { get; set; }

        public bool Military { get; set; }

        public class DataItem
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlText]
            public string Value { get; set; }
        }

        public class IncomeItem
        {
            public string Company { get; set; }
            public string LastDate { get; set; }
            public string Position { get; set; }
            public decimal Salary { get; set; }
        }

        // Propiedades adicionales según el XML
        [XmlIgnore]
        public DateTime DateOfBirth => new DateTime(GetDataItem("date_dob_y"), GetDataItem("date_dob_m"), GetDataItem("date_dob_d"));

        [XmlIgnore]
        public DateTime IncomeDate1 => new DateTime(GetDataItem("income_date1_y"), GetDataItem("income_date1_m"), GetDataItem("income_date1_d"));

        [XmlIgnore]
        public DateTime IncomeDate2 => new DateTime(GetDataItem("income_date2_y"), GetDataItem("income_date2_m"), GetDataItem("income_date2_d"));

        // Método auxiliar para obtener el valor de un nodo específico
        private int GetDataItem(string name)
        {
            return int.Parse(DataItems.Find(item => item.Name == name).Value);
        }
    }
}
