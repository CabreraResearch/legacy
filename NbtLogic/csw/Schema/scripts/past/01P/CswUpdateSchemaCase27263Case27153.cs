using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27263
    /// </summary>
    public class CswUpdateSchemaCase27263Case27153 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp RequestDispenseOcp = ContainerOc.getObjectClassProp( "Request Dispense" );
            if( null != RequestDispenseOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestDispenseOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, CswNbtObjClassContainer.RequestPropertyName );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestDispenseOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended, CswNbtNodePropButton.ButtonMode.menu );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestDispenseOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassContainer.RequestMenu.Options.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestDispenseOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassContainer.RequestMenu.Dispense );
            }
            CswNbtMetaDataObjectClassProp RequestDisposeOcp = ContainerOc.getObjectClassProp( "Request Dispose" );
            if( null != RequestDisposeOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp(RequestDisposeOcp, true);
            }
            
            CswNbtMetaDataObjectClassProp RequestMoveOcp = ContainerOc.getObjectClassProp( "Request Move" );
            if( null != RequestMoveOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp(RequestMoveOcp, true);
            }

        }//Update()

    }//class CswUpdateSchemaCase27263Case27153

}//namespace ChemSW.Nbt.Schema