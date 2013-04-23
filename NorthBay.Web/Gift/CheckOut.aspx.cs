﻿using System;
using System.Data.Linq;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;
using NorthBay.Framework.Database;
using NorthBay.Logic.Country;
using NorthBay.Logic.Gift;
using NorthBay.Logic.User;
using NorthBay.Utility;

namespace NorthBay.Web.Gift
{
    public partial class CheckOut : Basepage
    {
        private readonly UserBillingAddressClass _userBillingAddress = new UserBillingAddressClass();
        private readonly UserBillingAddressViewClass _userBillingAddressView = new UserBillingAddressViewClass();
        private readonly OrderClass _orderClass = new OrderClass();
        private readonly ShoppingCart _objShoppingCart = new ShoppingCart();

        private Cart ShoppingCart { get; set; }

        private int SelectedAddressId
        {
            get
            {
                if (Session["SelectedAddressId"] == null)
                    return 0;

                return (int)Session["SelectedAddressId"];
            }

            set { Session["SelectedAddressId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Load Shopping Cart
            ShoppingCart = LoadShoppingCartObject();

            //return if shopping cart is empty
            if (ShoppingCart == null || ShoppingCart.Count <= 0)
                return;

            //Check if user is logged in
            if (!IsAuthenticated())
                Response.Redirect("~/Login.aspx?ReturnUrl=" + HttpContext.Current.Request.Url.AbsoluteUri);

            if (Page.IsPostBack)
                return;

            //Set Country Dropdownlist
            SetCountry();

            //Bind address book
            DlAddress_DataBind();

            //Set Credit Card Expiry Month and Year
            SetCreditCardExpiry();

            wiz_cart.ActiveStepIndex = 0;
        }

        private void SetCreditCardExpiry()
        {
            //Set month
            for (var i = 1; i <= 12; i++)
                ddl_month.Items.Add(new ListItem(string.Format("{0:00}", i), i.ToString()));

            //Set Year
            for (var x = DateTime.Now.Year; x < DateTime.Now.Year + 20; x++)
                ddl_year.Items.Add(new ListItem(x.ToString(), x.ToString()));
        }

        private void GvCart_DataBind()
        {
            gv_cart.DataSource = ShoppingCart;
            gv_cart.DataBind();
        }

        private Cart LoadShoppingCartObject()
        {
            if (_objShoppingCart.Cart == null)
            {
                if (Session["Cart"] == null)
                    return null;

                //Add to object
                _objShoppingCart.Cart = (Cart)Session["Cart"];
            }

            return _objShoppingCart.Cart;
        }

        private void SetCountry()
        {
            ddl_country.DataSource = new CountryClass().SelectAll();
            ddl_country.DataTextField = "PrintableName";
            ddl_country.DataValueField = "CountryId";
            ddl_country.DataBind();
        }

        private void DlAddress_DataBind()
        {
            //Get userId from Session
            var list = _userBillingAddressView.SelectByUserId(5);

            //if no data hide datalist
            if (list.Count == 0)
                dl_address.Visible = false;

            dl_address.DataSource = list;
            dl_address.DataBind();
        }

        private EntitySet<OrderItem> GetOrderItems()
        {
            var items = new EntitySet<OrderItem>();

            foreach (var orderItem in ShoppingCart.Select(cartProduct => new OrderItem
                                                                             {
                                                                                 ProductId = cartProduct.ProductId,
                                                                                 Price = cartProduct.Price,
                                                                                 Quantity = cartProduct.Quantity
                                                                             }))
            {
                items.Add(orderItem);
            }

            return items;
        }

        protected void GvCart_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                var tableCells = e.Row.Cells;

                //Merge rows
                tableCells[0].ColumnSpan = 2;
                tableCells[2].ColumnSpan = 2;
                for (int x = 1; x <= tableCells.Count; x++)
                {
                    var cell = tableCells[x];

                    //Remove cell
                    tableCells.Remove(cell);
                }

                var lblSubTotal = e.Row.FindControl("lbl_subtotal") as Label;

                //Set sub total price
                if (lblSubTotal != null)
                    lblSubTotal.Text = _objShoppingCart.GetStringTotalPrice();

                var lblTax = e.Row.FindControl("lbl_tax") as Label;

                //Set sub total price
                if (lblTax != null)
                    lblTax.Text = _objShoppingCart.GetStringTax();

                var lblTotal = e.Row.FindControl("lbl_total") as Label;

                //Set sub total price
                if (lblTotal != null)
                    lblTotal.Text = _objShoppingCart.GetStringTotalPriceWithTax();
            }
        }

