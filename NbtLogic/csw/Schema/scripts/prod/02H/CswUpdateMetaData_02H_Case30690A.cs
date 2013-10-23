using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30690A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30690; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "A"; }
        }

        public override string Title
        {
            get { return "Create GHS Signal Word class"; }
        }

        public override void update()
        {

            #region Create GHS Signal Word OC
            
            CswNbtMetaDataObjectClass SignalWordOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSSignalWordClass );
            if( null == SignalWordOC )
            {
                SignalWordOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.GHSSignalWordClass, "warning.png", false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( SignalWordOC, new CswNbtWcfMetaDataModel.ObjectClassProp( SignalWordOC )
                    {
                        PropName = CswNbtObjClassGHSSignalWord.PropertyName.Code,
                        FieldType = CswEnumNbtFieldType.Text
                    } );

                foreach( string Language in CswNbtPropertySetPhrase.SupportedLanguages.All )
                {
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( SignalWordOC, new CswNbtWcfMetaDataModel.ObjectClassProp( SignalWordOC )
                        {
                            PropName = Language,
                            FieldType = CswEnumNbtFieldType.Text
                        } );
                }

                //Attached this new OC to the Phrase PS
                CswNbtMetaDataPropertySet PhrasePS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.PhraseSet );
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatePhrasePropSets", "jct_propertyset_objectclass" );
                DataTable ObjClassTbl = TableUpdate.getEmptyTable();
                DataRow SignalWordRow = ObjClassTbl.NewRow();

                SignalWordRow["propertysetid"] = PhrasePS.PropertySetId;
                SignalWordRow["objectclassid"] = SignalWordOC.ObjectClassId;

                ObjClassTbl.Rows.Add( SignalWordRow );

                TableUpdate.update( ObjClassTbl );
            }

            #endregion

            #region Make GHS Signal Word prop a relationship

            CswNbtMetaDataObjectClass GHS_OC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            CswNbtMetaDataObjectClassProp SignalWordOCP = GHS_OC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.SignalWord );
            CswNbtMetaDataFieldType RelationshipFieldType = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Relationship );

            CswTableUpdate SignalWordOCPTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "UpdateSignalWord", "object_class_props" );
            DataTable ObjClassPropsTbl = SignalWordOCPTableUpdate.getTable( "where objectclasspropid = " + SignalWordOCP.ObjectClassPropId );
            foreach( DataRow row in ObjClassPropsTbl.Rows )
            {
                row["fieldtypeid"] = RelationshipFieldType.FieldTypeId;
                row["isfk"] = CswConvert.ToDbVal( true );
                row["fkvalue"] = SignalWordOC.ObjectClassId;
                row["fktype"] = "ObjectClassId";
            }
            SignalWordOCPTableUpdate.update( ObjClassPropsTbl );

            CswTableUpdate SignalWordNTPTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "UpdateSignalWord", "nodetype_props" );
            DataTable NodeTypePropsTbl = SignalWordNTPTableUpdate.getTable( "where objectclasspropid = " + SignalWordOCP.ObjectClassPropId );
            foreach( DataRow row in NodeTypePropsTbl.Rows )
            {
                row["fieldtypeid"] = RelationshipFieldType.FieldTypeId;
                row["isfk"] = CswConvert.ToDbVal( true );
                row["fkvalue"] = SignalWordOC.ObjectClassId;
                row["fktype"] = "ObjectClassId";
            }
            SignalWordNTPTableUpdate.update( NodeTypePropsTbl );

            #endregion

        } // update()

    }

}//namespace ChemSW.Nbt.Schema