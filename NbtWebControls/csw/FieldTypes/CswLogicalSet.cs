using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Core;
using Telerik.Web.UI;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswLogicalSet : CswFieldTypeWebControl, INamingContainer
    {
        private string _NameColumn = "Name";
        private string _KeyColumn = "Key";


        public CswLogicalSet( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswLogicalSet_DataBinding );
        }


        protected override void OnInit( EventArgs e )
        {
            EnsureChildControls();
            base.OnInit( e );
        }

        private void CswLogicalSet_DataBinding( object sender, EventArgs e )
        {
            if( Prop != null )
            {
                EnsureChildControls();
                if( _EditMode != NodeEditMode.LowRes )
                {
                    DataTable Data = Prop.AsLogicalSet.GetDataAsTable( _NameColumn, _KeyColumn );
                    _CBArray.ReadOnly = ReadOnly;
                    _CBArray.CreateCheckBoxes( Data, _NameColumn, _KeyColumn );
                    _CBArray.Rows = Prop.AsLogicalSet.Rows;
                }
                else
                {
                    _Label.Text = Prop.AsLogicalSet.Gestalt;
                }
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                if( _EditMode != NodeEditMode.LowRes )
                {
                    DataTable Data = Prop.AsLogicalSet.GetDataAsTable( _NameColumn, _KeyColumn );

                    foreach( DataRow Row in Data.Rows )
                    {
                        string YValue = Row[_KeyColumn].ToString();
                        foreach( DataColumn Column in Data.Columns )
                        {
                            if( Column.ColumnName != _NameColumn && Column.ColumnName != _KeyColumn )
                            {
                                string XValue = Column.ColumnName;
                                Prop.AsLogicalSet.SetValue( XValue, YValue, _CBArray.GetValue( YValue, XValue ) );
                            }
                        }
                    }
                    Prop.AsLogicalSet.Save();
                }
            }
        }

        public override void AfterSave()
        {
            DataBind();
        }

        public override void Clear()
        {
            if( Prop != null && _EditMode != NodeEditMode.LowRes )
            {
                DataTable Data = Prop.AsLogicalSet.GetDataAsTable( _NameColumn, _KeyColumn );

                foreach( DataRow Row in Data.Rows )
                {
                    string YValue = Row[_KeyColumn].ToString();
                    foreach( DataColumn Column in Data.Columns )
                    {
                        if( Column.ColumnName != _NameColumn && Column.ColumnName != _KeyColumn )
                        {
                            string XValue = Column.ColumnName;
                            _CBArray.GetCheckBox( YValue, XValue ).Checked = false;
                        }
                    }
                }
            }
        }

        private CswCheckBoxArray _CBArray;
        private Label _Label;
        protected override void CreateChildControls()
        {
            if( _EditMode != NodeEditMode.LowRes )
            {
                _CBArray = new CswCheckBoxArray( _CswNbtResources );
                _CBArray.ID = "cbarray";
                this.Controls.Add( _CBArray );
            }
            else
            {
                _Label = new Label();
                _Label.ID = "ls_label";
                this.Controls.Add( _Label );
            }
            base.CreateChildControls();
        }
    }
}
