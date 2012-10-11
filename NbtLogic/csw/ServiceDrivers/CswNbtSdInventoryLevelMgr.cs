using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Mail;
using ChemSW.Nbt.csw.Conversion;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdInventoryLevelMgr
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtSdLocations _SdLocations;

        public CswNbtSdInventoryLevelMgr( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
            _SdLocations = new CswNbtSdLocations( _CswNbtResources );
        }

        #region Validation

        public bool doSendEmail( CswNbtObjClassInventoryLevel InventoryLevel )
        {
            return isLevelPastThreshhold( InventoryLevel ) && InventoryLevel.LastNotified.DateTimeValue <= DateTime.Now.AddDays( -1 );
        }

        public bool isLevelPastThreshhold( CswNbtObjClassInventoryLevel InventoryLevel )
        {
            bool Ret = false;

            switch( InventoryLevel.Type.Value )
            {
                case CswNbtObjClassInventoryLevel.Types.Maximum:
                    Ret = ( InventoryLevel.CurrentQuantity.Quantity > InventoryLevel.Level.Quantity );
                    break;
                case CswNbtObjClassInventoryLevel.Types.Minimum:
                    Ret = ( InventoryLevel.CurrentQuantity.Quantity < InventoryLevel.Level.Quantity );
                    break;
            }

            return Ret;
        }

        #endregion Validation

        #region Email

        private void _sendEmail( string Recipient, string Subject, string Message )
        {
            Collection<CswMailMessage> EmailMessage = _CswNbtResources.makeMailMessages( Subject, Message,
                                                                                        Recipient );
            _CswNbtResources.sendEmailNotification( EmailMessage );
        }

        /// <summary>
        /// Sends notification email and returns Now
        /// </summary>
        /// <returns></returns>
        public DateTime sendPastThreshholdEmail( CswNbtObjClassInventoryLevel InventoryLevel )
        {
            foreach( CswNbtObjClassUser User in InventoryLevel.Subscribe.SelectedUsers() )
            {
                if( false == string.IsNullOrEmpty( User.Email ) &&
                    false == string.IsNullOrEmpty( InventoryLevel.Type.Value ) )
                {
                    string Subject = InventoryLevel.Status.Value + " alert for " + InventoryLevel.Material.CachedNodeName + " at " + InventoryLevel.Location.CachedFullPath;
                    string Message = "The status for Inventory Level: " + InventoryLevel.Node.NodeName + " has changed to " + InventoryLevel.Status.Value + "\n";
                    Message += "Material: " + InventoryLevel.Material.CachedNodeName + "\n";
                    Message += "Location: " + InventoryLevel.Location.CachedFullPath + "\n";
                    Message += "Current Quantity: " + InventoryLevel.CurrentQuantity.Gestalt + "\n";
                    Message += "Threshhold: " + InventoryLevel.Level.Gestalt + "\n";

                    _sendEmail( User.Email, Subject, Message );
                }
            }
            return DateTime.Now;
        }

        #endregion Email

        #region Inventory

        public CswNbtView GetCurrentQuantityView( CswNbtObjClassInventoryLevel InventoryLevel, CswPrimaryKey StartLocationId )
        {
            CswNbtView Ret = null;
            if( CswTools.IsPrimaryKey( InventoryLevel.Material.RelatedNodeId ) )
            {
                Ret = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship LocationRel = _SdLocations.getAllChildrenLocationRelationship( Ret, StartLocationId );

                CswNbtMetaDataObjectClass ContainerOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
                CswNbtMetaDataObjectClassProp LocationOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
                CswNbtMetaDataObjectClassProp MaterialOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataObjectClassProp DisposedOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Disposed );
                CswNbtMetaDataObjectClassProp MissingOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Missing );
                CswNbtMetaDataObjectClassProp QuantityOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );

                CswNbtViewRelationship ContainerRel = Ret.AddViewRelationship( LocationRel, NbtViewPropOwnerType.Second, LocationOcp, false );
                Ret.AddViewPropertyAndFilter( ContainerRel, MaterialOcp, InventoryLevel.Material.RelatedNodeId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                Ret.AddViewPropertyAndFilter( ContainerRel, DisposedOcp, Tristate.True.ToString(), CswNbtSubField.SubFieldName.Checked, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
                Ret.AddViewPropertyAndFilter( ContainerRel, MissingOcp, Tristate.True.ToString(), CswNbtSubField.SubFieldName.Checked, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
                Ret.AddViewProperty( ContainerRel, QuantityOcp );
            }
            return Ret;
        }

        public double getCurrentInventoryLevel( CswNbtObjClassInventoryLevel InventoryLevel )
        {
            double Ret = 0;
            CswNbtView ContainerView = GetCurrentQuantityView( InventoryLevel, InventoryLevel.Location.SelectedNodeId );
            if( null != ContainerView )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( ContainerView, false, false );
                Int32 LocationNodeCount = Tree.getChildNodeCount();
                if( LocationNodeCount > 0 )
                {
                    Dictionary<Int32, CswNbtUnitConversion> UnitConversions = new Dictionary<int, CswNbtUnitConversion>();
                    for( Int32 L = 0; L < LocationNodeCount; L += 1 )
                    {
                        Tree.goToNthChild( L );
                        Int32 ContainerNodeCount = Tree.getChildNodeCount();
                        if( ContainerNodeCount > 0 )
                        {
                            for( Int32 C = 0; C < ContainerNodeCount; C += 1 )
                            {
                                Tree.goToNthChild( C );
                                foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
                                {
                                    CswNbtMetaDataFieldType.NbtFieldType FieldType = CswConvert.ToString( Prop["fieldtype"] );
                                    if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Quantity )
                                    {
                                        Int32 UnitTypeId = CswConvert.ToInt32( Prop["field1_fk"] );
                                        CswNbtUnitConversion Conversion;
                                        if( UnitConversions.ContainsKey( UnitTypeId ) )
                                        {
                                            Conversion = UnitConversions[UnitTypeId];
                                        }
                                        else
                                        {
                                            Conversion = new CswNbtUnitConversion( _CswNbtResources, new CswPrimaryKey( "nodes", UnitTypeId ), InventoryLevel.Level.UnitId, InventoryLevel.Material.RelatedNodeId );
                                            UnitConversions.Add( UnitTypeId, Conversion );
                                        }
                                        if( null != Conversion )
                                        {
                                            double ContainerQuantity = CswConvert.ToDouble( Prop["field1_numeric"] );
                                            Ret += Conversion.convertUnit( ContainerQuantity );
                                        }
                                    }
                                }
                                Tree.goToParentNode();
                            }
                        }
                        Tree.goToParentNode();
                    }
                }

            }
            return Ret;
        }

        private bool _addToCurrentQuantity( CswNbtObjClassInventoryLevel InventoryLevel, double Quantity, CswPrimaryKey UnitId, string Reason )
        {
            bool Ret = false;
            if( null != InventoryLevel )
            {
                CswNbtUnitConversion Conversion = new CswNbtUnitConversion( _CswNbtResources, UnitId, InventoryLevel.Level.UnitId, InventoryLevel.Material.RelatedNodeId );
                InventoryLevel.CurrentQuantity.Quantity += Conversion.convertUnit( Quantity );
                InventoryLevel.CurrentQuantityLog.AddComment( Reason );
                InventoryLevel.postChanges( true );
                Ret = true;
            }
            return Ret;
        }

        private const string _ParentLocationInventoryLevelViewName = "ParentLocationInventoryLevelView";
        private CswNbtView _getParentLocationInventoryLevelView( CswPrimaryKey LocationId, CswPrimaryKey MaterialId = null )
        {
            CswNbtView Ret = null;
            if( null != LocationId )
            {
                Ret = new CswNbtView( _CswNbtResources );
                Ret.ViewName = _ParentLocationInventoryLevelViewName;
                CswNbtViewRelationship LocationRel = _SdLocations.getAllParentsLocationRelationship( Ret, LocationId );
                CswNbtMetaDataObjectClass InventoryLevelOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InventoryLevelClass );
                CswNbtMetaDataObjectClassProp LocationOcp = InventoryLevelOc.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Location );
                CswNbtViewRelationship InventoryLevelRel = Ret.AddViewRelationship( LocationRel, NbtViewPropOwnerType.Second, LocationOcp, false );

                if( null != MaterialId )
                {
                    CswNbtMetaDataObjectClassProp MaterialOcp = InventoryLevelOc.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Material );
                    Ret.AddViewPropertyAndFilter( InventoryLevelRel, MaterialOcp, MaterialId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
                }
            }
            return Ret;
        }

        private Collection<CswNbtObjClassInventoryLevel> _InventoryLevels( CswNbtView InventoryLevelView )
        {
            Collection<CswNbtObjClassInventoryLevel> Ret = new Collection<CswNbtObjClassInventoryLevel>();
            if( null != InventoryLevelView && InventoryLevelView.ViewName == _ParentLocationInventoryLevelViewName )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( InventoryLevelView, false, false );
                Int32 LocationCount = Tree.getChildNodeCount();
                if( LocationCount > 0 )
                {
                    for( Int32 Loc = 0; Loc < LocationCount; Loc += 1 )
                    {
                        Tree.goToNthChild( Loc );
                        Int32 LevelCount = Tree.getChildNodeCount();
                        if( LevelCount > 0 )
                        {
                            for( Int32 Lev = 0; Lev < LevelCount; Lev += 1 )
                            {
                                Tree.goToNthChild( Lev );
                                CswNbtObjClassInventoryLevel InventoryLevel = Tree.getNodeForCurrentPosition();
                                Ret.Add( InventoryLevel );
                                Tree.goToParentNode();
                            }
                        }
                        Tree.goToParentNode();
                    }
                }
            }
            return Ret;
        }

        private void _applyQuantityToInventoryLevels( Collection<CswNbtObjClassInventoryLevel> InbtObjClassInventoryLevels, double Quantity, CswPrimaryKey UnitId, string Reason )
        {
            foreach( CswNbtObjClassInventoryLevel Level in InbtObjClassInventoryLevels )
            {
                _addToCurrentQuantity( Level, Quantity, UnitId, Reason );
            }
        }

        private void _getInventoryLevelCollections( out Collection<CswNbtObjClassInventoryLevel> PrevLevels, out Collection<CswNbtObjClassInventoryLevel> CurrentLevels, CswPrimaryKey PrevLocationId, CswPrimaryKey CurrentLocationId, CswPrimaryKey MaterialId = null )
        {
            CswNbtView PrevInventoryVelView = _getParentLocationInventoryLevelView( PrevLocationId, MaterialId );
            PrevLevels = _InventoryLevels( PrevInventoryVelView );

            CswNbtView CurrentInventoryVelView = _getParentLocationInventoryLevelView( CurrentLocationId, MaterialId );
            CurrentLevels = _InventoryLevels( CurrentInventoryVelView );
        }

        public void changeLocationOfLocation( CswPrimaryKey PrevLocationId, CswPrimaryKey CurrentLocationId )
        {
            Collection<CswNbtObjClassInventoryLevel> PrevLevels;
            Collection<CswNbtObjClassInventoryLevel> CurrentLevels;
            _getInventoryLevelCollections( out PrevLevels, out CurrentLevels, PrevLocationId, CurrentLocationId );

            Collection<CswNbtObjClassInventoryLevel> AppliesToLevels = new Collection<CswNbtObjClassInventoryLevel>();
            foreach( CswNbtObjClassInventoryLevel Prev in PrevLevels )
            {
                if( false == CurrentLevels.Contains( Prev ) )
                {
                    AppliesToLevels.Add( Prev );
                }
            }
            foreach( CswNbtObjClassInventoryLevel Current in CurrentLevels )
            {
                if( false == PrevLevels.Contains( Current ) && false == AppliesToLevels.Contains( Current ) )
                {
                    AppliesToLevels.Add( Current );
                }
            }
            foreach( CswNbtObjClassInventoryLevel LevelToUpdate in AppliesToLevels )
            {
                CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                LevelToUpdate.CurrentQuantity.Quantity = Mgr.getCurrentInventoryLevel( LevelToUpdate );
                LevelToUpdate.postChanges( true );
            }
        }

        public bool changeLocationOfQuantity( double Quantity, CswPrimaryKey UnitId, string Reason, CswPrimaryKey MaterialId, CswPrimaryKey PrevLocationId, CswPrimaryKey CurrentLocationId )
        {
            bool Ret = false;
            if( CswTools.IsPrimaryKey( MaterialId ) &&
                CswTools.IsPrimaryKey( PrevLocationId ) &&
                CswTools.IsPrimaryKey( CurrentLocationId ) )
            {
                Collection<CswNbtObjClassInventoryLevel> PrevLevels;
                Collection<CswNbtObjClassInventoryLevel> CurrentLevels;
                _getInventoryLevelCollections( out PrevLevels, out CurrentLevels, PrevLocationId, CurrentLocationId, MaterialId );

                Collection<CswNbtObjClassInventoryLevel> AppliesToPrevLevels = new Collection<CswNbtObjClassInventoryLevel>();
                foreach( CswNbtObjClassInventoryLevel Prev in PrevLevels )
                {
                    if( false == CurrentLevels.Contains( Prev ) )
                    {
                        AppliesToPrevLevels.Add( Prev );
                    }
                }
                //These Inventory Levels are losing inventory
                _applyQuantityToInventoryLevels( AppliesToPrevLevels, -( Quantity ), UnitId, Reason );

                Collection<CswNbtObjClassInventoryLevel> AppliesToCurrentLevels = new Collection<CswNbtObjClassInventoryLevel>();
                foreach( CswNbtObjClassInventoryLevel Current in CurrentLevels )
                {
                    if( false == PrevLevels.Contains( Current ) )
                    {
                        AppliesToCurrentLevels.Add( Current );
                    }
                }
                //These Inventory Levels are gaining inventory
                _applyQuantityToInventoryLevels( AppliesToCurrentLevels, Quantity, UnitId, Reason );
                Ret = true;
            }
            return Ret;
        }

        public bool addToCurrentQuantity( double Quantity, CswPrimaryKey UnitId, string Reason, CswPrimaryKey MaterialId = null, CswPrimaryKey LocationId = null )
        {
            bool Ret = false;
            if( CswTools.IsPrimaryKey( MaterialId ) &&
                CswTools.IsPrimaryKey( LocationId ) )
            {
                CswNbtView InventoryLevelView = _getParentLocationInventoryLevelView( LocationId, MaterialId );
                Collection<CswNbtObjClassInventoryLevel> InventoryLevels = _InventoryLevels( InventoryLevelView );
                _applyQuantityToInventoryLevels( InventoryLevels, Quantity, UnitId, Reason );
                Ret = true;
            }
            return Ret;
        }

        public bool addToCurrentQuantity( CswNbtObjClassInventoryLevel InventoryLevel, double Quantity, CswPrimaryKey UnitId, string Reason )
        {
            bool Ret = false;
            if( null != InventoryLevel )
            {
                Ret = _addToCurrentQuantity( InventoryLevel, Quantity, UnitId, Reason );
            }
            return Ret;
        }

        #endregion Inventory
    } // public class CswNbtSdInventoryLevelMgr

} // namespace ChemSW.Nbt.WebServices
