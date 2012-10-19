using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemAdd : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemAdd( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemData( DataRow LandingPageRow )
        {
            if( CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] ) != Int32.MinValue )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] ) );
                if( NodeType != null )
                {
                    bool CanAdd = NodeType.getObjectClass().CanAdd && _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType );
                    if( CanAdd )
                    {
                        if( LandingPageRow["displaytext"].ToString() != string.Empty )
                            _ItemData.Text = LandingPageRow["displaytext"].ToString();
                        else
                            _ItemData.Text = "Add New " + NodeType.NodeTypeName;
                        _ItemData.NodeTypeId = NodeType.NodeTypeId.ToString();
                        _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                        _ItemData.Type = "add_new_nodetype";
                    }
                }
            }
            _setCommonItemData( LandingPageRow );
        }
    }
}
