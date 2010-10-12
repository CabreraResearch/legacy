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
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    public class CswSequencesEditor : CompositeControl, INamingContainer
    {
        private CswNbtResources _CswNbtResources = null;
        private RadAjaxManager _AjaxManager;
        private DropDownList _SequenceList = null;
        private CswNbtSequenceManager _CswNbtSequenceManager = null;

        #region Fields

        public Int32 SelectedSequenceId
        {
            get
            {
                if (null == _SequenceList.SelectedItem)
                    throw (new CswDniException("A sequence must be selected"));

                return (Convert.ToInt32(_SequenceList.SelectedItem.Value));
            }
        }


        private Int32 _NodeTypePropId = Int32.MinValue;
        public Int32 NodeTypePropId
        {
            get
            {
                return _NodeTypePropId;
            }

            set
            {
                _NodeTypePropId = value;
            }
        }

        #endregion Fields
        
        #region Lifecycle

        public CswSequencesEditor(CswNbtResources Objs, RadAjaxManager AjaxManager, Int32 ThisNodeTypePropId)
        {
            NodeTypePropId = ThisNodeTypePropId;
            
            _CswNbtResources = Objs;
            _AjaxManager = AjaxManager;

            this.DataBinding += new EventHandler(CswSequences_DataBinding);

            _SequenceList = new DropDownList();
            _SequenceList.DataBound += CswSequences_SelectListBound;

            _CswNbtSequenceManager = new CswNbtSequenceManager(Objs);
        }

        private void CswSequences_DataBinding(object sender, EventArgs e)
        {
            DataTable CurrentSequences = getSequences();
            _SequenceList.DataSource = CurrentSequences;
            _SequenceList.DataTextField = "sequencename";
            _SequenceList.DataValueField = "sequenceid";
        }

        private void CswSequences_SelectListBound(object sender, EventArgs e)
        {
            if (_NodeTypePropId > 0)
            {
                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp(_NodeTypePropId);
                if(NodeTypeProp.SequenceId != Int32.MinValue)
                {
                    if (null != _SequenceList.Items.FindByValue(NodeTypeProp.SequenceId.ToString()))
                        _SequenceList.SelectedValue = NodeTypeProp.SequenceId.ToString();
    
                    setSequenceFields(Convert.ToInt32(_SequenceList.SelectedItem.Value));
                }
            }
        }

        private TextBox _tbxSequenceToAdd = new TextBox();
        private TextBox _tbxSeqPrepend = new TextBox();
        private TextBox _tbxSeqPostpend = new TextBox();
        private TextBox _tbxSeqPad = new TextBox();
        private Label _lblSequenceExample = new Label();
        private RequiredFieldValidator _NameRequiredValidator;

        protected override void CreateChildControls()
        {
            // Initialize Controls:
            _SequenceList.SelectedIndexChanged += SequenceSelect_Change;
            _SequenceList.AutoPostBack = true;
            _SequenceList.EnableViewState = false;
            _SequenceList.ID = "seqlist_" + NodeTypePropId;
            this.Controls.Add( _SequenceList );

            _tbxSequenceToAdd.ID = "tbxSequenceToAdd";

            _NameRequiredValidator = new RequiredFieldValidator();
            _NameRequiredValidator.ID = "RequiredValidator_" + NodeTypePropId;
            _NameRequiredValidator.Display = ValidatorDisplay.Dynamic;
            _NameRequiredValidator.EnableClientScript = true;
            _NameRequiredValidator.Text = "&nbsp;<img src=\"Images/vld/bad.gif\" alt=\"Value is required\" />";
            _NameRequiredValidator.ControlToValidate = _tbxSequenceToAdd.ID;
            _NameRequiredValidator.ValidationGroup = "SequenceEditor";

            _SequenceList.CssClass = "selectinput";
            _tbxSequenceToAdd.CssClass = "textinput";
            _tbxSeqPrepend.CssClass = "textinput";
            _tbxSeqPostpend.CssClass = "textinput";
            _tbxSeqPad.CssClass = "textinput";

            Label lblAddSequence = new Label();
            lblAddSequence.ID = "lblAddSequence";
            lblAddSequence.Text = "Add New Sequence";

            Label lblSequenceName = new Label();
            lblSequenceName.ID = "lblSequenceName";
            lblSequenceName.Text = "Name:";

            Button btnAddSequence = new Button();
            btnAddSequence.ID = "btnAddSequence";
            btnAddSequence.Text = "Add";
            btnAddSequence.Click += SequencesAddButton_Click;
            btnAddSequence.CssClass = "Button";
            btnAddSequence.ValidationGroup = "SequenceEditor";

            Label lblSeqPrepend = new Label();
            lblSeqPrepend.Text = "Pre:";

            _tbxSeqPrepend.ID = "tbxSeqPrepend";

            Label lblSeqPostpend = new Label();
            lblSeqPostpend.Text = "Post:";

            _tbxSeqPostpend.ID = "tbxSeqPostpend";

            Label lblSeqPad = new Label();
            lblSeqPad.Text = "Pad:";

            _tbxSeqPad.ID = "tbxSeqPad";

            //Table ControlTable = new Table();
            CswHiddenTable ControlTable = new CswHiddenTable();
            // ControlTable.CssClass = "OuterTable";
            ControlTable.ID = "AddSequenceTable";
            ControlTable.Table.CssClass = "OuterTable";

            Label _lblSequenceExampleLabel = new Label();
            _lblSequenceExampleLabel.Text = "Example:&nbsp;";

            _lblSequenceExample.ID = "lblSequenceExample";

            // Position Controls
            ControlTable.Table.addControl( 0, 0, lblAddSequence );
            ControlTable.Table.addControl( 1, 1, lblSequenceName );
            ControlTable.Table.addControl( 1, 2, _tbxSequenceToAdd );
            ControlTable.Table.addControl( 1, 2, _NameRequiredValidator );
            ControlTable.Table.addControl( 2, 1, lblSeqPrepend );
            ControlTable.Table.addControl( 2, 2, _tbxSeqPrepend );
            ControlTable.Table.addControl( 3, 1, lblSeqPostpend );
            ControlTable.Table.addControl( 3, 2, _tbxSeqPostpend );
            ControlTable.Table.addControl( 4, 1, lblSeqPad );
            ControlTable.Table.addControl( 4, 2, _tbxSeqPad );
            ControlTable.Table.addControl( 4, 3, btnAddSequence );
            ControlTable.Table.addControl( 5, 1, _lblSequenceExampleLabel );
            ControlTable.Table.addControl( 5, 2, _lblSequenceExample );

            this.Controls.Add( ControlTable );

            base.CreateChildControls();

        }//CreateChildControls

        protected override void OnLoad( EventArgs e )
        {
            //_AjaxManager.AjaxSettings.AddAjaxSetting(_SequenceList, _tbxSeqPad);
            //_AjaxManager.AjaxSettings.AddAjaxSetting( _SequenceList, _tbxSeqPrepend );
            //_AjaxManager.AjaxSettings.AddAjaxSetting( _SequenceList, _tbxSeqPostpend );
            //_AjaxManager.AjaxSettings.AddAjaxSetting( _SequenceList, _tbxSequenceToAdd );
            //_AjaxManager.AjaxSettings.AddAjaxSetting( _SequenceList, _lblSequenceExample );
            base.OnLoad( e );
        }

        protected override void OnPreRender(EventArgs e)
        {
            // BZ 8516
            setSequenceFields( Convert.ToInt32( _SequenceList.SelectedItem.Value ) );   

            base.OnPreRender( e );
        }

        #endregion Lifecycle
       
        #region Events

        protected void SequencesAddButton_Click(object sender, EventArgs e)
        {
            Int32 SequenceId = makeSequence(_tbxSequenceToAdd.Text, _tbxSeqPrepend.Text, _tbxSeqPostpend.Text, _tbxSeqPad.Text);
            _tbxSequenceToAdd.Text = "";

            //DataTable CurrentSequences = getSequences();
            //_SequenceList.DataSource = CurrentSequences;
            DataBind();
            _SequenceList.SelectedValue = SequenceId.ToString();
        }



        protected void SequenceSelect_Change( object sender, EventArgs e )
        {
            // BZ 8516 - we do this in prerender now
            // setSequenceFields( Convert.ToInt32( _SequenceList.SelectedItem.Value ) );
        }

        #endregion Events

        #region Helpers

        public DataTable getSequences()
        {

            return (_CswNbtSequenceManager.CurrentSequences);
        }

        public string formatExampleSequence(Int32 SequenceId, Int32 RawSequenceVal)
        {
            CswNbtSequenceValue CswNbtSequenceValue = new CswNbtSequenceValue(_CswNbtResources);
            CswNbtSequenceValue.SequenceId = SequenceId;
            return (CswNbtSequenceValue.makeExample(RawSequenceVal));
        }//formatExampleSequence()

        public DataTable getSequence(Int32 SequenceId)
        {
            return (_CswNbtSequenceManager.getSequence(SequenceId));
        }//getSequence() 

        private void setSequenceFields(Int32 SequenceId)
        {
            DataRow CurrentSequenceRow = getSequence(SequenceId).Rows[0];

            //if( !CurrentSequenceRow.IsNull( "sequencename" ) && CurrentSequenceRow["sequencename"].ToString().Trim().Length > 0 )
            //    _tbxSequenceToAdd.Text = CurrentSequenceRow["sequencename"].ToString();
            //else
            //    _tbxSequenceToAdd.Text = "";

            if( !CurrentSequenceRow.IsNull( "prep" ) && CurrentSequenceRow["prep"].ToString().Trim().Length > 0 )
                _tbxSeqPrepend.Text = CurrentSequenceRow["prep"].ToString();
            else
                _tbxSeqPrepend.Text = "";


            if (!CurrentSequenceRow.IsNull("post") && CurrentSequenceRow["post"].ToString().Trim().Length > 0)
                _tbxSeqPostpend.Text = CurrentSequenceRow["post"].ToString();
            else
                _tbxSeqPostpend.Text = "";

            if (!CurrentSequenceRow.IsNull("pad") && CurrentSequenceRow["pad"].ToString().Trim().Length > 0)
                _tbxSeqPad.Text = CurrentSequenceRow["pad"].ToString();
            else
                _tbxSeqPad.Text = "";

            _lblSequenceExample.Text = formatExampleSequence( SequenceId, 127 );

        }

        public void assignSequence(string SequenceName, Int32 NodeTypePropId)
        {
            _CswNbtSequenceManager.assignSequence(SequenceName, NodeTypePropId);
        }

        public void unAssignSequence(Int32 NodeTypePropId)
        {
            _CswNbtSequenceManager.unAssignSequence(NodeTypePropId);
        }

        public void removeSequence(string SequenceName)
        {
            _CswNbtSequenceManager.removeSequence(SequenceName);
        }

        public Int32 makeSequence(string SequenceName, string PrePend, string PostPend, string Pad)
        {
            return _CswNbtSequenceManager.makeSequence(SequenceName, PrePend, PostPend, Pad ,1);
        }
        #endregion Helpers
    }
}
