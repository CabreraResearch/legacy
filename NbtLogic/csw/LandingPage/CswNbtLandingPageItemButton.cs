﻿using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemButton : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemButton( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemDataForUI( DataRow LandingPageRow, LandingPageData.Request Request )
        {
            Int32 ObjectClassPropId = CswConvert.ToInt32( LandingPageRow["to_objectclasspropid"] );
            if( ObjectClassPropId != Int32.MinValue )
            {
                CswNbtNode RequestNode = _CswNbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( Request.NodeId ) );
                if( null != RequestNode )
                {
                    CswNbtMetaDataNodeType NodeType = RequestNode.getNodeType();
                    if ( null != NodeType )
                    {
                        String OCPName = _CswNbtResources.MetaData.getObjectClassPropName( ObjectClassPropId );
                        CswNbtMetaDataNodeTypeProp NodeTypeProp = NodeType.getNodeTypePropByObjectClassProp( OCPName );
                        if( null != NodeTypeProp && false == RequestNode.Properties[NodeTypeProp].AsButton.Hidden )
                        {                            
                            String DisplayText = LandingPageRow["displaytext"].ToString();
                            _ItemData.Text = false == String.IsNullOrEmpty( DisplayText ) ? DisplayText : "Add New " + NodeTypeProp.PropName;
                            _ItemData.NodeTypePropId = RequestNode.NodeId.ToString() + "_" + NodeTypeProp.PropId.ToString();
                            _ItemData.ActionName = NodeTypeProp.PropName;
                            _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                            _setCommonItemDataForUI(LandingPageRow);
                        }
                    }
                }
            }
        }

        public override void setDBValuesFromRequest( LandingPageData.Request Request )
        {
            Int32 ObjectClassPropId = CswConvert.ToInt32( Request.PkValue );
            if( ObjectClassPropId != Int32.MinValue )
            {
                _ItemRow["to_objectclasspropid"] = CswConvert.ToDbVal( ObjectClassPropId );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You must select a valid button type", "No button selected for new Button LandingPage Item" );
            }
            _setCommonDbValuesFromRequest( Request );
        }


        public override void setDBValuesFromExistingLandingPageItem( string RoleId, LandingPageData.LandingPageItem Item )
        {
            //really circituous route of getting the OCP, but we are not exposing the OCP any more direct way
            _ItemRow["to_objectclasspropid"] = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( Item.NodeTypePropId.Split( '_' )[1] ) ).ObjectClassPropId;
            _setCommonDBValuesFromExistingLandingPageItem( RoleId, Item );
        }
    }
}
