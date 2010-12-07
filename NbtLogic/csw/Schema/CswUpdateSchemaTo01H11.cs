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
    /// Updates the schema to version 01H-11
    /// </summary>
    public class CswUpdateSchemaTo01H11 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 11 ); } }
        public CswUpdateSchemaTo01H11( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // Case 20432
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataNodeTypeProp EmailNTP;
            CswNbtMetaDataNodeTypeProp UsernameNTP;
            String Username = "";
            foreach( CswNbtMetaDataNodeType NodeType in UserOC.NodeTypes )
            {
                EmailNTP = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassUser.EmailPropertyName );
                UsernameNTP = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassUser.UsernamePropertyName );
                foreach( CswNbtNode Node in NodeType.getNodes( true, false ) )
                {
                    Username = Node.Properties[UsernameNTP].AsText.Text;
                    if( String.Empty == Node.Properties[EmailNTP].AsText.Text )
                        Node.Properties[EmailNTP].AsText.Text = Username + "@local";
                }
            }

            CswNbtMetaDataObjectClassProp EmailOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.EmailPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( EmailOCP, "isrequired", CswConvert.ToDbVal( true ) );



        } // update()

    }//class CswUpdateSchemaTo01H11

}//namespace ChemSW.Nbt.Schema


