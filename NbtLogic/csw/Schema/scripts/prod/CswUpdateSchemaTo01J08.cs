using System;
using System.Data;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-08
    /// </summary>
    public class CswUpdateSchemaTo01J08 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 08 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 23936

            DataTable Views = _CswNbtSchemaModTrnsctn.getAllViews();
            foreach( DataRow ViewRow in Views.Rows )
            {
                Int32 ViewId = CswConvert.ToInt32( ViewRow["nodeviewid"] );
                if( Int32.MinValue != ViewId )
                {
                    CswNbtViewId NbtViewId = new CswNbtViewId( ViewId );
                    if( NbtViewId.isSet() )
                    {
                        CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( NbtViewId );
                        if( null != View )
                        {
                            View.save();
                        }
                    }
                }
            }

            #endregion Case 23936
        }//Update()

    }//class CswUpdateSchemaTo01J08

}//namespace ChemSW.Nbt.Schema


