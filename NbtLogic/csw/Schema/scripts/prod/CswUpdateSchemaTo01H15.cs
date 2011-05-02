using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-15
    /// </summary>
    public class CswUpdateSchemaTo01H15 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 15 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H15( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // case 20449
            // make 'Target Type' on Notifications required

            CswNbtMetaDataObjectClass NotificationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass );
            CswNbtMetaDataObjectClassProp TargetTypeOCP = NotificationOC.getObjectClassProp( CswNbtObjClassNotification.TargetTypePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TargetTypeOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, CswConvert.ToDbVal( true ) );

        } // update()

    }//class CswUpdateSchemaTo01H15

}//namespace ChemSW.Nbt.Schema


