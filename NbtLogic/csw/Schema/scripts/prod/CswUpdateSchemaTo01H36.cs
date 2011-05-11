
using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-36
    /// </summary>
    public class CswUpdateSchemaTo01H36 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 36 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H36( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // case 21660
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp PageSizeOcp = UserOC.getObjectClassProp( CswNbtObjClassUser.PageSizePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( PageSizeOcp, CswNbtSubField.SubFieldName.Value, "30" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PageSizeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PageSizeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            foreach( CswNbtNode UserNode in from UserNode
                                                in UserOC.getNodes( true, false )
                                            let User = CswNbtNodeCaster.AsUser( UserNode )
                                            where User.PageSize == Int32.MinValue
                                            select UserNode )
            {
                UserNode.Properties[PageSizeOcp.PropName].AsNumber.Value = 30;
            }

        } // update()

    }//class CswUpdateSchemaTo01H36

}//namespace ChemSW.Nbt.Schema

