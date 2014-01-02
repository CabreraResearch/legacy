using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case31464 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31464; }
        }

        public override string Title
        {
            get { return "Set value of 'hidden' column in modules table"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            // We want to hide the LOLI Sync module
            CswTableUpdate ModulesTblUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "modulesTblSetHiddenValue_Case31464", "modules" );
            DataTable ModulesTbl = ModulesTblUpdate.getTable();
            foreach( DataRow Row in ModulesTbl.Rows )
            {
                if( false == Row["name"].ToString().Equals( CswEnumNbtModuleName.LOLISync ) )
                {
                    Row["hidden"] = CswConvert.ToDbVal( false );
                }
                else
                {
                    Row["hidden"] = CswConvert.ToDbVal( true );
                }
            }
            ModulesTblUpdate.update( ModulesTbl );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema