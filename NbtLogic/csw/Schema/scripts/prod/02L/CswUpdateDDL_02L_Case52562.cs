using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02L_Case52562 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52562; }
        }

        public override string Title
        {
            get { return "Add PendingEvents column to nodes table"; }
        }

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "pendingevents" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes", "pendingevents", "Determines whether or not external update events are required", false );
            }
        }

    }
}