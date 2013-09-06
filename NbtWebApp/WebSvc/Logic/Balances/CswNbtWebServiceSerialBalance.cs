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
        public CswNbtBalanceData Data;

        public CswNbtBalanceReturn()
        {
            Data = new CswNbtBalanceData();
        }
    }

    [DataContract]
    public class CswNbtBalanceData
    {
        [DataMember]
        public Collection<SerialBalance> BalanceList;

        [DataMember]
        public Collection<BalanceConfiguration> ConfigurationList;
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

                CswNbtNodeCollection.AfterMakeNode AfterBalance = delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassBalance thisBalance = NewNode;
                        thisBalance.Name.Text = Request.NbtName;
                        thisBalance.Quantity.Quantity = Request.CurrentWeight;
                        thisBalance.LastActive.DateTimeValue = DateTime.Now;
                        thisBalance.Device.Text = Request.DeviceDescription;
                        thisBalance.Manufacturer.Text = Request.Manufacturer;
                        thisBalance.Operational.Checked = CswConvert.ToTristate( Request.Operational );
                        thisBalance.BalanceConfiguration.RelatedNodeId = _findConfigurationWithName( NbtResources, Request.Configuration ).NodeId;

                        CswNbtObjClassUnitOfMeasure Unit = _mapUnitToNode( NbtResources, Request.UnitOfMeasurement );

                        if( null != Unit )
                        {
                            thisBalance.Quantity.UnitId = Unit.NodeId;
                        }
                    };

                if( ExistingBalancesTree.getChildNodeCount() == 0 )
                {
                    //there is no balance with this name yet. Make a new one.
                    CswNbtMetaDataNodeType BalanceNT = BalanceOC.FirstNodeType;
                    Balance = NbtResources.Nodes.makeNodeFromNodeTypeId( BalanceNT.NodeTypeId, AfterBalance );
                }
                else
                {
                    //this balance already exists, grab a reference to it.
                    ExistingBalancesTree.goToNthChild( 0 );
                    Balance = ExistingBalancesTree.getCurrentNode();
                    AfterBalance(Balance.Node);
                    Balance.postChanges( false );
                }

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
                                        Value: DateTime.Now.Subtract( TimeSpan.FromMinutes( 11 ) ).ToString(),
                                        FilterMode: CswEnumNbtFilterMode.GreaterThan );

                //only add to the list of returned balances if it is Operational
                ExistingBalancesView.AddViewPropertyAndFilter( BalanceRel, BalanceOC.getObjectClassProp( CswNbtObjClassBalance.PropertyName.Operational ),
                                        SubFieldName: CswEnumNbtSubFieldName.Checked,
                                        Value: CswEnumTristate.True,
                                        FilterMode: CswEnumNbtFilterMode.Equals );

                ExistingBalancesView.AddViewPropertyAndFilter( BalanceRel, BalanceOC.getObjectClassProp( CswNbtObjClassBalance.PropertyName.Quantity ),
                                        SubFieldName: CswEnumNbtSubFieldName.Value,
                                        Value: "0",
                                        FilterMode: CswEnumNbtFilterMode.NotEquals );

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
                            NodeId = Balance.NodeId.ToString(),
                            IsActive = ( Balance.LastActive.DateTimeValue + TimeSpan.FromMinutes( 10 ) > DateTime.Now ),
                        } );

                        BalanceTree.goToParentNode();

                    } //for ( int i = 0; i < BalanceCount; i++ )
                } //if( BalanceCount > 0 )

            } //if ( null != BalanceOC )
        }//listConnectedBalances( ICswResources CswResources, CswNbtBalanceReturn Return, object Request )




        public static void listBalanceConfigurations( ICswResources CswResources, CswNbtBalanceReturn Return, object Request )
        {

            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Data.ConfigurationList = new Collection<BalanceConfiguration>();



            CswNbtMetaDataObjectClass BalanceConfigurationOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceConfigurationClass );
            if( null != BalanceConfigurationOC )
            {

                CswNbtView ExistingConfigurationsView = BalanceConfigurationOC.CreateDefaultView( true );

                ICswNbtTree ConfigTree = NbtResources.Trees.getTreeFromView( ExistingConfigurationsView, true, false, false );
                int ConfigCount = ConfigTree.getChildNodeCount();
                if( ConfigCount > 0 )
                {
                    ConfigTree.goToRoot();
                    for( int i = 0; i < ConfigCount; i++ )
                    {
                        ConfigTree.goToNthChild( i );
                        CswNbtObjClassBalanceConfiguration Configuration = ConfigTree.getCurrentNode();

                        Return.Data.ConfigurationList.Add( new BalanceConfiguration
                        {
                            Name = Configuration.Name.Text,
                            RequestFormat = Configuration.RequestFormat.Text,
                            ResponseFormat = Configuration.ResponseFormat.Text,
                            BaudRate = (int) Configuration.BaudRate.Value,
                            ParityBit = Configuration.ParityBit.Text,
                            DataBits = (int) Configuration.DataBits.Value,
                            StopBits = Configuration.StopBits.Text,
                            Handshake = Configuration.Handshake.Text,

                        } );

                        ConfigTree.goToParentNode();

                    } //for ( int i = 0; i < BalanceCount; i++ )

                } //if( BalanceCount > 0 )

            } //if ( null != BalanceOC )


        }//listBalanceConfigurations()



        private static CswNbtObjClassBalanceConfiguration _findConfigurationWithName( CswNbtResources NbtResources, string Name )
        {
            CswNbtObjClassBalanceConfiguration Ret = null;

            CswNbtMetaDataObjectClass BalanceConfigurationOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceConfigurationClass );
            CswNbtMetaDataObjectClassProp ConfigNameOCP = BalanceConfigurationOC.getObjectClassProp( CswNbtObjClassBalanceConfiguration.PropertyName.Name );

            CswNbtView ExistingConfigsView = new CswNbtView( NbtResources );
            ExistingConfigsView.ViewName = "Balance Configurations";
            CswNbtViewRelationship ConfigNameRel = ExistingConfigsView.AddViewRelationship( BalanceConfigurationOC, false );
            ExistingConfigsView.AddViewPropertyAndFilter( ConfigNameRel, ConfigNameOCP,
                                                           Value: Name,
                                                           FilterMode: CswEnumNbtFilterMode.Equals );
            ICswNbtTree ExistingConfigsTree = NbtResources.Trees.getTreeFromView( ExistingConfigsView, false, true, true );


            if( ExistingConfigsTree.getChildNodeCount() > 0 )
            {
                //this configuration exists, grab a reference to it.
                ExistingConfigsTree.goToNthChild( 0 );
                Ret = ExistingConfigsTree.getCurrentNode();
            }

            return Ret;
        }



        public static void registerBalanceConfiguration( ICswResources CswResources, CswNbtBalanceReturn Return, BalanceConfiguration Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtMetaDataObjectClass BalanceConfigurationOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceConfigurationClass );
            if( null != BalanceConfigurationOC )
            {
                Return.Data.ConfigurationList = new Collection<BalanceConfiguration>();

                CswNbtObjClassBalanceConfiguration BalanceConfiguration = _findConfigurationWithName( NbtResources, Request.Name );

                CswNbtNodeCollection.AfterMakeNode AfterBalanceConfig = delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassBalanceConfiguration thisBalanceConfig = NewNode;
                        thisBalanceConfig.Name.Text = Request.Name;
                        thisBalanceConfig.RequestFormat.Text = Request.RequestFormat;
                        thisBalanceConfig.ResponseFormat.Text = Request.ResponseFormat;
                        thisBalanceConfig.BaudRate.Value = Request.BaudRate;
                        thisBalanceConfig.ParityBit.Text = Request.ParityBit;
                        thisBalanceConfig.DataBits.Value = Request.DataBits;
                        thisBalanceConfig.StopBits.Text = Request.StopBits;
                        thisBalanceConfig.Handshake.Text = Request.Handshake;
                    };
                

                if( null == BalanceConfiguration )
                {
                    //there is no configuration with this name yet. Make a new one.
                    CswNbtMetaDataNodeType ConfigNT = BalanceConfigurationOC.FirstNodeType;
                    BalanceConfiguration = NbtResources.Nodes.makeNodeFromNodeTypeId( ConfigNT.NodeTypeId, AfterBalanceConfig );
                }
                else
                {
                    AfterBalanceConfig( BalanceConfiguration.Node );
                    BalanceConfiguration.postChanges( false );
                }

                Return.Data.ConfigurationList.Add( Request );

            }//if ( null != BalanceConfigurationOC )

        } // registerBalanceConfiguration()




        public static void getBalanceInformation( ICswResources CswResources, CswNbtBalanceReturn Return, string Request )
        {

            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Data.BalanceList = new Collection<SerialBalance>();
            CswPrimaryKey BalanceKey = new CswPrimaryKey();
            BalanceKey.FromString( Request );


            CswNbtObjClassBalance Balance = NbtResources.Nodes[BalanceKey];

            Return.Data.BalanceList.Add( new SerialBalance
            {
                NbtName = Balance.Name.Text,
                CurrentWeight = Balance.Quantity.Quantity,
                UnitOfMeasurement = Balance.Quantity.CachedUnitName,
            } );


        }//getBalanceInformation( ICswResources CswResources, CswNbtBalanceReturn Return, string Request )



        private static CswNbtObjClassUnitOfMeasure _mapUnitToNode( CswNbtResources NbtResources, string UnitName )
        {
            CswNbtObjClassUnitOfMeasure Unit = null;

            CswNbtMetaDataObjectClass UnitsOfMeasureOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtView MatchingUOMsView = new CswNbtView( NbtResources );
            CswNbtViewRelationship ParentRelationship = MatchingUOMsView.AddViewRelationship( UnitsOfMeasureOC, false );

            CswNbtMetaDataObjectClassProp NameOCP = UnitsOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
            MatchingUOMsView.AddViewPropertyAndFilter( ParentRelationship,
                                                       MetaDataProp: NameOCP,
                                                       Value: UnitName,
                                                       SubFieldName: CswEnumNbtSubFieldName.Text,
                                                       FilterMode: CswEnumNbtFilterMode.Equals );

            CswNbtMetaDataObjectClassProp AliasesOCP = UnitsOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Aliases );
            MatchingUOMsView.AddViewPropertyAndFilter( ParentRelationship,
                                                       MetaDataProp: AliasesOCP,
                                                       Value: UnitName,
                                                       SubFieldName: CswEnumNbtSubFieldName.Text,
                                                       FilterMode: CswEnumNbtFilterMode.Contains,
                                                       Conjunction: CswEnumNbtFilterConjunction.Or );

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