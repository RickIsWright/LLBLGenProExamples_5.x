﻿using System;

using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.Examples.Auditing;
using SD.LLBLGen.Pro.Examples.Auditing.DatabaseSpecific;
using System.Collections.Generic;
using SD.LLBLGen.Pro.Examples.Auditing.EntityClasses;

/// <summary>
/// Control class to edit a 'Product' entity instance
/// </summary>
public partial class Controls_EditProduct : System.Web.UI.UserControl, IEditControl
{
	#region Class Member Declarations
	private FormViewMode _editMode = FormViewMode.Insert;
	private string _pkValuesAfterInsert = string.Empty;
	private bool _inDeleteMode = false;
	#endregion
	
	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		frmEditProduct.DefaultMode = _editMode;

        if (!IsPostBack)
        {
            // prefetch so related entity fields could be populated in gridview
            IPrefetchPath2 path = new PrefetchPath2((int)EntityType.ProductEntity);
            path.Add(ProductEntity.PrefetchPathCategories);
            path.Add(ProductEntity.PrefetchPathSuppliers);

            _ProductDS.PrefetchPathToUse = path;
        }
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
	/// Handles the ItemCommand event of the frmEditProduct control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.FormViewCommandEventArgs"/> instance containing the event data.</param>
	protected void frmEditProduct_ItemCommand(object sender, FormViewCommandEventArgs e)
	{
		if(!Page.IsValid)
		{
			return;
		}
		
		switch(e.CommandName)
		{
			case "InsertAndNew":
				frmEditProduct.InsertItem(true);
				break;
			case "InsertAndHome":
				frmEditProduct.InsertItem(true);
				Response.Redirect("~/default.aspx");
				break;
			case "UpdateAndHome":
				frmEditProduct.UpdateItem(true);
				Response.Redirect("~/default.aspx");
				break;
			case "InsertAndView":
				frmEditProduct.InsertItem(true);
				Response.Redirect("~/ViewExisting.aspx?EntityType=" + (int)EntityType.ProductEntity + _pkValuesAfterInsert);
				break;
			case "EditExisting":
				frmEditProduct.ChangeMode(FormViewMode.Edit);
				break;
			case "DeleteExisting":
				frmEditProduct.DeleteItem();
				Response.Redirect("~/default.aspx");
				break;
		}
	}

	
	/// <summary>
	/// Handles the ItemCreated event of the frmEditProduct control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void frmEditProduct_ItemCreated(object sender, EventArgs e)
	{
		switch(frmEditProduct.CurrentMode)
		{
			case FormViewMode.Insert:
				// set filters for fk fields, if applicable. 
				if(Request.QueryString["20FkField"]=="SupplierId")
				{
					TextBox control = (TextBox)frmEditProduct.FindControl("tbxSupplierId");
					if(control!=null)
					{
						control.Text = Request.QueryString["SupplierId"];
						control.ReadOnly = true;
					}
				}
				if(Request.QueryString["20FkField"]=="CategoryId")
				{
					TextBox control = (TextBox)frmEditProduct.FindControl("tbxCategoryId");
					if(control!=null)
					{
						control.Text = Request.QueryString["CategoryId"];
						control.ReadOnly = true;
					}
				}
				break;
			case FormViewMode.ReadOnly:
				Button btnDelete = (Button)frmEditProduct.FindControl("btnDelete");
				Button btnEdit = (Button)frmEditProduct.FindControl("btnEdit");
				if(btnDelete!=null)
				{
					btnDelete.Visible = _inDeleteMode;
				}
				if(btnEdit!=null)
				{
					btnEdit.Visible = !_inDeleteMode;
				}
				break;
		}
	}
	
	
	/// <summary>
	/// Eventhandler for the PerformWork event on the _ProductDS datasourcecontrol
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void _ProductDS_PerformSelect(object sender, PerformSelectEventArgs2 e)
	{
		using(DataAccessAdapter adapter = new DataAccessAdapter())
		{
			adapter.FetchEntityCollection(e.ContainedCollection, e.Filter, e.MaxNumberOfItemsToReturn, e.Sorter, e.PrefetchPath, e.PageNumber, e.PageSize);
		}
	}

	
	/// <summary>
	/// Eventhandler for the PerformWork event on the _ProductDS datasourcecontrol
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void _ProductDS_PerformWork(object sender, PerformWorkEventArgs2 e)
	{
		// as we're using a formview, there's just 1 entity in the UoW.
		ProductEntity entityToProcess = null;
		List<UnitOfWorkElement2> elementsToInsert = e.Uow.GetEntityElementsToInsert();
		if(elementsToInsert.Count > 0)
		{
			// it's an insert operation. grab the entity so we can determine the PK later on. 
			entityToProcess = (ProductEntity)elementsToInsert[0].Entity;
		}
		using(DataAccessAdapter adapter = new DataAccessAdapter())
		{
			e.Uow.Commit(adapter, true);
		}
		if(entityToProcess != null)
		{
			// store the PK values so a redirect can use these. 
			_pkValuesAfterInsert = "&ProductId=" + entityToProcess.ProductId;

		}
	}
	
	
	#region Class Property Declarations
	/// <summary>
	/// Gets / sets the edit mode of the control
	/// </summary>	
	public FormViewMode EditMode
	{
		get { return _editMode; }
		set { _editMode = value; }
	}
	
	
	/// <summary>
	/// Sets the filter to use for the editcontrol's datasource control.
	/// </summary>
	public PredicateExpression FilterToUse 
	{
		set
		{
			if(value!=null)
			{
				_ProductDS.FilterToUse = new RelationPredicateBucket(value);
			}
		}
	}
	
	/// <summary>
	/// Sets a value indicating whether the delete mode is active. This is effective in FormViewMode.ReadOnly only, and it controls whether the delete button is
	/// active or the edit button. 
	/// </summary>
	public bool DeleteMode 
	{ 
		set
		{
			_inDeleteMode = value;
		}
	}

	#endregion
}