        protected void WizCart_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {

        }

        protected void WizCart_ActiveStepChanged(object sender, EventArgs e)
        {
            //Set header template
            var dataList = wiz_cart.FindControl("HeaderContainer").FindControl("dl_header") as DataList;

            if (dataList == null)

                return;

            dataList.DataSource = wiz_cart.WizardSteps;
            dataList.DataBind();
        }

        protected void WizCart_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            DlAddress_DataBind();
        }

        protected void Button_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button == null)
                return;

            var commandName = button.CommandName;

            if (string.IsNullOrEmpty(commandName))
                return;

            int? id;
            switch (commandName.ToLower())
            {
                case "ship":
                    {
                        id = TextHelper.ToInteger(button.CommandArgument);

                        if (id == null)
                            return;

                        //Set Selected Address Id
                        SelectedAddressId = (int)id;

                        //Go to next index
                        wiz_cart.MoveTo(wiz_cart.WizardSteps[1]);

                        //Bind Cart
                        GvCart_DataBind();
                    }
                    break;

                case "nextship":
                    {
                        var userBillingAddress = new UserBillingAddress
                        {
                            FullName = txt_fullname.Text,
                            AddressLine1 = txt_address1.Text,
                            AddressLine2 = txt_address2.Text,
                            City = txt_city.Text,
                            State = txt_state.Text,
                            CountryId = TextHelper.ToInteger(ddl_country.SelectedValue),
                            PostalCode = txt_postalcode.Text,
                            PhoneNumber = txt_phone.Text,
                            UserId = LoggedInUserId,
                            IsActive = true//Get from session
                        };

                        int returnId;

                        if (!_userBillingAddress.Insert(userBillingAddress, out returnId))
                            return;

                        SelectedAddressId = returnId; //Set selectedAddressId so that system won't re-add the data again

                        //Bind Cart
                        GvCart_DataBind();

                        //Clear all fields
                        txt_fullname.Text = string.Empty;
                        txt_address1.Text = string.Empty;
                        txt_address2.Text = string.Empty;
                        txt_city.Text = string.Empty;
                        txt_state.Text = string.Empty;
                        txt_phone.Text = string.Empty;
                        ddl_country.SelectedIndex = 0;

                        //Move to next step
                        MoveToNextStep(1);
                    }
                    break;

                case "nextpayment":
                    {
                        MoveToNextStep(2);
                    }
                    break;
                case "makepayment":
                    {
                        //Check if shopping cart is empty
                        if (ShoppingCart == null || ShoppingCart.Count == 0)
                            return;

                        //Check if address is selected
                        if (SelectedAddressId == 0)
                            return;

                        //Use transaction to make sure all queries run without error
                        using (var ts = new TransactionScope())
                        {
                            var order = new Order
                            {
                                CardNumber = txt_cardNumber.Text,
                                SecurityNumber = TextHelper.ToInteger(txt_securityNumber.Text),
                                NameOnCard = txt_nameOnCard.Text,
                                ExpirationMonth = TextHelper.ToInteger(ddl_month.SelectedValue),
                                ExpirationYear = TextHelper.ToInteger(ddl_year.SelectedValue),
                                UserBillingAddressId = SelectedAddressId,
                                UserId = LoggedInUserId, //get from session
                                OrderItems = GetOrderItems()
                            };

                            int orderNumber;
                            if (_orderClass.Insert(order, out orderNumber))
                            {
                                Session["Order"] = orderNumber;

                                ts.Complete();
                            }
                        }

                        ShoppingCart = null;

                        MoveToNextStep(3);
                    }
                    break;

                case "remove":
                    id = TextHelper.ToInteger(button.CommandArgument);

                    if (id == null)
                        return;

                    if (_userBillingAddress.Delete((int)id))
                    {
                        DlAddress_DataBind();
                    }
                    break;
            }
        }

        private void MoveToNextStep(int index)
        {
            wiz_cart.MoveTo(wiz_cart.WizardSteps[index]);
        }
    }
}