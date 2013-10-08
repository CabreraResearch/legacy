using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30537B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30537; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo + "B"; }
        }

        public override string Title
        {
            get { return "Create Phrase Property Set and DSD NodeType"; }
        }

        public override void update()
        {

            CswNbtMetaDataPropertySet PhrasePS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.PhraseSet );
            if( null == PhrasePS )
            {
                PhrasePS = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropertySet( CswEnumNbtPropertySetName.PhraseSet, "warning.png" );

                //Update the DSD and GHS Phrases
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatePhrasePropSets", "jct_propertyset_objectclass" );

                DataTable ObjClassTbl = TableUpdate.getEmptyTable();
                DataRow GHSRow = ObjClassTbl.NewRow();
                DataRow DSDRow = ObjClassTbl.NewRow();

                GHSRow["propertysetid"] = PhrasePS.PropertySetId;
                GHSRow["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswEnumNbtObjectClass.GHSPhraseClass );

                DSDRow["propertysetid"] = PhrasePS.PropertySetId;
                DSDRow["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswEnumNbtObjectClass.DSDPhraseClass );

                ObjClassTbl.Rows.Add( GHSRow );
                ObjClassTbl.Rows.Add( DSDRow );

                TableUpdate.update( ObjClassTbl );
            }

            CswNbtMetaDataNodeType DSD_NT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswEnumNbtObjectClass.DSDPhraseClass, "DSD Phrase", "System" );
            DSD_NT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtPropertySetPhrase.PropertyName.Code ) );
            foreach( CswNbtMetaDataNodeTypeProp NTP in DSD_NT.getNodeTypeProps() )
            {
                if( CswNbtPropertySetPhrase.PropertyName.English != NTP.PropName &&
                    CswNbtPropertySetPhrase.PropertyName.Code != NTP.PropName &&
                    CswNbtPropertySetPhrase.PropertyName.Category != NTP.PropName )
                {
                    NTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                }
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema