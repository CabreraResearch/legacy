using System;
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
            if( CswConvert.ToInt32( LandingPageRow["to_objectclasspropid"] ) != Int32.MinValue )
            {
                CswNbtNode RequestNode = _CswNbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( Request.NodeId ) );
                if( null != RequestNode )
                {
                    CswNbtMetaDataNodeType NodeType = RequestNode.getNodeType();
                    if ( null != NodeType )
                    {
                        CswNbtMetaDataNodeTypeProp NodeTypeProp = NodeType.getNodeTypePropByObjectClassProp( CswConvert.ToInt32(LandingPageRow["to_objectclasspropid"]));
                        if ( null != NodeTypeProp )
                        {                            
                            _ItemData.Text = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : NodeTypeProp.PropName;
                            _ItemData.NodeTypePropId = RequestNode.NodeId.ToString() + "_" + NodeTypeProp.PropId.ToString();
                            _ItemData.ActionName = NodeTypeProp.PropName;
                            _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                            _setCommonItemDataForUI(LandingPageRow);
                        }
                    }
                }
            }
        }

        public override void setItemDataForDB( LandingPageData.Request Request )
        {
            Int32 ObjectClassPropId = CswConvert.ToInt32( Request.PkValue );
            if( ObjectClassPropId != Int32.MinValue )
            {
                _ItemRow["to_objectclasspropid"] = CswConvert.ToDbVal( ObjectClassPropId );
            }
            else
            {
                throw new CswDniException( ErrorType.Warning, "You must select a valid button type", "No button selected for new Button LandingPage Item" );
            }
            _setCommonItemDataForDB( Request );
        }
    }
}
