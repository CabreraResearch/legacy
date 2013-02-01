
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
            _NodeTypeNameMapper.Add( "material", "Chemical" );

            //Add nodetype propname mappings to _NodeTypePropNameMapper
            _NodeTypePropNameMapper.Add( "materialname", "tradename" );

        }//ctor

        public CswNbtMetaDataForSpreadSheetCol read( string NodeTypeNameColVal, string PropTypeNameColVal, ref string Message )
        {
            CswNbtMetaDataForSpreadSheetCol ReturnVal = new CswNbtMetaDataForSpreadSheetCol();


            if( _NodeTypesPerSpreadsheetRowCols.ContainsKey( NodeTypeNameColVal ) )
            {
                ReturnVal.CswNbtMetaDataNodeType = _NodeTypesPerSpreadsheetRowCols[NodeTypeNameColVal];
            }
            else
            {

                if( null != ( ReturnVal.CswNbtMetaDataNodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeNameColVal ) ) )
                {
                    _NodeTypesPerSpreadsheetRowCols.Add( NodeTypeNameColVal, ReturnVal.CswNbtMetaDataNodeType );

                }
                else
                {

                    string OfficialNodeTypeName = string.Empty;
                    if( _NodeTypeNameMapper.ContainsKey( NodeTypeNameColVal ) )
                    {
                        OfficialNodeTypeName = _NodeTypeNameMapper[NodeTypeNameColVal];
                        if( null != ( ReturnVal.CswNbtMetaDataNodeType = _CswNbtResources.MetaData.getNodeType( OfficialNodeTypeName ) ) )
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


                if( null == ( ReturnVal.CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( ReturnVal.CswNbtMetaDataNodeType.NodeTypeId, PropTypeNameColVal ) ) )
                {
                    if( _NodeTypePropNameMapper.ContainsKey( PropTypeNameColVal ) )
                    {

                        string OfficialPropName = _NodeTypePropNameMapper[PropTypeNameColVal];
                        if( null == ( ReturnVal.CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( ReturnVal.CswNbtMetaDataNodeType.NodeTypeId, OfficialPropName ) ) )
                        {
                            Message = "The officially mapped node type name ( " + OfficialPropName + ") for the spreadsheet column nodetypeprop name " + PropTypeNameColVal + " does not correspond to a known node type prop";

                        }

                    }
                    else
                    {
                        Message = "The prop column " + PropTypeNameColVal + " for node type " + ReturnVal.CswNbtMetaDataNodeType.NodeTypeName + " does not correspond to a known node type and has no mapping";
                    }

                }//if the spreadsheet prop name did not get a prop

            }//if we have a nodetype

            //If we don't have both, reset return to null client will know to get the error message
            if( null == ReturnVal.CswNbtMetaDataNodeType || null == ReturnVal.CswNbtMetaDataNodeTypeProp )
            {
                ReturnVal = null;
            }

            return ( ReturnVal );

        }//read

    } // CswNbtMetaDataForSpreadSheetColReader


} // namespace ChemSW.Nbt
