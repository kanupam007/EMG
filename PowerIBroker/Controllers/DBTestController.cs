using System;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;

namespace PowerIBroker.Controllers
{
    public class DBTestController : Controller
    {
        DataTable dtFillGrid = new DataTable();
        //
        // GET: /Test/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection Collection)
        {
            //SendEmailtest();

            if (Collection["txtUserName"].ToString() == "admin" && Collection["txtPassword"].ToString() == "poweribroker!")
            {
                Session["DBTest"] = Collection["txtUserName"].ToString();
                return RedirectToAction("Query", "DBTest");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Query()
        {
            if(Session["DBTest"]==null)
                return RedirectToAction("Index", "DBTest");
            return View(dtFillGrid);
        }
        [HttpPost]
        public ActionResult Query(FormCollection Collection, string Command)
        {
            if (Session["DBTest"] == null)
                return RedirectToAction("Index", "DBTest");
            try
            {
                TempData["Query"] = Collection["txtQuery"].ToString();
                //SqlConnection Connection = new SqlConnection("data source=100.100.7.7;initial catalog=WCS;persist security info=True;user id=sa;password=Flexsinsa123;");
                SqlConnection Connection = new SqlConnection(@"Data Source=64.27.25.25;initial catalog=PowerIbroker;persist security info=True;User Id=flexsin; Password=WeBhyd{_KJH1@fle");
                if (Command == "FillGrid")
                {
                    Connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(Collection["txtQuery"].ToString(), Connection);
                    DataSet dsFillGrid = new DataSet();
                    da.Fill(dsFillGrid);
                    dtFillGrid = dsFillGrid.Tables[0];
                }
                if (Command == "Execute")
                {
                    Connection.Open();
                    SqlCommand cmd = new SqlCommand(Collection["txtQuery"].ToString(), Connection);
                    cmd.ExecuteNonQuery();
                    Connection.Close();

                }
                return View(dtFillGrid);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
                return View(dtFillGrid);
            }
        }

        public ActionResult Pdf()
        {
            return View();
        }
        
    }
}
