﻿using System;
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
            CswNbtObjClassLocation LocationNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( LocationType ), CswEnumNbtMakeNodeOperation.DoNothing );
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
            LocationNode.postChanges( true );
            _finalize();

            return LocationNode.Node;
        }

        internal CswNbtNode createContainerLocationNode( CswNbtNode ContainerNode = null, String Action = "", DateTime? NullableScanDate = null, CswPrimaryKey LocationId = null, String ContainerScan = "", String Type = "Receipt" )
        {
            CswNbtObjClassContainerLocation ContainerLocationNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Container Location" ), CswEnumNbtMakeNodeOperation.DoNothing );
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
            ContainerLocationNode.postChanges( true );
            _finalize();

            return ContainerLocationNode.Node;
        }

        internal CswNbtNode createContainerNode( string NodeTypeName = "Container", double Quantity = 1.0, CswNbtNode UnitOfMeasure = null, CswNbtNode Material = null, CswPrimaryKey LocationId = null )
        {
            CswNbtObjClassContainer ContainerNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswEnumNbtMakeNodeOperation.DoNothing );
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
            ContainerNode.postChanges( true );
            _finalize();

            return ContainerNode.Node;
        }

        internal CswNbtNode createUnitOfMeasureNode( string NodeTypeName, string Name, double ConversionFactorBase, int ConversionFactorExponent, CswEnumTristate Fractional )
        {
            CswNbtObjClassUnitOfMeasure UnitOfMeasureNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Unit (" + NodeTypeName + ")" ), CswEnumNbtMakeNodeOperation.DoNothing );
            UnitOfMeasureNode.Name.Text = Name + "Test";
            if( CswTools.IsDouble( ConversionFactorBase ) )
                UnitOfMeasureNode.ConversionFactor.Base = ConversionFactorBase;
            if( ConversionFactorExponent != Int32.MinValue )
                UnitOfMeasureNode.ConversionFactor.Exponent = ConversionFactorExponent;
            UnitOfMeasureNode.Fractional.Checked = Fractional;
            UnitOfMeasureNode.UnitType.Value = NodeTypeName;
            UnitOfMeasureNode.postChanges( true );
            _finalize();

            return UnitOfMeasureNode.Node;
        }

        internal CswNbtNode createMaterialNode( string NodeTypeName = "Chemical", string State = "Liquid", double SpecificGravity = 1.0, 
            string PPE = "", string Hazards = "", string SpecialFlags = "", string CASNo = "12-34-0", CswEnumTristate IsTierII = CswEnumTristate.True )
        {
            CswNbtObjClassChemical MaterialNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswEnumNbtMakeNodeOperation.DoNothing );
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
            }
            MaterialNode.postChanges( true );
            _finalize();

            return MaterialNode.Node;
        }

        internal CswNbtNode createControlZoneNode( string Name = "CZ_Test", string FireClassSetName = "Default" )
        {
            CswNbtNode ControlZoneNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Control Zone" ), CswEnumNbtMakeNodeOperation.DoNothing );
            CswNbtMetaDataNodeTypeProp NameNTP = _CswNbtResources.MetaData.getNodeTypeProp( ControlZoneNode.NodeTypeId, "Name" );
            ControlZoneNode.Properties[NameNTP].AsText.Text = Name;
            CswNbtMetaDataObjectClass FCEASOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountSetClass );
            foreach ( CswNbtObjClassFireClassExemptAmountSet DefaultFireClassSet in FCEASOC.getNodes( false, false ) )
            {
                if( DefaultFireClassSet.SetName.Text == FireClassSetName )
                {
                    CswNbtMetaDataNodeTypeProp FCSNNTP = _CswNbtResources.MetaData.getNodeTypeProp(ControlZoneNode.NodeTypeId, "Fire Class Set Name");
                    ControlZoneNode.Properties[FCSNNTP].AsRelationship.RelatedNodeId = DefaultFireClassSet.NodeId;
                    break;
                }
            }
            ControlZoneNode.postChanges( true );
            _finalize();

            return ControlZoneNode;
        }

        internal CswNbtNode createUserNode( string Username = "testuser", string Password = "Chemsw123!", CswEnumTristate isLocked = CswEnumTristate.False, CswEnumTristate isArchived = CswEnumTristate.False )
        {
            CswNbtMetaDataObjectClass RoleOc = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            CswPrimaryKey RoleId = RoleOc.getNodeIdAndNames( false, false ).Select( RoleIds => RoleIds.Key ).FirstOrDefault();

            CswNbtObjClassUser NewUser = CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "User" ), CswEnumNbtMakeNodeOperation.WriteNode, OverrideUniqueValidation: true );
            NewUser.UsernameProperty.Text = Username;
            NewUser.Role.RelatedNodeId = RoleId;
            NewUser.PasswordProperty.Password = Password;
            NewUser.AccountLocked.Checked = isLocked;
            NewUser.Archived.Checked = isArchived;
            NewUser.postChanges( ForceUpdate: false );
            _finalize();

            return NewUser.Node;
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

        private void _setMultiListValue(CswNbtNode Node, String MultiListValue, String MultiListPropName)
        {
            CswCommaDelimitedString MultiListString = new CswCommaDelimitedString();
            MultiListString.FromString( MultiListValue );
            CswNbtMetaDataNodeTypeProp MultiListNTP = _CswNbtResources.MetaData.getNodeTypeProp( Node.NodeTypeId, MultiListPropName );
            Node.Properties[MultiListNTP].AsMultiList.Value = MultiListString;
        }

        #endregion

    }
}
