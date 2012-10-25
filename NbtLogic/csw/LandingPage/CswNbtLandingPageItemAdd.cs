using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemAdd : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemAdd( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemDataForUI( DataRow LandingPageRow, LandingPageData.Request Request )
        {
            Int32 NodeTypeId = CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] );
            if( NodeTypeId != Int32.MinValue )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( NodeType != null )
                {
                    bool CanAdd = NodeType.getObjectClass().CanAdd && _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType );
                    if( CanAdd )
                    {
                        String DisplayText = LandingPageRow["displaytext"].ToString();
                        _ItemData.Text = false == String.IsNullOrEmpty( DisplayText ) ? DisplayText : "Add New " + NodeType.NodeTypeName;
                        _ItemData.NodeTypeId = NodeType.NodeTypeId.ToString();
                        _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                        _ItemData.Type = "add_new_nodetype";
                    }
                    _setCommonItemDataForUI( LandingPageRow );
                }
            }
        }

        public override void setItemDataForDB( LandingPageData.Request Request )
        {
            Int32 NodeTypeId = CswConvert.ToInt32( Request.NodeTypeId );
            if( NodeTypeId != Int32.MinValue )
            {
                _ItemRow["to_nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
            }
            else
            {
                throw new CswDniException(ErrorType.Warning, "You must select something to add", "No nodetype selected for new Add LandingPage Item");
            }
            _setCommonItemDataForDB( Request );
        }
    }
}
