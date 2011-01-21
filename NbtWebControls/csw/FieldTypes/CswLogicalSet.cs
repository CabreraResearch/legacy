using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ChemSW.Nbt;
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
                DataTable Data = Prop.AsLogicalSet.GetDataAsTable( _NameColumn, _KeyColumn );
                _CBArray.ReadOnly = ReadOnly;
                _CBArray.CreateCheckBoxes( Data, _NameColumn, _KeyColumn );
                _CBArray.Rows = Prop.AsLogicalSet.Rows;
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
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

        private CswCheckBoxArray _CBArray;
        private Label _Label;
        protected override void CreateChildControls()
        {
            _CBArray = new CswCheckBoxArray( _CswNbtResources );
            _CBArray.ID = "cbarray";
            this.Controls.Add( _CBArray );
            base.CreateChildControls();
        }
    }
}
