using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using System.Web.UI.HtmlControls;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;
using ChemSW.Core;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    public class CswSequencesEditor : CompositeControl, INamingContainer
    {
        private CswNbtResources _CswNbtResources;
        private RadAjaxManager _AjaxManager;
        private DropDownList _SequenceList;
        private CswNbtSequenceManager _CswNbtSequenceManager;

        #region Fields

        public Int32 SelectedSequenceId
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( _SequenceList.SelectedItem != null )
                    ret = CswConvert.ToInt32( _SequenceList.SelectedItem.Value );
                return ret;
            }
        }


        private Int32 _NodeTypePropId = Int32.MinValue;
        public Int32 NodeTypePropId
        {
            get
            {
                return _NodeTypePropId;
            }
        }

        #endregion Fields

        #region Lifecycle

        public CswSequencesEditor( CswNbtResources Objs, RadAjaxManager AjaxManager, Int32 ThisNodeTypePropId )
        {
            _NodeTypePropId = ThisNodeTypePropId;
            _CswNbtResources = Objs;
            _AjaxManager = AjaxManager;
            _CswNbtSequenceManager = new CswNbtSequenceManager( Objs );

            this.DataBinding += new EventHandler( CswSequences_DataBinding );
        }

        private void CswSequences_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();

            DataTable CurrentSequences = _CswNbtSequenceManager.CurrentSequences;
            DataRow NewRow = CurrentSequences.NewRow();
            NewRow["sequencename"] = "[new]";
            NewRow["sequenceid"] = CswConvert.ToDbVal( Int32.MinValue );
            CurrentSequences.Rows.InsertAt( NewRow, 0 );

            _SequenceList.DataSource = CurrentSequences;
            _SequenceList.DataTextField = "sequencename";
            _SequenceList.DataValueField = "sequenceid";
        }

        private void CswSequences_SelectListBound( object sender, EventArgs e )
        {
            if( _NodeTypePropId > 0 )
            {
                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( _NodeTypePropId );
                if( NodeTypeProp.SequenceId != Int32.MinValue )
                {
                    if( null != _SequenceList.Items.FindByValue( NodeTypeProp.SequenceId.ToString() ) )
                        _SequenceList.SelectedValue = NodeTypeProp.SequenceId.ToString();

                    setSequenceFields();
                }
            }
        }

        private TextBox _tbxSequenceToAdd;
        private TextBox _tbxSeqPrepend;
        private TextBox _tbxSeqPostpend;
        private TextBox _tbxSeqPad;
        private Label _lblSequenceNextValue;
        private RequiredFieldValidator _NameRequiredValidator;
        private Button _btnAddSequence;
        private CswHiddenTable _ControlTable;
        private Label _lblActionLabel;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _SequenceList = new DropDownList();
            _SequenceList.DataBound += CswSequences_SelectListBound;
            _SequenceList.SelectedIndexChanged += SequenceSelect_Change;
            _SequenceList.AutoPostBack = true;
            _SequenceList.EnableViewState = false;
            _SequenceList.ID = "seqlist_" + NodeTypePropId;
            _SequenceList.CssClass = "selectinput";
            Table.addControl( 0, 0, _SequenceList );

            _tbxSequenceToAdd = new TextBox();
            _tbxSequenceToAdd.ID = "tbxSequenceToAdd_" + NodeTypePropId;
            _tbxSequenceToAdd.CssClass = "textinput";

            _NameRequiredValidator = new RequiredFieldValidator();
            _NameRequiredValidator.ID = "RequiredValidator_" + NodeTypePropId;
            _NameRequiredValidator.Display = ValidatorDisplay.Dynamic;
            _NameRequiredValidator.EnableClientScript = true;
            _NameRequiredValidator.Text = "&nbsp;<img src=\"Images/vld/bad.gif\" alt=\"Value is required\" />";
            _NameRequiredValidator.ValidationGroup = "SequenceEditor";
            _NameRequiredValidator.ControlToValidate = _tbxSequenceToAdd.ID;

            _btnAddSequence = new Button();
            _btnAddSequence.ID = "btnAddSequence";
            _btnAddSequence.Text = "Add";
            //_btnAddSequence.Click += SequencesAddButton_Click;
            _btnAddSequence.Click += new EventHandler( _btnAddSequence_Click );
            _btnAddSequence.CssClass = "Button";
            _btnAddSequence.ValidationGroup = "SequenceEditor";

            _tbxSeqPrepend = new TextBox();
            _tbxSeqPrepend.ID = "tbxSeqPrepend";
            _tbxSeqPrepend.CssClass = "textinput";

            _tbxSeqPostpend = new TextBox();
            _tbxSeqPostpend.ID = "tbxSeqPostpend";
            _tbxSeqPostpend.CssClass = "textinput";

            _tbxSeqPad = new TextBox();
            _tbxSeqPad.ID = "tbxSeqPad";
            _tbxSeqPad.CssClass = "textinput";

            _lblSequenceNextValue = new Label();
            _lblSequenceNextValue.ID = "lblSequenceNextValue";

            _ControlTable = new CswHiddenTable();
            _ControlTable.ID = "AddSequenceTable";
            _ControlTable.Table.CssClass = "OuterTable";

            _lblActionLabel = new Label();

            _ControlTable.Table.addControl( 0, 0, _lblActionLabel );
            _ControlTable.Table.addControl( 1, 1, new CswLiteralText( "Name:" ) );
            _ControlTable.Table.addControl( 1, 2, _tbxSequenceToAdd );
            _ControlTable.Table.addControl( 1, 2, _NameRequiredValidator );
            _ControlTable.Table.addControl( 2, 1, new CswLiteralText( "Pre:" ) );
            _ControlTable.Table.addControl( 2, 2, _tbxSeqPrepend );
            _ControlTable.Table.addControl( 3, 1, new CswLiteralText( "Post:" ) );
            _ControlTable.Table.addControl( 3, 2, _tbxSeqPostpend );
            _ControlTable.Table.addControl( 4, 1, new CswLiteralText( "Pad:" ) );
            _ControlTable.Table.addControl( 4, 2, _tbxSeqPad );
            _ControlTable.Table.addControl( 4, 3, _btnAddSequence );
            _ControlTable.Table.addControl( 5, 1, new CswLiteralText( "Next Value:&nbsp;" ) );
            _ControlTable.Table.addControl( 5, 2, _lblSequenceNextValue );

            Table.addControl( 0, 1, _ControlTable );

            // fixes the border
            _ControlTable.Table.FillTable = true;

            base.CreateChildControls();

        }//CreateChildControls

        protected override void OnPreRender( EventArgs e )
        {
            // BZ 8516
            setSequenceFields();

            base.OnPreRender( e );
        }

        #endregion Lifecycle

        #region Events

        protected void _btnAddSequence_Click( object sender, EventArgs e )
        {
            Int32 Pad = 0;
            if( CswTools.IsInteger( _tbxSeqPad.Text ) )
                Pad = CswConvert.ToInt32( _tbxSeqPad.Text );

            if( SelectedSequenceId == Int32.MinValue )
            {
                // Add
                Int32 SequenceId = _CswNbtSequenceManager.makeSequence( new CswSequenceName(_tbxSequenceToAdd.Text), _tbxSeqPrepend.Text, _tbxSeqPostpend.Text, Pad, 1 );
                DataBind();
                _SequenceList.SelectedValue = SequenceId.ToString();
            }
            else
            {
                // Edit
                _CswNbtSequenceManager.editSequence( SelectedSequenceId, new CswSequenceName(_tbxSequenceToAdd.Text), _tbxSeqPrepend.Text, _tbxSeqPostpend.Text, Pad );
                Int32 OldSelectedId = SelectedSequenceId;
                DataBind();
                _SequenceList.SelectedValue = OldSelectedId.ToString();
            }
        }

        protected void SequenceSelect_Change( object sender, EventArgs e )
        {
            // BZ 8516 - we do this in prerender now
            // setSequenceFields( CswConvert.ToInt32( _SequenceList.SelectedItem.Value ) );
        }

        #endregion Events

        #region Helpers

        public string formatNextValueSequence( Int32 SequenceId, CswSequenceName SequenceName )
        {
            CswNbtSequenceValue CswNbtSequenceValue = new CswNbtSequenceValue( _CswNbtResources , SequenceId);
            return ( CswNbtSequenceValue.makeExample( _CswNbtSequenceManager.getSequenceValue( SequenceName ) ) );
        }

        private void setSequenceFields()
        {
            _tbxSequenceToAdd.Text = "";
            _tbxSeqPrepend.Text = "";
            _tbxSeqPostpend.Text = "";
            _tbxSeqPad.Text = "";
            _lblActionLabel.Text = "Add New Sequence";
            _btnAddSequence.Text = "Add";
            _lblSequenceNextValue.Text = "1";

            if( SelectedSequenceId != Int32.MinValue )
            {
                _lblActionLabel.Text = "Edit Sequence";
                _btnAddSequence.Text = "Edit";

                DataRow CurrentSequenceRow = _CswNbtSequenceManager.getSequence( SelectedSequenceId ).Rows[0];
                CswSequenceName SequenceName = new CswSequenceName(string.Empty);
                if( !CurrentSequenceRow.IsNull( "sequencename" ) && CurrentSequenceRow["sequencename"].ToString().Trim().Length > 0 )
                {
                    SequenceName = new CswSequenceName( CurrentSequenceRow["sequencename"].ToString() );
                    _tbxSequenceToAdd.Text = CurrentSequenceRow["sequencename"].ToString();
                }

                if( !CurrentSequenceRow.IsNull( "prep" ) && CurrentSequenceRow["prep"].ToString().Trim().Length > 0 )
                    _tbxSeqPrepend.Text = CurrentSequenceRow["prep"].ToString();

                if( !CurrentSequenceRow.IsNull( "post" ) && CurrentSequenceRow["post"].ToString().Trim().Length > 0 )
                    _tbxSeqPostpend.Text = CurrentSequenceRow["post"].ToString();

                if( !CurrentSequenceRow.IsNull( "pad" ) && CurrentSequenceRow["pad"].ToString().Trim().Length > 0 )
                    _tbxSeqPad.Text = CurrentSequenceRow["pad"].ToString();

                _lblSequenceNextValue.Text = formatNextValueSequence( SelectedSequenceId, SequenceName );

            } // if( SelectedSequenceId != Int32.MinValue )
        } // setSequenceFields()

        public void assignSequence( CswSequenceName SequenceName, Int32 NodeTypePropId )
        {
            _CswNbtSequenceManager.assignSequence( SequenceName, NodeTypePropId );
        }

        public void unAssignSequence( Int32 NodeTypePropId )
        {
            _CswNbtSequenceManager.unAssignSequence( NodeTypePropId );
        }

        public void removeSequence( CswSequenceName SequenceName )
        {
            _CswNbtSequenceManager.removeSequence( SequenceName );
        }

        #endregion Helpers
    }
}
