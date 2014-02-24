using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 31545
    /// </summary>
    public class CswUpdateSchema_02K_Case31545 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31545; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            // Remove C3ACDPreferredSuppliers from all layouts
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp C3ACDPrefSuppliersNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.C3ACDPreferredSuppliers );
                C3ACDPrefSuppliersNTP.removeFromAllLayouts();
            }

        } // update()

    }//class CswUpdateSchema_02K_Case31545

}//namespace ChemSW.Nbt.Schema