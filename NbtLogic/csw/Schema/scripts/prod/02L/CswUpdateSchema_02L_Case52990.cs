using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52990B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 52990; }
        }

        public override string Title
        {
            get { return "Update Supply and Biological Edit Layout to include new Expiration Props"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }


        public override void update()
        {

            CswNbtMetaDataObjectClass BiologicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BiologicalClass );
           CswNbtMetaDataObjectClass SupplyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass );

            foreach( CswNbtMetaDataObjectClass MaterialOC in new[] {BiologicalOC, SupplyOC} )
            {
                CswNbtMetaDataObjectClassProp OpenExpireOCP = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.OpenExpireInterval );
                CswNbtMetaDataObjectClassProp ExpirationIntervalOCP = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.ExpirationInterval );
                foreach( CswNbtMetaDataObjectClassProp OCP in new[] {OpenExpireOCP, ExpirationIntervalOCP} )
                {
                    foreach( CswNbtMetaDataNodeTypeProp NTP in OCP.getNodeTypeProps() )
                    {
                        CswNbtMetaDataNodeType NT = NTP.getNodeType();
                        NTP.updateLayout( CswEnumNbtLayoutType.Edit, true, NT.getFirstNodeTypeTab().TabId );
                    }
                }
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema