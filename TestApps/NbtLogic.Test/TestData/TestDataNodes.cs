using System;
using System.Collections;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Test
{
    internal class TestDataNodes
    {
        private CswNbtResources _CswNbtResources;
        private int _UniqueSequence;
        internal int Sequence
        {
            get
            {
                _UniqueSequence++;
                return _UniqueSequence;
            }
        }

        internal bool FinalizeNodes = false;

        internal TestDataNodes( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        internal CswNbtResources CswNbtResources
        {
            get { return _CswNbtResources; }
        }

        #region Nodes

        internal CswNbtNode createLocationNode( String LocationType = "Room", String Name = "New Room", CswPrimaryKey ParentLocationId = null, CswPrimaryKey ControlZoneId = null )
        {
            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( LocationType ), delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassLocation LocationNode = NewNode;
                    LocationNode.Name.Text = Name;
                    if( ParentLocationId != null )
                    {
                        LocationNode.Location.SelectedNodeId = ParentLocationId;
                        LocationNode.Location.RefreshNodeName();
                    }
                    if( ControlZoneId != null )
                    {
                        LocationNode.ControlZone.RelatedNodeId = ControlZoneId;
                    }
                    //LocationNode.postChanges( true );
                } );
            _finalize();

            return ret;
        }

        internal CswNbtNode createContainerLocationNode( CswNbtNode ContainerNode = null, String Action = "", DateTime? NullableScanDate = null, CswPrimaryKey LocationId = null, String ContainerScan = "", String Type = "Receipt" )
        {
            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Container Location" ), delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainerLocation ContainerLocationNode = NewNode;
                    if( null == ContainerNode )
                    {
                        ContainerNode = createContainerNode( LocationId: LocationId );
                    }
                    ContainerLocationNode.Container.RelatedNodeId = ContainerNode.NodeId;
                    ContainerLocationNode.Action.Value = Action;
                    ContainerLocationNode.Type.Value = Type;
                    DateTime ScanDate = NullableScanDate ?? DateTime.Now;
                    ContainerLocationNode.ScanDate.DateTimeValue = ScanDate;
                    if( LocationId != null )
                    {
                        ContainerLocationNode.Location.SelectedNodeId = LocationId;
                        ContainerLocationNode.Location.RefreshNodeName();
                    }
                    ContainerLocationNode.ContainerScan.Text = ContainerScan;
                    //ContainerLocationNode.postChanges( true );
                } );
            _finalize();

            return ret;
        }

        internal CswNbtNode createContainerNode( string NodeTypeName = "Container", double Quantity = 1.0, CswNbtNode UnitOfMeasure = null, CswNbtNode Material = null, CswPrimaryKey LocationId = null )
        {
            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainer ContainerNode = NewNode;
                    ContainerNode.Quantity.Quantity = Quantity;
                    if( null == UnitOfMeasure )
                    {
                        UnitOfMeasure = createUnitOfMeasureNode( "Volume", "Liters" + Sequence, 1.0, 0, CswEnumTristate.True );
                    }
                    ContainerNode.Quantity.UnitId = UnitOfMeasure.NodeId;
                    ContainerNode.UseType.Value = CswEnumNbtContainerUseTypes.Storage;
                    ContainerNode.StorageTemperature.Value = CswEnumNbtContainerStorageTemperatures.RoomTemperature;
                    ContainerNode.StoragePressure.Value = CswEnumNbtContainerStoragePressures.Atmospheric;
                    if( Material != null )
                    {
                        ContainerNode.Material.RelatedNodeId = Material.NodeId;
                    }
                    if( LocationId != null )
                    {
                        ContainerNode.Location.SelectedNodeId = LocationId;
                        ContainerNode.Location.RefreshNodeName();
                    }
                    //ContainerNode.postChanges( true );
                } );
            _finalize();

            return ret;
        }

        internal CswNbtNode createUnitOfMeasureNode( string NodeTypeName, string Name, double ConversionFactorBase, int ConversionFactorExponent, CswEnumTristate Fractional )
        {
            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Unit (" + NodeTypeName + ")" ), delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassUnitOfMeasure UnitOfMeasureNode = NewNode;
                    UnitOfMeasureNode.Name.Text = Name + "Test";
                    if( CswTools.IsDouble( ConversionFactorBase ) )
                        UnitOfMeasureNode.ConversionFactor.Base = ConversionFactorBase;
                    if( ConversionFactorExponent != Int32.MinValue )
                        UnitOfMeasureNode.ConversionFactor.Exponent = ConversionFactorExponent;
                    UnitOfMeasureNode.Fractional.Checked = Fractional;
                    UnitOfMeasureNode.UnitType.Value = NodeTypeName;
                    //UnitOfMeasureNode.postChanges( true );
                } );
            _finalize();

            return ret;
        }

        internal CswNbtNode createMaterialNode( string NodeTypeName = "Chemical", string State = "Liquid", double SpecificGravity = 1.0,
            string PPE = "", string Hazards = "", string SpecialFlags = "", string CASNo = "12-34-0", bool IsTierII = true )
        {
            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassChemical MaterialNode = NewNode;
                    if( CswTools.IsDouble( SpecificGravity ) )
                        MaterialNode.SpecificGravity.Value = SpecificGravity;
                    MaterialNode.PhysicalState.Value = State;
            MaterialNode.TradeName.Text = "Fake Chemical " + Sequence;
            MaterialNode.PartNumber.Text = "ABC00" + Sequence;
                    if( NodeTypeName == "Chemical" )
                    {
                        if( false == String.IsNullOrEmpty( PPE ) )
                        {
                            _setMultiListValue( MaterialNode.Node, PPE, "PPE" );
                        }
                        if( false == String.IsNullOrEmpty( Hazards ) )
                        {
                            _setMultiListValue( MaterialNode.Node, Hazards, "Hazard Classes" );
                        }
                        MaterialNode.postChanges( true );
                        _setMultiListValue( MaterialNode.Node, SpecialFlags, "Special Flags" );

                        MaterialNode.CasNo.Text = CASNo;
                        MaterialNode.IsTierII.Checked = CswConvert.ToTristate( IsTierII );
                    }
                    //MaterialNode.postChanges( true );
                } );
            _finalize();

            return ret;
        }

        internal CswNbtNode createControlZoneNode( string Name = "CZ_Test", string FireClassSetName = "Default" )
        {
            CswNbtNode ControlZoneNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Control Zone" ), delegate( CswNbtNode NewNode )
                {
                    CswNbtMetaDataNodeTypeProp NameNTP = _CswNbtResources.MetaData.getNodeTypeProp( NewNode.NodeTypeId, "Name" );
                    NewNode.Properties[NameNTP].AsText.Text = Name;
                    CswNbtMetaDataObjectClass FCEASOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountSetClass );
                    foreach( CswNbtObjClassFireClassExemptAmountSet DefaultFireClassSet in FCEASOC.getNodes( false, false ) )
                    {
                        if( DefaultFireClassSet.SetName.Text == FireClassSetName )
                        {
                            CswNbtMetaDataNodeTypeProp FCSNNTP = _CswNbtResources.MetaData.getNodeTypeProp( NewNode.NodeTypeId, "Fire Class Set Name" );
                            NewNode.Properties[FCSNNTP].AsRelationship.RelatedNodeId = DefaultFireClassSet.NodeId;
                            break;
                        }
                    }
                    //ControlZoneNode.postChanges( true );
                } );
            _finalize();

            return ControlZoneNode;
        }

        internal CswNbtNode createUserNode( string Username = "testuser", string Password = "Chemsw123!", bool isLocked = false, bool isArchived = false )
        {
            CswNbtMetaDataObjectClass RoleOc = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            CswPrimaryKey RoleId = RoleOc.getNodeIdAndNames( false, false ).Select( RoleIds => RoleIds.Key ).FirstOrDefault();

            CswNbtNode ret = CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "User" ), OverrideUniqueValidation: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassUser NewUser = NewNode;
                    NewUser.UsernameProperty.Text = Username;
                    NewUser.Role.RelatedNodeId = RoleId;
                    NewUser.PasswordProperty.Password = Password;
                    NewUser.AccountLocked.Checked = CswConvert.ToTristate( isLocked );
                    NewUser.Archived.Checked = CswConvert.ToTristate( isArchived );
                    //NewUser.postChanges( ForceUpdate: false );
                } );
            _finalize();

            return ret;
        }

        internal CswNbtNode createGeneratorNode( CswEnumRateIntervalType IntervalType, String NodeTypeName = "Equipment Schedule", int WarningDays = 0, SortedList Days = null )
        {
            CswNbtObjClassGenerator GeneratorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswEnumNbtMakeNodeOperation.WriteNode );
            CswRateInterval RateInt = new CswRateInterval( _CswNbtResources );
            if( IntervalType == CswEnumRateIntervalType.WeeklyByDay )
            {
                if( null == Days )
                {
                    Days = new SortedList { { DayOfWeek.Monday, DayOfWeek.Monday } };
                }
                RateInt.setWeeklyByDay( Days, new DateTime( 2012, 1, 1 ) );
            }
            else if( IntervalType == CswEnumRateIntervalType.MonthlyByDate )
            {
                RateInt.setMonthlyByDate( 1, 15, 1, 2012 );
            }
            GeneratorNode.DueDateInterval.RateInterval = RateInt;
            GeneratorNode.WarningDays.Value = WarningDays;
            GeneratorNode.postChanges( ForceUpdate: false );
            _finalize();

            return GeneratorNode.Node;
        }

        #endregion

        #region Private Helper Functions

        private void _finalize()
        {
            if( FinalizeNodes )
            {
                CswNbtResources.finalize();
            }
        }

        private int _getNodeTypeId( string NodeTypeName )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeName );
            if( NodeType == null )
            {
                throw new Exception( "Expected NodeType not found: " + NodeTypeName );
            }
            return NodeType.NodeTypeId;
        }

        private void _setMultiListValue( CswNbtNode Node, String MultiListValue, String MultiListPropName )
        {
            CswCommaDelimitedString MultiListString = new CswCommaDelimitedString();
            MultiListString.FromString( MultiListValue );
            CswNbtMetaDataNodeTypeProp MultiListNTP = _CswNbtResources.MetaData.getNodeTypeProp( Node.NodeTypeId, MultiListPropName );
            Node.Properties[MultiListNTP].AsMultiList.Value = MultiListString;
        }

        #endregion

    }
}
