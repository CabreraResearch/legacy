using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52312 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52312; }
        }

        public override string Title
        {
            get { return "Add new InventoryGroup OCPs"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass InvGrpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClassProp LRDLOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( InvGrpOC, new CswNbtWcfMetaDataModel.ObjectClassProp( InvGrpOC )
            {
                PropName = CswNbtObjClassInventoryGroup.PropertyName.LimitRequestDeliveryLocation,
                FieldType = CswEnumNbtFieldType.Logical,
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( LRDLOCP, CswEnumTristate.False, CswEnumNbtSubFieldName.Checked );
            CswNbtMetaDataObjectClassProp ACAOCP = InvGrpOC.getObjectClassProp( CswNbtObjClassInventoryGroup.PropertyName.AutomaticCertificateApproval );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ACAOCP, CswEnumNbtObjectClassPropAttributes.isrequired, false );
            

            foreach( CswNbtMetaDataNodeType InvGrpNT in InvGrpOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp CentralNTP = InvGrpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroup.PropertyName.Central );
                CswNbtMetaDataNodeTypeProp ACANTP = InvGrpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroup.PropertyName.AutomaticCertificateApproval );
                ACANTP.DesignNode.Required.Checked = CswEnumTristate.False;
                ACANTP.DesignNode.postOnlyChanges( false, true );
                ACANTP.setFilter( CentralNTP, CswEnumNbtSubFieldName.Checked, CswEnumNbtFilterMode.Equals, CswEnumTristate.True );
            }
        }
    }
}