
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
namespace ChemSW.Nbt.ImportExport
{

    public class CswNbtMetaDataForSpreadSheetColReader
    {
        private Dictionary<string, string> _NodeTypeNameMapper = new Dictionary<string, string>();
        private Dictionary<string, string> _NodeTypePropNameMapper = new Dictionary<string, string>();
        private Dictionary<string, CswNbtMetaDataNodeType> _NodeTypesPerSpreadsheetRowCols = new Dictionary<string, CswNbtMetaDataNodeType>();

        private CswNbtResources _CswNbtResources = null;
        public CswNbtMetaDataForSpreadSheetColReader( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            //Add nodetype name mappings to _NodeTypeNameMapper


            //Add nodetype propname mappings to _NodeTypePropNameMapper


        }//ctor

        public CswNbtMetaDataForSpreadSheetCol read( string NodeTypeNameColVal, string PropNameColVal, ref string Message )
        {
            CswNbtMetaDataForSpreadSheetCol ReturnVal = new CswNbtMetaDataForSpreadSheetCol();


            if( _NodeTypesPerSpreadsheetRowCols.ContainsKey( NodeTypeNameColVal ) )
            {
                ReturnVal.CswNbtMetaDataNodeType = _NodeTypesPerSpreadsheetRowCols[NodeTypeNameColVal];
            }
            else
            {

                if( null != ( ReturnVal.CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeType( NodeTypeNameColVal ) ) )
                {
                    _NodeTypesPerSpreadsheetRowCols.Add( NodeTypeNameColVal, ReturnVal.CswNbtMetaDataNodeType );

                }
                else
                {

                    string OfficialNodeTypeName = string.Empty;
                    if( _NodeTypeNameMapper.ContainsKey( NodeTypeNameColVal ) )
                    {
                        OfficialNodeTypeName = _NodeTypeNameMapper[NodeTypeNameColVal];
                        if( null != ( ReturnVal.CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeType( OfficialNodeTypeName ) ) )
                        {
                            _NodeTypesPerSpreadsheetRowCols.Add( NodeTypeNameColVal, ReturnVal.CswNbtMetaDataNodeType );
                        }
                        else
                        {
                            Message = "The officially mapped node type name ( " + OfficialNodeTypeName + ") for the spreadsheet column nodetype name " + NodeTypeNameColVal + " does not correspond to a known node type";

                        }//even the offiial name doesn't map
                    }
                    else
                    {
                        Message = "The spreadsheet column nodetype name " + NodeTypeNameColVal + " does not correspond to a known node type and has no mapping";

                    }//if we have a mapped nodetype name
                }
            }//if we already have the nodetype


            if( null != ReturnVal.CswNbtMetaDataNodeType )
            {

                if( null == ( ReturnVal.CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( PropNameColVal ) ) )
                {
                    if( _NodeTypePropNameMapper.ContainsKey( PropNameColVal ) )
                    {

                        string OfficialPropName = _NodeTypePropNameMapper[PropNameColVal];
                        if( null == ( ReturnVal.CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( OfficialPropName ) ) )
                        {

                        }

                    }
                    else
                    {

                    }
                }//if the spreadsheet prop name did not get a prop
            }

            return ( ReturnVal );

        }//read

    } // CswNbtMetaDataForSpreadSheetColReader


} // namespace ChemSW.Nbt
