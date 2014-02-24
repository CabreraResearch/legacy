using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02K_Case31616 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31616; }
        }

        public override void update()
        {
            // nodetypes.searchable
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", "searchable" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetypes", "searchable", "Whether a nodetype is configured to be searchable", false );
            }
        } // update()

    }//class CswUpdateDDL_02K_Case31616
}//namespace ChemSW.Nbt.Schema


