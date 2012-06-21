using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
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
        private CswNbtObjClassInventoryLevel _InventoryLevel;

        public CswNbtSdInventoryLevelMgr( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }
        public CswNbtSdInventoryLevelMgr( CswNbtResources Resources, CswNbtObjClassInventoryLevel InventoryLevel )
        {
            _CswNbtResources = Resources;
            _InventoryLevel = InventoryLevel;
        }

        #region Validation

        public bool doSendEmail()
        {
            return isLevelPastThreshhold() && _InventoryLevel.LastNotified.DateTimeValue <= DateTime.Now.AddDays( -1 );
        }

        public bool isLevelPastThreshhold()
        {
            bool Ret = false;

            switch( _InventoryLevel.Type.Value )
            {
                case CswNbtObjClassInventoryLevel.Types.Maximum:
                    Ret = ( _InventoryLevel.CurrentQuantity.Quantity > _InventoryLevel.Level.Quantity );
                    break;
                case CswNbtObjClassInventoryLevel.Types.Minimum:
                    Ret = ( _InventoryLevel.CurrentQuantity.Quantity < _InventoryLevel.Level.Quantity );
                    break;
            }

            return Ret;
        }

        #endregion Validation

        #region Email

        private void _sendEmail( string Recipient )
        {
            string Subject = "";
            string Message = "";

            Collection<CswMailMessage> EmailMessage = _CswNbtResources.makeMailMessages( Subject, Message,
                                                                                        Recipient );
            _CswNbtResources.sendEmailNotification( EmailMessage );
        }

        /// <summary>
        /// Sends notification email and returns Now
        /// </summary>
        /// <returns></returns>
        public DateTime sendPastThreshholdEmail()
        {
            foreach( CswNbtObjClassUser User in _InventoryLevel.Subscribe.SelectedUsers() )
            {
                if( false == string.IsNullOrEmpty( User.Email ) )
                {
                    _sendEmail( User.Email );
                }
            }
            return DateTime.Now;
        }

        #endregion Email

        #region Inventory

        public CswNbtView QuantityView( CswPrimaryKey StartLocationId )
        {
            CswNbtView Ret = null;
            
            Ret = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship LocationRel = _getLocationRelationship( Ret, StartLocationId );

            CswNbtMetaDataObjectClass ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp LocationOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.LocationPropertyName );
            CswNbtMetaDataObjectClassProp MaterialOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.MaterialPropertyName );
            CswNbtMetaDataObjectClassProp DisposedOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.DisposedPropertyName );
            CswNbtMetaDataObjectClassProp MissingOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.MissingPropertyName );
            CswNbtMetaDataObjectClassProp QuantityOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.QuantityPropertyName );

            CswNbtViewRelationship ContainerRel = Ret.AddViewRelationship( LocationRel, NbtViewPropOwnerType.Second, LocationOcp, false );
            Ret.AddViewPropertyAndFilter( ContainerRel, MaterialOcp, _InventoryLevel.Material.RelatedNodeId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
            Ret.AddViewPropertyAndFilter( ContainerRel, DisposedOcp, Tristate.True.ToString(), CswNbtSubField.SubFieldName.Checked, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            Ret.AddViewPropertyAndFilter( ContainerRel, MissingOcp, Tristate.True.ToString(), CswNbtSubField.SubFieldName.Checked, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            Ret.AddViewProperty( ContainerRel, QuantityOcp );
            return Ret;
        }

        private CswNbtViewRelationship _getLocationRelationship( CswNbtView LocationsView, CswPrimaryKey StartLocationId )
        {
            CswNbtViewRelationship LocationRel = null;
            if( null != StartLocationId )
            {
                string LocationSql = @"select distinct nodeid from (select n.nodeid, jnp.field1_fk
                                      from nodes n 
                                      join nodetypes nt on n.nodetypeid=nt.nodetypeid
                                      join object_class oc on nt.objectclassid=oc.objectclassid
                                      join jct_nodes_props jnp on n.nodeid=jnp.nodeid
                                      join nodetype_props ntp on jnp.nodetypepropid=ntp.nodetypepropid
                                      join field_types ft on ntp.fieldtypeid=ft.fieldtypeid
                                      where oc.objectclass='LocationClass' 
                                            and ft.fieldtype='Location'  
                                      start with n.nodeid = " + StartLocationId.PrimaryKey + " " +
                                     " connect by jnp.field1_fk = prior n.nodeid )";
                CswArbitrarySelect LocationSelect = _CswNbtResources.makeCswArbitrarySelect( "populateLocations_select", LocationSql );
                DataTable LocationTable = null;
                try
                {
                    LocationTable = LocationSelect.getTable();
                    //For faster lookup
                    Dictionary<Int32, Int32> LocationDict = new Dictionary<int, int>();
                    //For assignment
                    Collection<CswPrimaryKey> LocationPks = new Collection<CswPrimaryKey>();
                    LocationDict.Add( StartLocationId.PrimaryKey, StartLocationId.PrimaryKey );
                    LocationPks.Add( StartLocationId );
                    CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                    LocationRel = LocationsView.AddViewRelationship( LocationOc, false );

                    if( LocationTable.Rows.Count > 0 )
                    {
                        foreach( DataRow Row in LocationTable.Rows )
                        {
                            Int32 LocationNodeId = CswConvert.ToInt32( Row["nodeid"] );
                            if( false == LocationDict.ContainsKey( LocationNodeId ) )
                            {
                                LocationDict.Add( LocationNodeId, LocationNodeId );
                                CswPrimaryKey LocationPk = new CswPrimaryKey( "nodes", LocationNodeId );
                                LocationPks.Add( LocationPk );
                            }
                        }
                    }
                    LocationRel.NodeIdsToFilterIn = LocationPks;
                }
                catch( Exception ex )
                {
                    throw new CswDniException( ErrorType.Error, "Invalid Query", "_getContainerRelationship() attempted to run invalid SQL: " + LocationSql, ex );
                }
            }
            return LocationRel;
        }

        public double getCurrentInventoryLevel()
        {
            double Ret = 0;
            CswNbtView ContainerView = QuantityView( _InventoryLevel.Location.SelectedNodeId );
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
                                            Conversion = new CswNbtUnitConversion( _CswNbtResources, _InventoryLevel.Level.UnitId, new CswPrimaryKey( "nodes", UnitTypeId ), _InventoryLevel.Material.RelatedNodeId );
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

        #endregion Inventory
    } // public class CswNbtSdInventoryLevelMgr

} // namespace ChemSW.Nbt.WebServices
