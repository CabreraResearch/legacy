using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;
using ChemSW.Session;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceUpdateProperties
    {
        private CswNbtWebServiceResources _CswNbtWebServiceResources;
        public CswNbtWebServiceUpdateProperties( CswNbtWebServiceResources CswNbtWebServiceResources )
        {
            _CswNbtWebServiceResources = CswNbtWebServiceResources;
        }

        public string Run( string Updates )
        {
            string UpdatedRowIds = string.Empty;
            string[] UpdateItems = Updates.Split( ';' );
            foreach( string CurrentUpdateItem in UpdateItems )
            {

                string[] ItemBreakdown = CurrentUpdateItem.Split( ',' );
                string ClientRowId = ItemBreakdown[0];
                string PropId = ItemBreakdown[1];
                string Value = ItemBreakdown[2];

                string[] ItemId = PropId.Split( '_' );
                Int32 NodeTypePropId = Convert.ToInt32( ItemId[1] );
                Int32 NodeId = Convert.ToInt32( ItemId[4] );

                CswPrimaryKey CswPrimaryKey = new CswPrimaryKey();
                CswPrimaryKey.FromString( ItemId[3] + "_" + NodeId );

                CswNbtNode CswNbtNode = _CswNbtWebServiceResources.CswNbtResources.Nodes[CswPrimaryKey];
                CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp = _CswNbtWebServiceResources.CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                CswNbtNodePropWrapper CswNbtNodePropWrapper = CswNbtNode.Properties[CswNbtMetaDataNodeTypeProp];



                switch( CswNbtNodePropWrapper.FieldType.FieldType )
                {
                    case CswNbtMetaDataFieldType.NbtFieldType.Text:
                        CswNbtNodePropWrapper.AsText.Text = Value;
                        break;

                    case CswNbtMetaDataFieldType.NbtFieldType.Question:
                        CswNbtNodePropWrapper.AsQuestion.Answer = Value;
                        break;

                    case CswNbtMetaDataFieldType.NbtFieldType.Date:
                        CswNbtNodePropWrapper.AsDate.DateValue = Convert.ToDateTime( Value );
                        break;

                    case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                        CswNbtNodePropWrapper.AsMemo.Text = Value;
                        break;

                    default:
                        throw ( new CswDniException( "Unhandled field type " + CswNbtNodePropWrapper.FieldType.FieldType.ToString() ) );

                }//switch on field type 

                CswNbtNode.postChanges( false );

                if( UpdatedRowIds.Length > 0 )
                {
                    UpdatedRowIds += ",";
                }

                UpdatedRowIds += ClientRowId;


            }//iterate update items

            return ( UpdatedRowIds );
        } // Run()

    } // class CswNbtWebServiceUpdateProperties

} // namespace ChemSW.WebServices

