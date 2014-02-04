using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31829: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31829; }
        }

        public override string Title
        {
            get { return "Move Kiosk Mode to Containers category for new customers"; }
        }

        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                _CswNbtSchemaModTrnsctn.Actions[CswEnumNbtActionName.Kiosk_Mode].SetCategory( CswEnumNbtCategory.Containers );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema