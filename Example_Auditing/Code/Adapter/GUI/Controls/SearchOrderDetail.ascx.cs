﻿using System;

using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.Examples.Auditing.HelperClasses;

/// <summary>
/// Control class to search one or more 'OrderDetail' entity instances
/// </summary>
public partial class Controls_SearchOrderDetail : System.Web.UI.UserControl, ISearchControl
{
	#region Events
	/// <summary>
	/// Event which is raised when the user clicked a search button. After this event, the 'Filter' property is valid.
	/// </summary>
	public event EventHandler SearchClicked;
	#endregion
	
	#region Class Member Declarations
	private PredicateExpression _filter;
	#endregion
		
	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}
	
	/// <summary>
	/// Creates a predicate expression filter based on the query string passed in. 
	/// </summary>
	/// <param name="queryString">The query string with PK field names and values.</param>
	/// <returns>a predicate expression with a filter on the pk fields and values.</returns>
	public PredicateExpression CreateFilter(NameValueCollection queryString)
	{
		PredicateExpression toReturn = new PredicateExpression();
		string valueFromQueryString = null;
		valueFromQueryString = queryString["OrderId"];
		if(valueFromQueryString!=null)
		{
			toReturn.AddWithAnd(OrderDetailFields.OrderId==(System.Int32)Convert.ChangeType(valueFromQueryString, typeof(System.Int32)));
		}
		valueFromQueryString = queryString["ProductId"];
		if(valueFromQueryString!=null)
		{
			toReturn.AddWithAnd(OrderDetailFields.ProductId==(System.Int32)Convert.ChangeType(valueFromQueryString, typeof(System.Int32)));
		}

		return toReturn;
	}
	

	/// <summary>
	/// Handles the Click event of the btnCancel control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void btnCancel_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/default.aspx");
	}


	/// <summary>
	/// Handles the Click event of the btnSearchPK control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void btnSearchPk_Click(object sender, EventArgs e)
	{
		if(!Page.IsValid)
		{
			return;
		}
		_filter = new PredicateExpression();
		_filter.AddWithAnd(OrderDetailFields.OrderId==(System.Int32)Convert.ChangeType(tbxOrderId.Text, typeof(System.Int32)));
		_filter.AddWithAnd(OrderDetailFields.ProductId==(System.Int32)Convert.ChangeType(tbxProductId.Text, typeof(System.Int32)));
		if((SearchClicked!=null) && (_filter.Count>0))
		{
			SearchClicked(this, new EventArgs());
		}
	}



	/// <summary>
	/// Handles the Click event of the btnSearchSubSet control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void btnSearchSubSet_Click(object sender, EventArgs e)
	{
		if(!Page.IsValid)
		{
			return;
		}

		_filter = new PredicateExpression();
		IPredicate toAdd = null;
		if(chkEnableOrderId.Checked)
		{
			string fromValueAsString = tbxOrderIdMiFrom.Text;
			string toValueAsString = tbxOrderIdMiTo.Text;
			toAdd = GeneralUtils.CreatePredicate(OrderDetailFields.OrderId, Convert.ToInt32(opOrderId.SelectedValue), chkNotOrderId.Checked, 
						fromValueAsString, toValueAsString);
			if(toAdd != null)
			{
				_filter.Add(toAdd);
			}
		}
		if(chkEnableProductId.Checked)
		{
			string fromValueAsString = tbxProductIdMiFrom.Text;
			string toValueAsString = tbxProductIdMiTo.Text;
			toAdd = GeneralUtils.CreatePredicate(OrderDetailFields.ProductId, Convert.ToInt32(opProductId.SelectedValue), chkNotProductId.Checked, 
						fromValueAsString, toValueAsString);
			if(toAdd != null)
			{
				_filter.Add(toAdd);
			}
		}
		if(chkEnableUnitPrice.Checked)
		{
			string fromValueAsString = tbxUnitPriceMiFrom.Text;
			string toValueAsString = tbxUnitPriceMiTo.Text;
			toAdd = GeneralUtils.CreatePredicate(OrderDetailFields.UnitPrice, Convert.ToInt32(opUnitPrice.SelectedValue), chkNotUnitPrice.Checked, 
						fromValueAsString, toValueAsString);
			if(toAdd != null)
			{
				_filter.Add(toAdd);
			}
		}
		if(chkEnableQuantity.Checked)
		{
			string fromValueAsString = tbxQuantityMiFrom.Text;
			string toValueAsString = tbxQuantityMiTo.Text;
			toAdd = GeneralUtils.CreatePredicate(OrderDetailFields.Quantity, Convert.ToInt32(opQuantity.SelectedValue), chkNotQuantity.Checked, 
						fromValueAsString, toValueAsString);
			if(toAdd != null)
			{
				_filter.Add(toAdd);
			}
		}
		if(chkEnableDiscount.Checked)
		{
			string fromValueAsString = tbxDiscountMiFrom.Text;
			string toValueAsString = tbxDiscountMiTo.Text;
			toAdd = GeneralUtils.CreatePredicate(OrderDetailFields.Discount, Convert.ToInt32(opDiscount.SelectedValue), chkNotDiscount.Checked, 
						fromValueAsString, toValueAsString);
			if(toAdd != null)
			{
				_filter.Add(toAdd);
			}
		}
		if(SearchClicked != null)
		{
			SearchClicked(this, new EventArgs());
		}
	}
	
	#region Class Property Declarations
	/// <summary>
	/// Gets the filter formulated with the values specified in the search control. Valid after SearchClicked has been raised. 
	/// </summary>
	public PredicateExpression Filter 
	{ 
		get { return _filter;}
	}
	
	/// <summary>
	/// Sets the flag for allowing single entity searches. If set to true, the search control will show PK fields and if applicable, UC fields.
	/// </summary>
	public bool AllowSingleEntitySearches
	{ 
		set
		{
			phSingleInstance.Visible=value;
		}
	}
	
	/// <summary>
	/// Sets the flag for allowing multi entity searches. If set to true, the search control will show fields to formulate a search over all fields, and 
	/// thus a filter which could lead to multiple results.
	/// </summary>
	public bool AllowMultiEntitySearches 
	{ 
		set
		{
			phMultiInstance.Visible=value;
		}
	}
	#endregion
}
