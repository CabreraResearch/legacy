using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28395
    /// </summary>
    public class CswUpdateSchema_01W_Case28395 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28395; }
        }

        public override void update()
        {
            //Add TierII Reporting Action
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Tier_II_Reporting, true, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Tier_II_Reporting );

            //Add Trade Secret Chemical Prop
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                CswNbtMetaDataNodeTypeProp ChemicalSpecialFlagsNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp(ChemicalNT.NodeTypeId, "Special Flags");
                if( null != ChemicalSpecialFlagsNTP )
                {
                    ChemicalSpecialFlagsNTP.ListOptions = "EHS,Waste,Not Reportable,Trade Secret";
                }
            }

            //Add Container fire reporting props to Audit Table
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataNodeType ContainerNT = ContainerOC.FirstNodeType;
            if( null != ContainerNT )
            {
                CswNbtMetaDataNodeTypeProp StorageTemperatureNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp(ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StorageTemperature);
                StorageTemperatureNTP.AuditLevel = AuditLevel.PlainAudit;
                CswNbtMetaDataNodeTypeProp StoragePressureNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp(ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StoragePressure);
                StoragePressureNTP.AuditLevel = AuditLevel.PlainAudit;
                CswNbtMetaDataNodeTypeProp UseTypeNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp(ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.UseType);
                UseTypeNTP.AuditLevel = AuditLevel.PlainAudit;
            }
        }//Update()

    }//class CswUpdateSchemaCase_01W_28395

}//namespace ChemSW.Nbt.Schema