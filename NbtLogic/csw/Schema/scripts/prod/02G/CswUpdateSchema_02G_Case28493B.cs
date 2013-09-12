using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case28493B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28493; }
        }

        public override string ScriptName
        {
            get { return "02G_Case28493B"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GHSPhraseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            Dictionary<string, CswNbtObjClassGHSPhrase> GHSPhrases = _getAllGHSPhrases();

            string FilePath = CswFilePath.getConfigurationFilePath( CswEnumSetupMode.NbtExe ) + "\\" + "signalwords_hs_ps_en.xlsx";
            DataSet GHSLanguageData = _readExcel( FilePath );

            //Note - we are ignoring the signal words sheet here
            DataTable HazardStatementsTbl = GHSLanguageData.Tables["'Hazard statements$'"];
            DataTable PrecationaryStatementsTbl = GHSLanguageData.Tables["'Precautionary statements $'"];
            
            _handleData( HazardStatementsTbl, 2, 91, GHSPhrases, GHSPhraseOC.getNodeTypeIds().FirstOrDefault() );
            _handleData( PrecationaryStatementsTbl, 3, 142, GHSPhrases, GHSPhraseOC.getNodeTypeIds().FirstOrDefault() );
        }


        private void _handleData( DataTable Tbl, int LanguageRow, int MaxRows, Dictionary<string, CswNbtObjClassGHSPhrase> GHSPhrases, int GHSPhraseNTId )
        {
            DataRow LangRow = Tbl.Rows[LanguageRow];
            for( int i = LanguageRow + 1; i <= MaxRows; i++ )
            {
                DataRow HazardsRow = Tbl.Rows[i];

                string code = HazardsRow[0].ToString();

                if( GHSPhrases.ContainsKey( code ) )
                {
                    CswNbtObjClassGHSPhrase GHSPhrase = GHSPhrases[code];
                    for( int c = 1; c <= 23; c++ )
                    {
                        string Language = LanguageCodeMap[LangRow[c].ToString().Trim()];
                        string LanguageText = HazardsRow[c].ToString();
                        GHSPhrase.Node.Properties[Language].AsText.Text = LanguageText;
                        GHSPhrase.postChanges( false );
                    }
                }
                else
                {
                    _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( GHSPhraseNTId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassGHSPhrase NodeAsGHSPhrase = NewNode;
                        NodeAsGHSPhrase.Code.Text = code;
                        for( int c = 1; c <= 23; c++ )
                        {
                            string Language = LanguageCodeMap[LangRow[c].ToString().Trim()];
                            string LanguageText = HazardsRow[c].ToString();
                            NodeAsGHSPhrase.Node.Properties[Language].AsText.Text = LanguageText;
                        }
                    } );
                }
            }
        }


        private readonly Dictionary<string, string> LanguageCodeMap = new Dictionary<string, string>
            {
                {"BG", "Bulgarian"},
                {"ES", "Spanish"},
                {"CS", "Czech"},
                {"DA", "Danish"},
                {"DE", "German"},
                {"ET", "Estonian"},
                {"EL", "Greek"},
                {"EN", "English"},
                {"FR", "French"},
                {"GA", "Irish"},
                {"IT", "Italian"},
                {"LV", "Latvian"},
                {"LT", "Lithuanian"},
                {"HU", "Hungarian"},
                {"MT", "Maltese"},
                {"NL", "Dutch"},
                {"PL", "Polish"},
                {"PT", "Portuguese"},
                {"RO", "Romanian"},
                {"SK", "Slovac"},
                {"SL", "Slovenian"},
                {"FI", "Finnish"},
                {"SV", "Swedish"}
            };

        private Dictionary<string, CswNbtObjClassGHSPhrase> _getAllGHSPhrases()
        {
            Dictionary<string, CswNbtObjClassGHSPhrase> Ret = new Dictionary<string, CswNbtObjClassGHSPhrase>();

            CswNbtMetaDataObjectClass GHSPhraseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            foreach( CswNbtObjClassGHSPhrase GHSPhrase in GHSPhraseOC.getNodes( false, true, false, true ) )
            {
                //Potential exception here - is "Code" unique? Probably should be
                Ret[GHSPhrase.Code.Text] = GHSPhrase;
            }

            return Ret;
        }

        private DataSet _readExcel( string FilePath )
        {
            DataSet ret = new DataSet();

            //Set up ADO connection to spreadsheet
            string ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=Excel 8.0;";
            OleDbConnection ExcelConn = new OleDbConnection( ConnStr );
            ExcelConn.Open();

            DataTable ExcelMetaDataTable = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
            if( null == ExcelMetaDataTable )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Invalid File", "Could not process the excel file: " + FilePath );
            }

            foreach( DataRow ExcelMetaDataRow in ExcelMetaDataTable.Rows )
            {
                string SheetName = ExcelMetaDataRow["TABLE_NAME"].ToString();

                OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + SheetName + "]", ExcelConn );
                DataAdapter.SelectCommand = SelectCommand;

                DataTable ExcelDataTable = new DataTable( SheetName );
                DataAdapter.Fill( ExcelDataTable );

                ret.Tables.Add( ExcelDataTable );
            }
            return ret;
        }



    }

}//namespace ChemSW.Nbt.Schema