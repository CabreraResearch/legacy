using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
    public class CswUpdateSchema_02H_Case30690B: CswUpdateSchemaTo
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
            get { return "02H_Case" + CaseNo + "B"; }
        }

        public override string Title
        {
            get { return "Create GHS Signal Word nodes, update existing GHS nodes"; }
        }

        public override void update()
        {
            Dictionary<string, string> DangerDictionary = new Dictionary<string, string>()
                {
                    {"Bulgarian", "опасност"},
                    {"Spanish", "peligro"},
                    {"Chinese", "危险"},
                    {"Czech", "nebezpečí"},
                    {"Danish", "Danger"},
                    {"German", "Danger"},
                    {"Estonian", "oht"},
                    {"Greek", "κίνδυνος"},
                    {"English", "Danger"},
                    {"French", "danger"},
                    {"Irish", "Danger"},
                    {"Italian", "pericolo"},
                    {"Latvian", "briesmas"},
                    {"Lithuanian", "pavojus"},
                    {"Hungarian", "veszély"},
                    {"Maltese", "periklu"},
                    {"Dutch", "gevaar"},
                    {"Polish", "niebezpieczeństwo"},
                    {"Portuguese", "perigo"},
                    {"Romanian", "pericol"},
                    {"Slovac", "nebezpečenstvo"},
                    {"Slovenian", "nevarnost"},
                    {"Finnish", "vaara"},
                    {"Swedish", "fara"},
                };

            Dictionary<string, string> WarningDictionary = new Dictionary<string, string>()
                {
                    {"Bulgarian", "Предупреждение"},
                    {"Spanish", "Advertencia"},
                    {"Chinese", "警告"},
                    {"Czech", "Upozornění"},
                    {"Danish", "advarsel"},
                    {"German", "Warnung"},
                    {"Estonian", "Hoiatus"},
                    {"Greek", "προειδοποίηση"},
                    {"English", "Warning"},
                    {"French", "avertissement"},
                    {"Irish", "rabhadh"},
                    {"Italian", "Attenzione"},
                    {"Latvian", "brīdinājums"},
                    {"Lithuanian", "Įspėjimas"},
                    {"Hungarian", "Warning"},
                    {"Maltese", "twissija"},
                    {"Dutch", "waarschuwing"},
                    {"Polish", "Ostrzeżenie"},
                    {"Portuguese", "aviso"},
                    {"Romanian", "Atenție"},
                    {"Slovac", "Upozornenie"},
                    {"Slovenian", "Opozorilo"},
                    {"Finnish", "Varoitus"},
                    {"Swedish", "Varning"},
                };


            CswNbtMetaDataObjectClass GHSOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            CswNbtMetaDataObjectClass GHSSignalWordOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSSignalWordClass );
            CswNbtMetaDataNodeType SignalWordNodeType = GHSSignalWordOC.getNodeTypes().FirstOrDefault();
            if( null == SignalWordNodeType )
            {
                SignalWordNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GHSSignalWordOC.ObjectClassId, "GHS Signal Word", "System" );
                SignalWordNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassGHSSignalWord.PropertyName.Code ) );

                CswNbtView SignalWordRelationshipPropView = _CswNbtSchemaModTrnsctn.makeSafeView( "GHS Signal Word Relationship Prop View", CswEnumNbtViewVisibility.Hidden );
                SignalWordRelationshipPropView.AddViewRelationship( GHSSignalWordOC, false );
                SignalWordRelationshipPropView.save();

                CswNbtMetaDataNodeTypeProp EnglishNTP = SignalWordNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassGHSSignalWord.PropertyName.English );
                EnglishNTP.updateLayout( CswEnumNbtLayoutType.Add, true );

                foreach(CswNbtMetaDataNodeTypeProp SignalWordNTP in GHSOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.SignalWord ).getNodeTypeProps())
                {
                    SignalWordNTP.ViewId = SignalWordRelationshipPropView.ViewId;
                }

            }
            CswNbtNode DangerNode = _createSignalWord( SignalWordNodeType, DangerDictionary );
            CswNbtNode WarningNode = _createSignalWord( SignalWordNodeType, WarningDictionary );

            CswCommaDelimitedString NodeIds = new CswCommaDelimitedString();
            foreach( KeyValuePair<CswPrimaryKey, string> Pair in GHSOC.getNodeIdAndNames( false, true, false, true ) )
            {
                NodeIds.Add( Pair.Key.PrimaryKey.ToString() );
            }

            _updateGHSNodes( "Danger", DangerNode, NodeIds, GHSOC );
            _updateGHSNodes( "Warning", WarningNode, NodeIds, GHSOC );


        } // update()

        private CswNbtNode _createSignalWord( CswNbtMetaDataNodeType SignalWordNodeType, Dictionary<string, string> Translations )
        {
            return _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SignalWordNodeType.NodeTypeId, OnAfterMakeNode : delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassGHSSignalWord NewSignalWord = (CswNbtObjClassGHSSignalWord) NewNode;

                NewSignalWord.Code.Text = Translations["English"]; //Default Code to English

                foreach( string Language in CswNbtPropertySetPhrase.SupportedLanguages.All )
                {
                    NewNode.Properties[Language].AsText.Text = Translations[Language];
                }
            } );
        }

        private void _updateGHSNodes( string Word, CswNbtNode Related, CswCommaDelimitedString NodeIds, CswNbtMetaDataObjectClass GHSOC )
        {
            if( NodeIds.Count > 0 )
            {
                Collection<int> NodesToUpdate = new Collection<int>();
                string sql = "where nodeid in (" + NodeIds + ") and field1 = '" + Word + "'";
                CswTableUpdate PropUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "UpdateSignalWordPropsWarning", "jct_nodes_props" );
                DataTable PropsTbl = PropUpdate.getTable( sql );
                foreach( DataRow row in PropsTbl.Rows )
                {
                    NodesToUpdate.Add( CswConvert.ToInt32( row["nodeid"] ) );
                    row["gestaltsearch"] = "";
                    row["gestalt"] = "";
                    row["field1"] = "";
                    row["field2"] = "";
                }
                PropUpdate.update( PropsTbl );

                //Now update the nodes to have the REAL value for the signal word prop
                foreach( CswNbtObjClassGHS GHSNode in GHSOC.getNodes( false, true, false, true ).Where( Node => NodesToUpdate.Contains( Node.NodeId.PrimaryKey ) ) )
                {
                    GHSNode.SignalWord.RelatedNodeId = Related.NodeId;
                    GHSNode.postChanges( false );
                }
            }
        }

    }

}//namespace ChemSW.Nbt.Schema