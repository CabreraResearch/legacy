using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27876
    /// </summary>
    public class CswUpdateSchema_01U_Case27876 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27876; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp CASNoOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.CasNo );
            foreach( CswNbtMetaDataNodeTypeProp casNoNTP in CASNoOCP.getNodeTypeProps() )
            {
                casNoNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }

            CswNbtMetaDataNodeType chemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != chemicalNT )
            {
                CswNbtMetaDataNodeTypeTab chemicalTab = chemicalNT.getFirstNodeTypeTab();
                CswNbtMetaDataNodeTypeProp chemicalCASNoNTP = chemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.CasNo );
                CswNbtMetaDataNodeTypeProp chemicalSynoNTP = chemicalNT.getNodeTypeProp( "Synonyms" );
                if( null != chemicalSynoNTP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, chemicalCASNoNTP, chemicalSynoNTP, true );
                }
            }

        }

        //Update()

    }//class CswUpdateSchemaCase27876

}//namespace ChemSW.Nbt.Schema