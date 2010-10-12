using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls
{
    public class CswNodeTypeDropDown : DropDownList
    {
        private CswNbtResources _CswNbtResources;
        public CswNodeTypeDropDown( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
            base.SelectedIndexChanged += new EventHandler( CswNodeTypeSelect_SelectedIndexChanged );
        }

        /// <summary>
        /// Include a blank select option
        /// </summary>
        public bool IncludeBlank = false;
        
        /// <summary>
        /// Constrain options to a given object class
        /// </summary>
        public Int32 ConstrainToObjectClassId = Int32.MinValue;

        private static string _ObjectClassPrefix = "oc_";
        private static string _NodeTypePrefix = "nt_";

        public override void DataBind()
        {
            this.Items.Clear();

            if( IncludeBlank )
                this.Items.Add( new ListItem( "Select...", "" ) );

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
            {
                if( ConstrainToObjectClassId == Int32.MinValue || NodeType.ObjectClass.ObjectClassId == ConstrainToObjectClassId ) 
                    this.Items.Add( new ListItem( NodeType.NodeTypeName, _NodeTypePrefix + NodeType.FirstVersionNodeTypeId.ToString() ) );
            }

            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
            {
                if( ConstrainToObjectClassId == Int32.MinValue || ObjectClass.ObjectClassId == ConstrainToObjectClassId )
                    this.Items.Add( new ListItem( "All " + ObjectClass.ObjectClass.ToString(), _ObjectClassPrefix + ObjectClass.ObjectClassId.ToString() ) );
            }

            base.DataBind();
        }

        public Int32 SelectedObjectClassId
        {
            get
            {
                EnsureChildControls();
                Int32 ret = Int32.MinValue;
                if( this.SelectedValue != string.Empty && this.SelectedValue.StartsWith( _ObjectClassPrefix ) )
                    ret = Convert.ToInt32( this.SelectedValue.Substring( _ObjectClassPrefix.Length ) );
                return ret;
            }
            set
            {
                string NewValue = _ObjectClassPrefix + value.ToString();
                if( this.Items.FindByValue( NewValue ) != null )
                    this.SelectedValue = NewValue;
            }
        }
        public Int32 SelectedNodeTypeFirstVersionId
        {
            get
            {
                EnsureChildControls();
                Int32 ret = Int32.MinValue;
                if( this.SelectedValue != string.Empty && this.SelectedValue.StartsWith( _NodeTypePrefix ) )
                    ret = Convert.ToInt32( this.SelectedValue.Substring( _NodeTypePrefix.Length ) );
                return ret;
            }
            set
            {
                string NewValue = _NodeTypePrefix + value.ToString();
                if( this.Items.FindByValue( NewValue ) != null )
                    this.SelectedValue = NewValue;
            }
        }
        public CswNbtMetaDataNodeType SelectedNodeTypeLatestVersion
        {
            get
            {
                CswNbtMetaDataNodeType ret = null;
                Int32 SNTId = SelectedNodeTypeFirstVersionId;
                if( SNTId != Int32.MinValue )
                    ret = _CswNbtResources.MetaData.getNodeType( SNTId ).LatestVersionNodeType;
                return ret;
            }
        }


        #region Events

        public delegate void SelectedNodeTypeChangedHandler( CswNbtViewRelationship.RelatedIdType SelectedType, Int32 SelectedValue );
        public event SelectedNodeTypeChangedHandler SelectedNodeTypeChanged = null;

        void CswNodeTypeSelect_SelectedIndexChanged( object sender, EventArgs e )
        {
            CswNbtViewRelationship.RelatedIdType SelectedType = CswNbtViewRelationship.RelatedIdType.Unknown;
            Int32 SelectedVal = Int32.MinValue;

            if( this.SelectedValue.StartsWith( _NodeTypePrefix ) )
            {
                SelectedType = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
                SelectedVal = Convert.ToInt32( this.SelectedValue.Substring( _NodeTypePrefix.Length ) );
            }
            else if( this.SelectedValue.StartsWith( _ObjectClassPrefix ) )
            {
                SelectedType = CswNbtViewRelationship.RelatedIdType.ObjectClassId;
                SelectedVal = SelectedObjectClassId;
            }

            if( SelectedNodeTypeChanged != null )
                SelectedNodeTypeChanged( SelectedType, SelectedVal );
        }

        #endregion Events
    }
}
