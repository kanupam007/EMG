using System;

namespace PowerIBroker.Models
{
    public class EmgApiModels
    {
        public long OpnEnrollmentId { get; set; }
        public string Type { get; set; }
        public long EmpID { get; set; }
        public long? PlanCategoryID { get; set; }
        public string WaiveReason { get; set; }
        public long? WaiveOptionID { get; set; }
        public int? Flag { get; set; }
        public long? PlanID { get; set; }
        public string StrPlanID { get; set; }
        public string DependentIDs { get; set; }
        public long? CovID { get; set; }
        public string Unselect { get; set; }
        public string Monthlycost { get; set; }
        public string Slidselectedval { get; set; }
        public string childval { get; set; }
        public string Spval { get; set; }
        public string Password { get; set; }
        public long CompanyID { get; set; }
        public int EvnetID { get; set; }
        public DateTime LiftEvnetDate { get; set; }
        public long LEventId { get; set; }
        public int flatdays { get; set; }
        public decimal ImputedIncome { get; set; }
        public string SelectedDept { get; set; }
        public int SpouseFlatDays { get; set; }
        public int SpouseSelectedRange { get; set; }
        public int IsBeneficiary { get; set; }

     
              
       
        public int EventID { get; set; }
        public DateTime LifeEventDate { get; set; }
        //public long LEventId { get; set; }
        public string FirstNameDependent { get; set; }
        public string MiddleNameDependent { get; set; }
        public string LastNameDependent { get; set; }
        public string NameTitle { get; set; }
        public string SSNDependent { get; set; }
        public string Dependent { get; set; }
        public string Gender { get; set; }
        public Boolean IsDisable { get; set; }
        public Boolean IsSmoker { get; set; }
        public DateTime DOBDependent { get; set; }
        public Boolean IsStudent { get; set; }
        public Boolean SpouseIsEmployed { get; set; }
        public Boolean SpouseHasCoverage { get; set; }
        public string UidSession { get; set; }

    }

}