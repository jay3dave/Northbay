﻿using System;
using NorthBay.Logic.Volunteer;
using NorthBay.Utility;

namespace NorthBay.Web.Admin.Volunteer
{
    public partial class Add : Basepage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Load javascript just for this page
            JScript.AddScript("");

            //Load css just for this page
            Css.AddCss("");

            //Check if page postback
            if (Page.IsPostBack)
                return;

            SetVolunteerCateogryData();
        }


        private void SetVolunteerCateogryData()
        {
            var objVolunteerCategory = new VolunteerCategoryClass();

            ddl_category.DataSource = objVolunteerCategory.SelectAll();
            ddl_category.DataTextField = "Name";
            ddl_category.DataValueField = "VolunteerCategoryId";
            ddl_category.DataBind();

        }

        /// <summary>
        /// Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ButtonClick(object sender, EventArgs e)
        {

            //create volunteering model and assign values to it
            var volunteer = new Framework.Database.Volunteer
            {
                Title = txt_title.Text,
                VolunteerCategoryId = TextHelper.ToInteger(ddl_category.SelectedValue) ?? 1,
                Description = txt_description.Text,
                PostDate = TextHelper.ToDateTime(txt_postdate.Text) ?? DateTime.Now,
                EndDate = TextHelper.ToDateTime(txt_enddate.Text),
                Active = TextHelper.ToBool(ddl_active.SelectedValue) ?? false
            };

            //Create new Volunteering Object
            var objVolunteer = new VolunteerClass();

            if (!objVolunteer.Insert(volunteer))
            {
                //show error message
            }
            else
            {
                Redirect("~/Admin/Volunteer/");
            }
        }
    }
}