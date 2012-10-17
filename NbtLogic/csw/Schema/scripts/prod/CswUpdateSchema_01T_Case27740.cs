using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27740
    /// </summary>
    public class CswUpdateSchema_01T_Case27740 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass invGrpPermOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryGroupPermissionClass );

            //Dispose and Undispose should be required
            CswNbtMetaDataObjectClassProp disposeOCP = invGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Dispose );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( disposeOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            CswNbtMetaDataObjectClassProp undisposeOCP = invGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Undispose );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( undisposeOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            //Set View, Edit, Dispense, Request, Dispose and Undispose default values to unchecked
            CswNbtMetaDataObjectClassProp viewOCP = invGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.View );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( viewOCP, false, CswNbtSubField.SubFieldName.Checked );

            CswNbtMetaDataObjectClassProp editOCP = invGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Edit );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( editOCP, false, CswNbtSubField.SubFieldName.Checked );

            CswNbtMetaDataObjectClassProp dispenseOCP = invGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Dispense );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( dispenseOCP, false, CswNbtSubField.SubFieldName.Checked );

            CswNbtMetaDataObjectClassProp requestOCP = invGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Request );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( requestOCP, false, CswNbtSubField.SubFieldName.Checked );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( disposeOCP, false, CswNbtSubField.SubFieldName.Checked );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( undisposeOCP, false, CswNbtSubField.SubFieldName.Checked );

            //A InvGrpPerm either allows or disallows the following. There are no half measures!
            foreach( CswNbtObjClassInventoryGroupPermission invGrpPermNode in invGrpPermOC.getNodes( false, false, false ) )
            {
                if( Tristate.Null == invGrpPermNode.View.Checked ) invGrpPermNode.View.Checked = Tristate.False;
                if( Tristate.Null == invGrpPermNode.Edit.Checked ) invGrpPermNode.Edit.Checked = Tristate.False;
                if( Tristate.Null == invGrpPermNode.Dispense.Checked ) invGrpPermNode.Dispense.Checked = Tristate.False;
                if( Tristate.Null == invGrpPermNode.Request.Checked ) invGrpPermNode.Request.Checked = Tristate.False;
                if( Tristate.Null == invGrpPermNode.Dispose.Checked ) invGrpPermNode.Dispose.Checked = Tristate.False;
                if( Tristate.Null == invGrpPermNode.Undispose.Checked ) invGrpPermNode.Undispose.Checked = Tristate.False;

                if( invGrpPermNode.Node.ModificationState.Value.Equals( NodeModificationState.Modified ) )
                {
                    invGrpPermNode.postChanges( false );
                }
            }
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27740; }
        }

        //Update()

    }//class CswUpdateSchema_01T_Case27740

}//namespace ChemSW.Nbt.Schema