using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebRole1
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Session["checkBox"] != null)
                {

                    HotelCheck.Checked = (bool)Session["checkBox"];

                }
            }
            }

        protected void BtnPost_Click(object sender, EventArgs e)
        {

            
            Session["checkBox"] = HotelCheck.Checked;
            
         
            Response.Redirect("Flight.aspx");

        }
    }
}