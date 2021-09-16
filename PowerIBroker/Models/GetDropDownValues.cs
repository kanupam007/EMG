using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PowerIBrokerDataLayer;
using PowerIBroker.Areas.Company.Models;


namespace PowerIBroker.Models
{
    public class GetDropDownValues
    {
        PowerIBrokerEntities context = new PowerIBrokerEntities();

        public List<BeneficiaryProperty> GetEmployeeDependentList(string SelectedId, long employeeId)
        {

            var ObjCardType = (from data in context.GetDependentandBeneficiary(employeeId, 0, "", 0, 4, 0)
                               select new BeneficiaryProperty()
                               {
                                   ID = data.ID,
                                   Value = data.FULLName,
                                   IsBeneficiary = data.IsBeneficiary
                               }).ToList();

            return ObjCardType;

        }
        public List<BeneficiaryProperty> GetEmployeeTrustList(long employeeId)
        {

            var ObjTrustType = (from data in context.GetTrustBeneficiary(employeeId)
                                select new BeneficiaryProperty()
                                {
                                    ID = data.ID,
                                    Value = data.FULLName,
                                    IsBeneficiary = 1

                                }).ToList();

            return ObjTrustType;

        }

        //public SelectList GetEmployeeTrustList(string SelectedId, long employeeId)
        //{
        //    SelectList ConList;
        //    var ObjTrustType = (from data in context.EMG_Beneficiary.Where(a => a.EmployeeId == employeeId && a.TrustType == 2)
        //                      select new DropDownProperty()
        //                      {
        //                          ID = data.ID,
        //                          Value = data.TrustName,

        //                      }).ToList();

        //    if (string.IsNullOrEmpty(SelectedId))
        //        ConList = new SelectList(ObjTrustType, "ID", "Value");
        //    else
        //        ConList = new SelectList(ObjTrustType, "ID", "Value", SelectedId);
        //    return ConList;

        //}
        public SelectList GetcompensationType(string SelectedId)
        {
            SelectList ConList;

            var ObjComType = (from data in context.psp_GetDropDownValues("6")
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.Value,
                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;

        }

        public SelectList GetEmployeeType(string SelectedId)
        {
            SelectList ConList;
            var ObjEmpType = (from data in context.psp_GetDropDownValues("5")
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.Value,

                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjEmpType, "ID", "Value");
            else
                ConList = new SelectList(ObjEmpType, "ID", "Value", SelectedId);
            return ConList;

        }
        public SelectList GetPayrollDeductionCode(string SelectedId)
        {
            SelectList ConList;
            var ObjEmpType = (from data in context.psp_GetDropDownValues("5")
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.Value,

                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjEmpType, "ID", "Value");
            else
                ConList = new SelectList(ObjEmpType, "ID", "Value", SelectedId);
            return ConList;

        }
        public SelectList GetGenderType(string SelectedId)
        {
            SelectList ConList;

            var ObjEmpType = (from data in context.psp_GetDropDownValues("8")
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.Value,
                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjEmpType, "ID", "Value");
            else
                ConList = new SelectList(ObjEmpType, "ID", "Value", SelectedId);
            return ConList;
        }
        public SelectList GetMonthlyStatus(string SelectedId)
        {
            SelectList ConList;
            List<SelectListItem> choices = new List<SelectListItem>();
            int i = 1;
            foreach (Enums.MonthlyStatus c in Enum.GetValues(typeof(Enums.MonthlyStatus)))
            {
                choices.Add(new SelectListItem() { Text = c.ToString(), Value = i.ToString() });
                i++;
            }
            var list = choices.OrderByDescending(a => a.Value).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(list, "Value", "Text");
            else
                ConList = new SelectList(list, "Value", "Text", SelectedId);
            return ConList;
        }

        public SelectList GetCompanyDepartment(string SelectedId, long CompanyId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.psp_ManageCompanyEmployee("1", CompanyId, 0)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.Value,

                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;
        }
        public SelectList GetCompanyDivision(string SelectedId, long CompanyId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.EMG_Division.Where(a => a.CompanyId == CompanyId && a.IsActive == true)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.DivisionName,
                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;

        }

        public SelectList GetEmployeeState(string SelectedId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.EmployeeStates
                              select new DropDownProperty()
                              {
                                  ID = data.Id,
                                  Value = data.StateCode,
                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;

        }
        public SelectList GetCompanyEnrollment(string SelectedId, long CompanyId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.EMG_OpenEnrollment.Where(a => a.CompanyID == CompanyId && a.IsActive == true)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.Description,
                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;

        }

        public SelectList GetPayrollDeduction(string SelectedId)
        {
            SelectList ConList;

            var ObjComType = (from data in context.EMG_PlanDeduction.Where(a => a.IsActive == true)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanDeduction,

                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;

        }

        public SelectList GetExpiredEnrollment(long EmployeeId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.GetExpiredOpenEnrollment(EmployeeId)
                              select new DropDownPropertyExpired()
                              {
                                  //ID = Convert.ToInt64(data.Id),
                                  ID = data.UidSession,
                                  Value = data.Description,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");
            return ConList;

        }
        public SelectList GetDependent(long EmployeeId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.tblCompany_Employee_DependentInfo.Where(p => p.IsActive == true && p.EmployeeId == EmployeeId)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.FirstName + " " + data.LastName + " (" + data.Dependent + ") ",

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");
            return ConList;

        }

        public SelectList PlanCategoryReport()
        {
            var idList = new[] { 7, 8, 11, 13 };
            SelectList ConList;
            var ObjComType = (from data in context.EMG_PlanCategory.Where(p => p.IsActive == true && idList.Contains(p.ID)).OrderBy(a => a.SortOrder)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanCategory,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");
            return ConList;

        }
        public SelectList PlanCategoryLifeVLifeReport()
        {
            var idList = new[] { 4, 5 };
            SelectList ConList;
            var ObjComType = (from data in context.EMG_PlanCategory.Where(p => p.IsActive == true && idList.Contains(p.ID)).OrderBy(a => a.SortOrder)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanCategory,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");
            return ConList;

        }
        public SelectList PlanCategoryHealthCareReport()
        {
            var idList = new[] { 1, 2, 3 };
            SelectList ConList;
            var ObjComType = (from data in context.EMG_PlanCategory.Where(p => p.IsActive == true && idList.Contains(p.ID)).OrderBy(a => a.SortOrder)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanCategory,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");
            return ConList;

        }

        public SelectList GetCompanyLocation(string SelectedId, long CompanyId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.psp_ManageCompanyEmployee("2", CompanyId, 0)
                              select new DropDownProperty()
                              {
                                  ID = Convert.ToInt64(data.ID),
                                  Value = data.Value,

                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;

        }
        public SelectList GetEmployeeStates(string SelectedId, long CompanyId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.psp_ManageCompanyEmployee("20", CompanyId, 0)
                              select new DropDownProperty()
                              {
                                  ID = Convert.ToInt64(data.ID),
                                  Value = data.Value,

                              }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;

        }
        public SelectList GetEmployeeStateBeneficiary(string SelectedId = "")
        {
            SelectList ConList;
            var ObjComType = (from data in context.EmployeeStates
                              select new DropDownProperty()
                              {
                                  Value = data.StateCode,
                                  ID = data.Id,
                              }).Distinct().ToList();

            //var ObjComType = (from data in context.EmployeeStates
            //                  select new DropDownProperty()
            //                  {
            //                      ID = data.Id,
            //                      Value = data.StateCode,

            //                  }).ToList();
            if (string.IsNullOrEmpty(SelectedId))
                ConList = new SelectList(ObjComType, "ID", "Value");
            else
                ConList = new SelectList(ObjComType, "ID", "Value", SelectedId);
            return ConList;


        }


        public tblCompanyEmailTemplate GetTemplateContent()
        {
            var data = context.tblCompanyEmailTemplates.Where(a => a.popUpID == "1").FirstOrDefault();
            return data;
        }
        public CompanyMaster GetCompanyBasicInfo(long companyID)
        {
            return context.CompanyMasters.Where(a => a.ID == companyID).FirstOrDefault();
        }
        // Dropdown for property Group Master
        public SelectList GetPropertyGroupMaster(int PlanCategoryId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.EMG_PropertyGroupMaster.Where(p => p.IsActive == true && p.PlanCategoryId == PlanCategoryId)
                              select new DropDownProperty()
                              {
                                  ID = data.GroupId,
                                  Value = data.PropertyGroupName,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");
            return ConList;

        }
        public SelectList GetPlanCategoryMaster()
        {
            SelectList ConList;
            var ObjComType = (from data in context.EMG_PlanCategory.Where(p => p.IsActive == true && p.ID < 4).OrderBy(a => a.SortOrder)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanCategory,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
        public SelectList GetPlanCategoryMasterWelfare()
        {
            var idList = new[] { 4, 5, 7, 8, 11, 13 };
            SelectList ConList;
            var ObjComType = (from data in context.EMG_PlanCategory.Where(p => p.IsActive == true && idList.Contains(p.ID)).OrderBy(a => a.SortOrder)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanCategory,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
        public SelectList GetCategoryMaster()
        {
            SelectList ConList;
            var ObjComType = (from data in context.EMG_PlanCategory.Where(p => p.IsActive == true)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanCategory,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
        public SelectList GetPlanName(long CompanyId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.EMG_InsurancePlans.Where(p => p.IsActive == true && p.CompanyID == CompanyId)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanName,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
        public SelectList GetPlanNameBenefitCensus(long CompanyId)
        {


            SelectList ConList;
            var ObjComType = (from data in context.EMG_InsurancePlans.Where(p => p.IsActive == true && p.CompanyID == CompanyId && p.PlanCategoryID < 4)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanName,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
        public SelectList GetPlanNameBenefitCensusWelfare(long CompanyId)
        {
            List<int?> myInClause = new List<int?>() { 4, 5, 7, 8, 11, 13 };
            SelectList ConList;
            var ObjComType = (from data in context.EMG_InsurancePlans.Where(p => p.IsActive == true && p.CompanyID == CompanyId && myInClause.Contains(p.PlanCategoryID))
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.PlanName,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
        public SelectList GetProvider(long CompanyId)
        {
            SelectList ConList;
            var ObjComType = (from data in context.EMG_PlanProvider.Where(p => p.IsActive == true && p.CompanyID == CompanyId)
                              select new DropDownProperty()
                              {
                                  ID = data.ID,
                                  Value = data.Name,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
        public SelectList GetBrokerMaster()
        {
            SelectList ConList;
            var ObjComType = (from data in context.tblBrokers.Where(p => p.IsActive == true)
                              select new DropDownProperty()
                              {
                                  ID = data.BrokerId,
                                  Value = data.Broker,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
        public SelectList GetReasonMaster()
        {
            SelectList ConList;
            var ObjComType = (from data in context.EMG_Reason.Where(p => p.IsActive == true)
                              select new DropDownProperty()
                              {
                                  ID = data.ReasonId,
                                  Value = data.ReasonName,

                              }).ToList();
            ConList = new SelectList(ObjComType, "ID", "Value");

            return ConList;

        }
    }
    public class DropDownProperty
    {
        public long? ID { get; set; }
        public string Value { get; set; }
    }
    public class DropDownPropertyExpired
    {
        public string ID { get; set; }
        public string Value { get; set; }
    }
    public class BeneficiaryProperty
    {
        public long? ID { get; set; }
        public string Value { get; set; }

        public int IsBeneficiary { get; set; }
    }
}