﻿using System;
using System.Web.UI.WebControls;
using NorthBay.Logic.Volunteer;
using NorthBay.Utility;

namespace NorthBay.Web.Admin.Volunteer
{
    public partial class Default : Basepage
    {
        private readonly VolunteerViewClass _objVolunteerView = new VolunteerViewClass();
        private readonly VolunteerClass _objVolunteer = new VolunteerClass();

        /// <summary>
        /// Using viewstate to store the sort direction
        /// </summary>
        private SortDirection SortDirection
        {
            get
            {
                if (ViewState["SortDirection"] == null)
                    ViewState["SortDirection"] = SortDirection.Ascending;

                return (SortDirection)ViewState["SortDirection"];
            }
            set { ViewState["SortDirection"] = value; }
        }

        /// <summary>
        /// Using Viewstate to store the sort expression
        /// </summary>
        private string SortExpression
        {
            get
            {
                return ViewState["SortExpression"] as string ?? "VolunteerCategoryName";
            }
            set { ViewState["SortExpression"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            //Load javascript just for this page
            JScript.AddScript("global.js");

            //If page is postback, stop executing
            if (Page.IsPostBack)
                return;

            GridView_DataBind();
        }

        private void GridView_DataBind()
        {
            gridView.DataSource = _objVolunteerView.SortAll(SortExpression, SortDirection);
            gridView.DataBind();
        }

        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (string.IsNullOrEmpty(SortExpression))
                SortExpression = e.SortExpression;
            else
            {
                //If sorting the same column
                if (SortExpression == e.SortExpression)
                {
                    //Set Sort Direction
                    SortDirection = SortDirection == SortDirection.Ascending
                                        ? SortDirection.Descending
                                        : SortDirection.Ascending;
                }
                else
                {
                    SortExpression = e.SortExpression;
                    SortDirection = e.SortDirection;
                }
            }

            //Calling Sort All Function
            GridView_DataBind();
        }


        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridView.PageIndex = e.NewPageIndex;

            GridView_DataBind();
        }

        protected void Button_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gridView.Rows)
            {
                var objControl = row.FindControl("cb_select");

                if (!(objControl is CheckBox))
                    continue;

                var checkbox = objControl as CheckBox;
                // If checkbox is not check, move on
                if (!checkbox.Checked)
                    continue;

                //look for hidden field
                objControl = row.FindControl("hf_id");

                if (!(objControl is HiddenField))
                    continue;

                var hiddenField = objControl as HiddenField;

                //Get id
                var id = TextHelper.ToInteger(hiddenField.Value) ?? -1;

                if (id == -1) continue;

                //Delete data by Id
                _objVolunteer.Delete(id);
            }

            //Rebind
            GridView_DataBind();
        }
    }
}