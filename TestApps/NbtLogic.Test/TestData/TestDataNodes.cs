using System.Collections;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System;
using System.Linq;

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

        internal CswNbtNode createTempNode()
        {
            CswNbtNode ControlZoneNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Control Zone" ), delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassControlZone ControlZone = NewNode;
                ControlZone.ControlZoneName.Text = "TempNode";
            }, true );
            _finalize();

            return ControlZoneNode;
        }

        internal CswNbtNode createVendorNode( bool IsTemp = false )
        {
            CswNbtObjClassVendor VendorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Vendor" ), delegate( CswNbtNode NewNode )
            {
                CswNbtMetaDataNodeTypeProp NameNTP = _CswNbtResources.MetaData.getNodeTypeProp( NewNode.NodeTypeId, CswNbtObjClassVendor.PropertyName.VendorName );
                NewNode.Properties[NameNTP].AsText.Text = "TempVendor";
            }, IsTemp );
            _finalize();

            return VendorNode.Node;
        }

        internal CswNbtNode createLocationNode( String LocationType = "Room", String Name = "New Room", CswPrimaryKey ParentLocationId = null, CswPrimaryKey ControlZoneId = null, bool AllowInventory = true )
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
                    LocationNode.AllowInventory.Checked = AllowInventory ? CswEnumTristate.True : CswEnumTristate.False;
                }, OverrideUniqueValidation: true );

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
                } );

            _finalize();

            return ret;
        }

        internal CswNbtNode createContainerDispenseTransactionNode( CswNbtObjClassContainer Container, DateTime? DateCreated = null, double? Quantity = null, string Type = null )
        {
            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Container Dispense Transaction" ), delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = NewNode;
                ContDispTransNode.DestinationContainer.RelatedNodeId = Container.NodeId;
                ContDispTransNode.QuantityDispensed.Quantity = Quantity ?? Container.Quantity.Quantity;
                ContDispTransNode.QuantityDispensed.UnitId = Container.Quantity.UnitId;
                ContDispTransNode.Type.Value = Type ?? CswEnumNbtContainerDispenseType.Receive.ToString();
                ContDispTransNode.DispensedDate.DateTimeValue = DateCreated ?? Container.DateCreated.DateTimeValue;
            } );

            _finalize();

            return ret;
        }

        internal CswNbtNode createContainerNode( string NodeTypeName = "Container", double Quantity = 1.0, CswNbtNode UnitOfMeasure = null, CswNbtNode Material = null, CswPrimaryKey LocationId = null, string UseType = "", bool Missing = false )
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
                    ContainerNode.UseType.Value = UseType ?? CswEnumNbtContainerUseTypes.Storage;
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
                    if( Missing )
                    {
                        ContainerNode.Missing.Checked = CswEnumTristate.True;
                    }
                    //ContainerNode.MaterialObsolete..CachedValue = "";
                } );
            _finalize();

            return ret;
        }

        internal CswNbtNode createContainerWithRecords( string NodeTypeName = "Container", double Quantity = 1.0, CswNbtNode UnitOfMeasure = null, CswNbtNode Material = null, CswPrimaryKey LocationId = null, DateTime? DateCreated = null, string UseType = null )
        {
            CswNbtNode ret = createContainerNode( NodeTypeName, Quantity, UnitOfMeasure, Material, LocationId, UseType );
            createContainerDispenseTransactionNode( ret, DateCreated );
            createContainerLocationNode( ret, LocationId: LocationId, NullableScanDate: DateCreated );
            return ret;
        }

        internal CswNbtNode createUnitOfMeasureNode( string NodeTypeName, string Name, double ConversionFactorBase, int ConversionFactorExponent, CswEnumTristate Fractional )
        {
            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Unit_" + NodeTypeName ), delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassUnitOfMeasure UnitOfMeasureNode = NewNode;
                    UnitOfMeasureNode.Name.Text = Name + "Test";
                    if( CswTools.IsDouble( ConversionFactorBase ) )
                        UnitOfMeasureNode.ConversionFactor.Base = ConversionFactorBase;
                    if( ConversionFactorExponent != Int32.MinValue )
                        UnitOfMeasureNode.ConversionFactor.Exponent = ConversionFactorExponent;
                    UnitOfMeasureNode.Fractional.Checked = Fractional;
                    UnitOfMeasureNode.UnitType.Value = NodeTypeName;
                } );
            _finalize();

            return ret;
        }

        internal CswNbtNode createMaterialNode( string NodeTypeName = "Chemical", string State = "Liquid", double SpecificGravity = 1.0,
            string PPE = "", string Hazards = "", string SpecialFlags = "", string CASNo = "12-34-0", CswEnumTristate IsTierII = null, 
            Collection<CswNbtNode> Constituents = null, int ConstPercentage = 10 )
        {
            IsTierII = IsTierII ?? CswEnumTristate.True;

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
                        MaterialNode.IsTierII.Checked = IsTierII;
                        if( null != Constituents )
                        {
                            foreach( CswNbtNode Constituent in Constituents )
                            {
                                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Material Component" ), delegate( CswNbtNode Node )
                                    {
                                        CswNbtObjClassMaterialComponent MaterialComponentNode = Node;
                                        MaterialComponentNode.Mixture.RelatedNodeId = MaterialNode.NodeId;
                                        MaterialComponentNode.Constituent.RelatedNodeId = Constituent.NodeId;
                                        MaterialComponentNode.LowPercentageValue.Value = ConstPercentage;
                                        MaterialComponentNode.TargetPercentageValue.Value = ConstPercentage;
                                        MaterialComponentNode.HighPercentageValue.Value = ConstPercentage;
                                        MaterialComponentNode.Percentage.Value = ConstPercentage;
                                    } );
                            }
                        }
                    }
                }, OverrideUniqueValidation: true );
            _finalize();

            return ret;
        }

        internal CswNbtNode createConstituentNode( string NodeTypeName = "Chemical", string State = "Liquid", double SpecificGravity = 1.0,
            string CASNo = "12-34-0", CswEnumTristate IsTierII = null )
        {
            IsTierII = IsTierII ?? CswEnumTristate.True;

            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Constituent" ), delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassChemical MaterialNode = NewNode;
                if( CswTools.IsDouble( SpecificGravity ) )
                    MaterialNode.SpecificGravity.Value = SpecificGravity;
                MaterialNode.PhysicalState.Value = State;
                MaterialNode.TradeName.Text = "Fake Constituent " + Sequence;
                MaterialNode.PartNumber.Text = "ABC00" + Sequence;
                MaterialNode.CasNo.Text = CASNo;
                MaterialNode.IsTierII.Checked = IsTierII;
            }, OverrideUniqueValidation: true );
            _finalize();

            return ret;
        }

        internal CswNbtNode createControlZoneNode( string Name = "CZ_Test", string FireClassSetName = "Default" )
        {
            CswNbtNode ControlZoneNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Control Zone" ), delegate( CswNbtNode NewNode )
               {
                CswNbtObjClassControlZone ControlZone = NewNode;
                ControlZone.ControlZoneName.Text = Name;
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
               } );
            _finalize();

            return ControlZoneNode;
        }

        internal CswNbtNode createUserNode( string Username = "testuser", string Password = "Chemsw123!", CswEnumTristate isLocked = null, CswEnumTristate isArchived = null )
        {
            isLocked = isLocked ?? CswEnumTristate.False;
            isArchived = isArchived ?? CswEnumTristate.False;

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

        internal CswNbtNode createGeneratorNode( CswEnumRateIntervalType IntervalType, String NodeTypeName = "Equipment Schedule", int WarningDays = 0, SortedList Days = null, DateTime? StartDate = null )
        {
            CswNbtNode ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassGenerator GeneratorNode = NewNode;
                    CswRateInterval RateInt = new CswRateInterval( _CswNbtResources );
                    DateTime StDate = StartDate != null ? (DateTime) StartDate : new DateTime( 2012, 1, 15 );
                    if( IntervalType == CswEnumRateIntervalType.WeeklyByDay )
                    {
                        if( null == Days )
                        {
                            Days = new SortedList { { DayOfWeek.Monday, DayOfWeek.Monday } };
                        }
                        RateInt.setWeeklyByDay( Days, StDate );
                    }
                    else if( IntervalType == CswEnumRateIntervalType.MonthlyByDate )
                    {
                        RateInt.setMonthlyByDate( 1, StDate.Day, StDate.Month, StDate.Year );
                    }
                    GeneratorNode.DueDateInterval.RateInterval = RateInt;
                    GeneratorNode.WarningDays.Value = WarningDays;
                    //GeneratorNode.postChanges( ForceUpdate: false );
                } );

            _finalize();

            return ret;
        } // createGeneratorNode()

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
