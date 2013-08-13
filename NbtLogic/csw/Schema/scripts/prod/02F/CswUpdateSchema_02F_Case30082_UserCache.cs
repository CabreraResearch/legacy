using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30082_UserCache : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30082; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach ( CswNbtObjClassUser User in UserOc.getNodes( forceReInit: true, includeSystemNodes: false, IncludeDefaultFilters: false, IncludeHiddenNodes: true ) )
            {
                CswNbtView ReqView = _CswNbtSchemaModTrnsctn.makeView();
                CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
                CswNbtViewRelationship RootVr = ReqView.AddViewRelationship( RequestOc, IncludeDefaultFilters: true );

                CswNbtMetaDataObjectClassProp RequestorOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );
                ReqView.AddViewPropertyAndFilter( RootVr, RequestorOcp, FilterMode: CswEnumNbtFilterMode.Equals, SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: User.NodeId.PrimaryKey.ToString() );

                foreach ( CswEnumNbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
                {
                    CswNbtMetaDataObjectClass MemberOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( Member );
                    CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );
                    CswNbtViewRelationship RequestItemRel = ReqView.AddViewRelationship( RootVr,
                                                                                     CswEnumNbtViewPropOwnerType.Second,
                                                                                     RequestOcp, IncludeDefaultFilters: true );


                }

                ICswNbtTree Tree = _CswNbtSchemaModTrnsctn.getTreeFromView( ReqView, IncludeSystemNodes: false );
                Int32 RequestCount = Tree.getChildNodeCount();
                if ( RequestCount > 0 )
                {
                    CswNbtObjClassUser.Cache UserCache = CswNbtObjClassUser.getCurrentUserCache( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources );
                    
                    for ( Int32 R = 0; R < RequestCount; R += 1 )
                    {
                        Tree.goToNthChild( R );
                        CswNbtObjClassRequest Request = Tree.getNodeForCurrentPosition();
                        if( Request.IsFavorite.Checked == CswEnumTristate.True )
                        {
                            UserCache.CartCounts.FavoriteRequestItems += Tree.getChildNodeCount();
                        }
                        else if( Request.IsRecurring.Checked == CswEnumTristate.True )
                        {
                            UserCache.CartCounts.RecurringRequestItems += Tree.getChildNodeCount();
                        }
                        else
                        {
                            for( Int32 I = 0; I < Tree.getChildNodeCount(); I += 1 )
                            {
                                Tree.goToNthChild( I );
                                CswNbtPropertySetRequestItem RequestItem = Tree.getNodeForCurrentPosition();
                                if( RequestItem.Status.Text == CswNbtPropertySetRequestItem.Statuses.Pending )
                                {
                                    UserCache.CartCounts.PendingRequestItems += 1;
                                }
                                else
                                {
                                    UserCache.CartCounts.SubmittedRequestItems += 1;
                                }
                                Tree.goToParentNode();
                            }
                        }
                        Tree.goToParentNode();
                    }
                    UserCache.update( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources );
                }
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema