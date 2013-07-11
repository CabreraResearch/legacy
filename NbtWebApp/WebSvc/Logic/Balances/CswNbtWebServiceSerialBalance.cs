using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Logic;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Serial Balance Return Object
    /// </summary>
    [DataContract]
    public class CswNbtBalanceReturn : CswWebSvcReturn
    {
        [DataMember] 
        public CswNbtBalanceList Data;

        public CswNbtBalanceReturn()
        {
            Data = new CswNbtBalanceList();
        }
    }

    [DataContract]
    public class CswNbtBalanceList
    {
        [DataMember] 
        public Collection<SerialBalance> BalanceList;
    }




    public class CswNbtWebServiceSerialBalance
    {

        public static void UpdateBalanceData( ICswResources CswResources, CswNbtBalanceReturn Return, SerialBalance Request )
        {
            
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtMetaDataObjectClass BalanceOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass );
            if( null != BalanceOC )
            {
                    CswNbtMetaDataObjectClassProp BalanceNameOCP = BalanceOC.getObjectClassProp( CswNbtObjClassBalance.PropertyName.Name );

                    CswNbtView ExistingBalancesView = new CswNbtView( NbtResources );
                    ExistingBalancesView.ViewName = "Existing Balances";
                    CswNbtViewRelationship BalanceRel = ExistingBalancesView.AddViewRelationship( BalanceOC, false );
                    ExistingBalancesView.AddViewPropertyAndFilter( BalanceRel, BalanceNameOCP,
                                                                   Value: Request.NbtName,
                                                                   FilterMode: CswEnumNbtFilterMode.Equals );
                    ICswNbtTree ExistingBalancesTree = NbtResources.Trees.getTreeFromView( ExistingBalancesView, false, true, true );

                    CswNbtObjClassBalance Balance;

                    if( ExistingBalancesTree.getChildNodeCount() == 0 )
                    {
                        //there is no balance with this name yet. Make a new one.
                        CswNbtMetaDataNodeType BalanceNT = BalanceOC.FirstNodeType;
                        Balance = NbtResources.Nodes.makeNodeFromNodeTypeId( BalanceNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                        Balance.Name.Text = Request.NbtName;
                    }
                    else
                    {
                        //this balance already exists, grab a reference to it.
                        ExistingBalancesTree.goToNthChild( 0 );
                        Balance = ExistingBalancesTree.getCurrentNode();
                    }

                    Balance.Quantity.Quantity = Request.CurrentWeight;
                    Balance.LastActive.DateTimeValue = DateTime.Now;

                    CswNbtObjClassUnitOfMeasure Unit = _mapUnitToNode( NbtResources, Request.UnitOfMeasurement );

                    if( null != Unit )
                    {
                        Balance.Quantity.UnitId = Unit.NodeId;
                    }

                Balance.postChanges( false );

            }//if ( null != BalanceOC )

        }//UpdateBalanceData()


        public static void listConnectedBalances( ICswResources CswResources, CswNbtBalanceReturn Return, object Request )
        {

            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Data.BalanceList = new Collection<SerialBalance>();


            CswNbtMetaDataObjectClass BalanceOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass );
            if( null != BalanceOC )
            {
                    
                    CswNbtView ExistingBalancesView = new CswNbtView( NbtResources );
                    ExistingBalancesView.ViewName = "Connected Balances";
                    CswNbtViewRelationship BalanceRel = ExistingBalancesView.AddViewRelationship( BalanceOC, true );
                    //only add to the list of returned balances if it has had an announcement in the last 11 minutes
                       ExistingBalancesView.AddViewPropertyAndFilter( BalanceRel, BalanceOC.getObjectClassProp( CswNbtObjClassBalance.PropertyName.LastActive ),
                                               SubFieldName: CswEnumNbtSubFieldName.Value,
                                               Value : DateTime.Now.Subtract( TimeSpan.FromMinutes( 11 ) ).ToString(),
                                               FilterMode : CswEnumNbtFilterMode.GreaterThan );

                    ICswNbtTree BalanceTree = NbtResources.Trees.getTreeFromView( ExistingBalancesView, true, false, false );
                    int BalanceCount = BalanceTree.getChildNodeCount();
                    if( BalanceCount > 0 )
                    {
                        BalanceTree.goToRoot();
                        for( int i = 0; i < BalanceCount; i++ )
                        {
                            BalanceTree.goToNthChild( i );
                            CswNbtObjClassBalance Balance = BalanceTree.getCurrentNode();
                            
                            Return.Data.BalanceList.Add( new SerialBalance
                                {
                                    NbtName = Balance.Name.Text,
                                    CurrentWeight = Balance.Quantity.Quantity,
                                    UnitOfMeasurement = Balance.Quantity.CachedUnitName,
                                } );

                        BalanceTree.goToParentNode();

                        } //for ( int i = 0; i < BalanceCount; i++ )

                    } //if( BalanceCount > 0 )

            } //if ( null != BalanceOC )

        }//listConnectedBalances( ICswResources CswResources, CswNbtBalanceReturn Return, object Request )



        public static void getBalanceInformation( ICswResources CswResources, CswNbtBalanceReturn Return, string Request )
        {

            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Data.BalanceList = new Collection<SerialBalance>();
            CswPrimaryKey BalanceKey = new CswPrimaryKey();
            BalanceKey.FromString( Request );


            CswNbtObjClassBalance Balance = NbtResources.getNode( BalanceKey, DateTime.Now );
            
            Return.Data.BalanceList.Add( new SerialBalance
            {
                NbtName = Balance.Name.Text,
                CurrentWeight = Balance.Quantity.Quantity,
                UnitOfMeasurement = Balance.Quantity.CachedUnitName,
            } );


        }//listConnectedBalances( ICswResources CswResources, CswNbtBalanceReturn Return, object Request )




        private static
            CswNbtObjClassUnitOfMeasure _mapUnitToNode( CswNbtResources NbtResources, string UnitName )
        {
            CswNbtObjClassUnitOfMeasure Unit = null;

            CswNbtMetaDataObjectClass UnitsOfMeasureOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtView MatchingUOMsView = new CswNbtView( NbtResources );
            CswNbtViewRelationship ParentRelationship = MatchingUOMsView.AddViewRelationship( UnitsOfMeasureOC, false );

            CswNbtMetaDataObjectClassProp NameOCP = UnitsOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
            MatchingUOMsView.AddViewPropertyAndFilter( ParentRelationship,
                                                       MetaDataProp : NameOCP,
                                                       Value : UnitName,
                                                       SubFieldName : CswEnumNbtSubFieldName.Text,
                                                       FilterMode : CswEnumNbtFilterMode.Equals );

            CswNbtMetaDataObjectClassProp AliasesOCP = UnitsOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Aliases );
            MatchingUOMsView.AddViewPropertyAndFilter( ParentRelationship,
                                                       MetaDataProp : AliasesOCP,
                                                       Value : UnitName,
                                                       SubFieldName : CswEnumNbtSubFieldName.Text,
                                                       FilterMode : CswEnumNbtFilterMode.Contains,
                                                       Conjunction : CswEnumNbtFilterConjunction.Or );

            // Get and iterate the Tree
            ICswNbtTree MatchingUOMsTree = NbtResources.Trees.getTreeFromView( MatchingUOMsView, false, false, true );

            int Count = MatchingUOMsTree.getChildNodeCount();

            for( int i = 0; i < Count; i++ )
            {
                MatchingUOMsTree.goToNthChild( i );
                Unit = MatchingUOMsTree.getNodeForCurrentPosition();
                MatchingUOMsTree.goToParentNode();
            }

            return Unit;

        }//_mapUnitToNode( CswNbtResources NbtResources, string UnitName )


    }//class CswNbtWebServiceSerialBalance

}//namespace