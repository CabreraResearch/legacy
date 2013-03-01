using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28754
    /// </summary>
    public class CswUpdateSchema_01W_Case28754 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28754; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass invGrpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryGroupPermissionClass );
            CswNbtMetaDataObjectClassProp workUnitOCP = invGrpOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.WorkUnit );

            CswNbtMetaDataObjectClass workUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.WorkUnitClass );
            CswNbtMetaDataObjectClassProp nameOCP = workUnitOC.getObjectClassProp( CswNbtObjClassWorkUnit.PropertyName.Name );

            CswNbtView workUnits = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship parent = workUnits.AddViewRelationship( workUnitOC, false );
            workUnits.AddViewPropertyAndFilter( parent, nameOCP,
                Value: "Default Work Unit",
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree tree = _CswNbtSchemaModTrnsctn.getTreeFromView( workUnits, true );
            CswPrimaryKey nodeid = null;
            if( tree.getChildNodeCount() > 0 )
            {
                tree.goToNthChild( 0 );
                nodeid = tree.getNodeIdForCurrentPosition();
            }

            if( null != nodeid )
            {
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( workUnitOCP, nodeid.PrimaryKey, CswNbtSubField.SubFieldName.NodeID );
            }

        } //Update()

    }//class CswUpdateSchema_01W_Case28754

}//namespace ChemSW.Nbt.Schema