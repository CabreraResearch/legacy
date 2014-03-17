using System.Linq;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52788: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 52788; }
        }

        public override string Title
        {
            get { return "Update Modules Enabled on Master"; }
        }

        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                string[] ModulesToEnable = new[]
                    {
                        CswEnumNbtModuleName.CISPro,
                        CswEnumNbtModuleName.Containers,
                        CswEnumNbtModuleName.RegulatoryLists,
                        CswEnumNbtModuleName.SDS
                    };
                foreach( CswEnumNbtModuleName module in CswEnumNbtModuleName.All )
                {
                    if( ModulesToEnable.Contains( module.ToString() ) )
                    {
                        _CswNbtSchemaModTrnsctn.Modules.EnableModule( module );
                    }
                    else
                    {
                        _CswNbtSchemaModTrnsctn.Modules.DisableModule( module );
                    }
                }//for each module
            }//if is master
        } // update()

    }

}//namespace ChemSW.Nbt.Schema