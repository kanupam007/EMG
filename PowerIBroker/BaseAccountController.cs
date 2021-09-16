
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class BaseAccountController : Controller
    {
        // GET: BaseAccount
        private HttpContextBase Context = null;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            Context = requestContext.HttpContext;
            base.Initialize(requestContext);
           

        }



        #region constructor
        public BaseAccountController()
        {

        }
        #endregion
        public class LoginUser
        {
            public string User { get; set; }
            public int UserId { get; set; }
        }
        private LoginUser _companyUser = null;
        private LoginUser _empser = null;
        public LoginUser CompanyUser
        {
            get
            {
                if (_companyUser == null)
                {
                    _companyUser = new LoginUser();
                }
                if (!string.IsNullOrEmpty(Convert.ToString(Session["EmgAdminID"])))
                {
                    _companyUser.UserId = Convert.ToInt32(Session["EmgAdminID"]);
                    _companyUser.User = "Admin";
                }
                else
                {
                    _companyUser.UserId = Convert.ToInt32(Session["ComEmployeeId"]);//Convert.ToInt32(Session["CompanyId"]);
                    _companyUser.User = "Company";
                }
                //else
                //{
                //    _currentAccountUser = Session["EmployeeId"].ToString();
                //}
                return _companyUser;
            }
        }

        public LoginUser EmpUser
        {
            get
            {
                if (_empser == null)
                {
                    _empser = new LoginUser();
                }

                if (!string.IsNullOrEmpty(Convert.ToString(Session["EmgAdminID"])))
                {
                    _empser.UserId = Convert.ToInt32(Session["EmgAdminID"]);
                    _empser.User = "Admin";
                }
                else if (!string.IsNullOrEmpty(Convert.ToString(Session["ComAdminID"])))
                {
                    //_empser.UserId = Convert.ToInt32(Session["ComAdminID"]);
                    _empser.UserId = Convert.ToInt32(Session["ComEmployeeId"]);
                    _empser.User = "Company";
                }
                else
                {
                    _empser.UserId = Convert.ToInt32(Session["EmployeeId"]);
                    _empser.User = "Employee";
                }
                return _empser;
            }
        }
    }
}