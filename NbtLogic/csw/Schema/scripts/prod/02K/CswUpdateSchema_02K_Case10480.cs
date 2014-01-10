using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 10480
    /// </summary>
    public class CswUpdateSchema_02K_Case10480 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 10480; }
        }

        public override string Description
        {
            get { return "Winter is coming."; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DesignNodeTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass );
            foreach( CswNbtObjClassDesignNodeType DesignNodeType in DesignNodeTypeOC.getNodes( false, true, false, true ) )
            {
                DesignNodeType.NodeTypeName.Text = CswFormat.MakeIntoValidName( DesignNodeType.NodeTypeName.Text );
                DesignNodeType.postChanges( false );
            }
        } // update()
    }//class CswUpdateSchema_02K_Case10480
}//namespace ChemSW.Nbt.Schema