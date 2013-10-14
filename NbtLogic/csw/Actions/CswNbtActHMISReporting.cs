using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Conversion;

namespace ChemSW.Nbt.Actions
{
    #region DataContract

    [DataContract]
    public class HMISData
    {
        public HMISData()
        {
            Materials = new Collection<HMISMaterial>();
        }

        [DataMember]
        public String FireClassExemptAmountSet;
        [DataMember]
        public String ControlZone;
        [DataMember]
        public Collection<HMISMaterial> Materials;

        [DataContract]
        public class HMISMaterial
        {
            public HMISMaterial()
            {
                Storage = new StorageData();
                Closed = new ClosedData();
                Open = new OpenData();
            }

            [DataMember]
            public String Material = String.Empty;
            [DataMember]
            public Int32 NodeId = 0;
            [DataMember]
            public String HazardClass = String.Empty;
            [DataMember]
            public String HazardCategory = String.Empty;
            [DataMember]
            public String Class = String.Empty;
            [DataMember]
            public Double SortOrder = 0.0;
            [DataMember]
            public StorageData Storage;
            [DataMember]
            public ClosedData Closed;
            [DataMember]
            public OpenData Open;
        }

        [DataContract]
        public class StorageData
        {
            public StorageData()
            {
                Solid = new HMISQty();
                Liquid = new HMISQty();
                Gas = new HMISQty();
            }

            [DataMember]
            public HMISQty Solid;
            [DataMember]
            public HMISQty Liquid;
            [DataMember]
            public HMISQty Gas;
        }

        [DataContract]
        public class ClosedData
        {
            public ClosedData()
            {
                Solid = new HMISQty();
                Liquid = new HMISQty();
                Gas = new HMISQty();
            }

            [DataMember]
            public HMISQty Solid;
            [DataMember]
            public HMISQty Liquid;
            [DataMember]
            public HMISQty Gas;
        }

        [DataContract]
        public class OpenData
        {
            public OpenData()
            {
                Solid = new HMISQty();
                Liquid = new HMISQty();
            }

            [DataMember]
            public HMISQty Solid;
            [DataMember]
            public HMISQty Liquid;
        }

        [DataContract]
        public class HMISQty
        {
            private Int32 Precision = 6;
            private Double _Qty;
            [DataMember]
            public String MAQ = String.Empty;
            [DataMember]
            public Double Qty
            {
                get { return _Qty; }
                set { _Qty = CswTools.IsDouble( value ) ? Math.Round( value, Precision, MidpointRounding.AwayFromZero ) : 0.0; }
            }
        }

        [DataContract]
        public class HMISDataRequest
        {
            [DataMember]
            public String ControlZoneId = String.Empty;
            [DataMember]
            public String ControlZone = String.Empty;
            [DataMember]
            public String Class = String.Empty;
        }

    } // HMISData

    #endregion DataContract

    public class CswNbtActHMISReporting
    {
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        //private HMISData Data;
        //private CswPrimaryKey ControlZoneId;
        private const Int32 RoundPrecision = 3;

        public CswNbtActHMISReporting( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            //Data = new HMISData();
        }

        #endregion Properties and ctor
        
        #region Public Methods

        public DataTable getHMISDataTable( HMISData.HMISDataRequest Request )
        {
            DataTable ret = new DataTable( "HMIS" );
            ret.Columns.Add( "controlzone" );
            ret.Columns.Add( "material_nodeid" );
            ret.Columns.Add( "material" );
            ret.Columns.Add( "class" );
            ret.Columns.Add( "hazardcategory" );
            ret.Columns.Add( "hazardclass" );
            ret.Columns.Add( "storage_solid_maq" );
            ret.Columns.Add( "storage_solid_qty" );
            ret.Columns.Add( "storage_liquid_maq" );
            ret.Columns.Add( "storage_liquid_qty" );
            ret.Columns.Add( "storage_gas_maq" );
            ret.Columns.Add( "storage_gas_qty" );
            ret.Columns.Add( "closed_solid_maq" );
            ret.Columns.Add( "closed_solid_qty" );
            ret.Columns.Add( "closed_liquid_maq" );
            ret.Columns.Add( "closed_liquid_qty" );
            ret.Columns.Add( "closed_gas_maq" );
            ret.Columns.Add( "closed_gas_qty" );
            ret.Columns.Add( "open_solid_maq" );
            ret.Columns.Add( "open_solid_qty" );
            ret.Columns.Add( "open_liquid_maq" );
            ret.Columns.Add( "open_liquid_qty" );

            HMISData Data = getHMISData( Request );
            foreach( HMISData.HMISMaterial Material in Data.Materials )
            {
                DataRow thisRow = ret.NewRow();
                thisRow["controlzone"] = Data.ControlZone;
                thisRow["material_nodeid"] = Material.NodeId;
                thisRow["material"] = Material.Material;
                thisRow["class"] = Material.Class;
                thisRow["hazardcategory"] = Material.HazardCategory;
                thisRow["hazardclass"] = Material.HazardClass;
                thisRow["storage_solid_maq"] = Material.Storage.Solid.MAQ;
                thisRow["storage_solid_qty"] = Math.Round( Material.Storage.Solid.Qty, RoundPrecision );
                thisRow["storage_liquid_maq"] = Material.Storage.Liquid.MAQ;
                thisRow["storage_liquid_qty"] = Math.Round( Material.Storage.Liquid.Qty, RoundPrecision );
                thisRow["storage_gas_maq"] = Material.Storage.Gas.MAQ;
                thisRow["storage_gas_qty"] = Math.Round( Material.Storage.Gas.Qty, RoundPrecision );
                thisRow["closed_solid_maq"] = Material.Closed.Solid.MAQ;
                thisRow["closed_solid_qty"] = Math.Round( Material.Closed.Solid.Qty, RoundPrecision );
                thisRow["closed_liquid_maq"] = Material.Closed.Liquid.MAQ;
                thisRow["closed_liquid_qty"] = Math.Round( Material.Closed.Liquid.Qty, RoundPrecision );
                thisRow["closed_gas_maq"] = Material.Closed.Gas.MAQ;
                thisRow["closed_gas_qty"] = Math.Round( Material.Closed.Gas.Qty, RoundPrecision );
                thisRow["open_solid_maq"] = Material.Open.Solid.MAQ;
                thisRow["open_solid_qty"] = Math.Round( Material.Open.Solid.Qty, RoundPrecision );
                thisRow["open_liquid_maq"] = Material.Open.Liquid.MAQ;
                thisRow["open_liquid_qty"] = Math.Round( Material.Open.Liquid.Qty, RoundPrecision );
                ret.Rows.Add( thisRow );
            }
            return ret;
        }

        public HMISData getHMISData( HMISData.HMISDataRequest Request )
        {
            HMISData Data = new HMISData();
            CswPrimaryKey ControlZoneId = null;
            if( false == string.IsNullOrEmpty( Request.ControlZoneId ) )
            {
                ControlZoneId = CswConvert.ToPrimaryKey( Request.ControlZoneId );
            }
            else if( false == string.IsNullOrEmpty( Request.ControlZone ) )
            {
                if( CswTools.IsInteger( Request.ControlZone ) )
                {
                    ControlZoneId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Request.ControlZone ) );
                }
                else
                {
                    CswNbtView ControlZoneView = getControlZonesView( Request.ControlZone );
                    ICswNbtTree ControlZoneTree = _CswNbtResources.Trees.getTreeFromView( ControlZoneView, RequireViewPermissions: false, IncludeSystemNodes: true, IncludeHiddenNodes: true );
                    if( ControlZoneTree.getChildNodeCount() > 0 )
                    {
                        ControlZoneTree.goToNthChild( 0 );
                        ControlZoneId = ControlZoneTree.getNodeIdForCurrentPosition();
                    }
                }
            }

            if( CswTools.IsPrimaryKey( ControlZoneId ) )
            {
                Data.ControlZone = _CswNbtResources.Nodes.getNodeName( ControlZoneId );

                string HMISSql = @"with loc as (select n.nodeid
                                                  from nodes n
                                                  join nodetypes t on t.nodetypeid = n.nodetypeid
                                                  join object_class oc on t.objectclassid = oc.objectclassid
                                                  join (select j.nodeid, j.field1_fk 
                                                          from object_class_props ocp  
                                                          join nodetype_props ntp on ocp.objectclasspropid = ntp.objectclasspropid 
                                                          join jct_nodes_props j on ntp.nodetypepropid = j.nodetypepropid
                                                         where ocp.propname = 'Control Zone'
                                                       ) cz on (cz.nodeid = n.nodeid)
                                                 where oc.objectclass = 'LocationClass'
                                                   and cz.field1_fk = " + ControlZoneId.PrimaryKey + @"
                                               ),
                                        mat as (select n.nodeid, n.nodename materialname, hc.clobdata hazardclasses, sf.gestaltsearch specialflags, ps.field1 physstate
                                                  from nodes n
                                                  join nodetypes t on t.nodetypeid = n.nodetypeid
                                                  join object_class oc on t.objectclassid = oc.objectclassid
                                                  join (select j.nodeid, j.clobdata
                                                          from object_class_props ocp  
                                                          join nodetype_props ntp on ocp.objectclasspropid = ntp.objectclasspropid 
                                                          join jct_nodes_props j on ntp.nodetypepropid = j.nodetypepropid
                                                         where ocp.propname = 'Hazard Classes'
                                                       ) hc on (hc.nodeid = n.nodeid)
                                                  join (select j.nodeid, j.gestaltsearch 
                                                          from object_class_props ocp  
                                                          join nodetype_props ntp on ocp.objectclasspropid = ntp.objectclasspropid 
                                                          join jct_nodes_props j on ntp.nodetypepropid = j.nodetypepropid
                                                         where ocp.propname = 'Special Flags'
                                                       ) sf on (sf.nodeid = n.nodeid)
                                                  join (select j.nodeid, j.field1
                                                          from object_class_props ocp  
                                                          join nodetype_props ntp on ocp.objectclasspropid = ntp.objectclasspropid 
                                                          join jct_nodes_props j on ntp.nodetypepropid = j.nodetypepropid
                                                         where ocp.propname = 'Physical State'
                                                       ) ps on (ps.nodeid = n.nodeid)
                                                 where oc.objectclass = 'ChemicalClass'
                                                   and sf.gestaltsearch not like '%Not Reportable%'";
                if( string.IsNullOrEmpty( Request.Class ) )
                {
                    HMISSql += "   and hc.clobdata is not null";
                }
                else
                {
                    HMISSql += "   and hc.clobdata like '%" + Request.Class + @"%'";
                }
                HMISSql += @"                ),
                                       cont as (select SUM(q.field2_numeric) total_qty_kg, 
                                                       SUM(q.field3_numeric) total_qty_lt, 
                                                       ut.field1 usetype, 
                                                       m.field1_fk materialid
                                                  from nodes n
                                                  join nodetypes t on t.nodetypeid = n.nodetypeid
                                                  join object_class oc on t.objectclassid = oc.objectclassid
                                                  join (select j.nodeid, j.field1_fk 
                                                          from object_class_props ocp  
                                                          join nodetype_props ntp on ocp.objectclasspropid = ntp.objectclasspropid 
                                                          join jct_nodes_props j on ntp.nodetypepropid = j.nodetypepropid
                                                         where ocp.propname = 'Location'
                                                       ) l on (l.nodeid = n.nodeid)
                                                  join (select j.nodeid, j.field2_numeric, j.field3_numeric 
                                                          from object_class_props ocp  
                                                          join nodetype_props ntp on ocp.objectclasspropid = ntp.objectclasspropid 
                                                          join jct_nodes_props j on ntp.nodetypepropid = j.nodetypepropid
                                                         where ocp.propname = 'Quantity'
                                                       ) q on (q.nodeid = n.nodeid)
                                                  join (select j.nodeid, j.field1 
                                                          from object_class_props ocp  
                                                          join nodetype_props ntp on ocp.objectclasspropid = ntp.objectclasspropid 
                                                          join jct_nodes_props j on ntp.nodetypepropid = j.nodetypepropid
                                                         where ocp.propname = 'Use Type'
                                                       ) ut on (ut.nodeid = n.nodeid)
                                                  join (select j.nodeid, j.field1_fk 
                                                          from object_class_props ocp  
                                                          join nodetype_props ntp on ocp.objectclasspropid = ntp.objectclasspropid 
                                                          join jct_nodes_props j on ntp.nodetypepropid = j.nodetypepropid
                                                         where ocp.propname = 'Material'
                                                       ) m on (m.nodeid = n.nodeid)
                                                 where oc.objectclass = 'ContainerClass'
                                                   and ut.field1 is not null
                                                   and l.field1_fk in (select nodeid from loc) 
                                                   and (q.field2_numeric > 0 
                                                     or q.field3_numeric > 0)
                                                 group by ut.field1, m.field1_fk
                                               )
                                   select c.*, mat.hazardclasses, mat.specialflags, mat.materialname, mat.physstate
                                     from cont c
                                     join mat on (c.materialid = mat.nodeid)";
                CswArbitrarySelect HMISSelect = _CswNbtResources.makeCswArbitrarySelect( "HMIS_Select", HMISSql );
                DataTable HMISTable = HMISSelect.getTable();

                if( string.IsNullOrEmpty( Request.Class ) )
                {
                    // Get totals for all classes
                    _setFireClasses( ControlZoneId, Data );

                    foreach( DataRow row in HMISTable.Rows )
                    {
                        CswCommaDelimitedString HazardClasses = new CswCommaDelimitedString();
                        HazardClasses.FromString( CswConvert.ToString( row["hazardclasses"] ) );
                        if( HazardClasses.Contains( "FL-1A" ) || HazardClasses.Contains( "FL-1B" ) || HazardClasses.Contains( "FL-1C" ) )
                        {
                            HazardClasses.Add( "FL-Comb" );
                        }
                        foreach( String HazardClass in HazardClasses )
                        {
                            HMISData.HMISMaterial HMISMaterial = Data.Materials.FirstOrDefault( EmptyHazardClass => EmptyHazardClass.HazardClass == HazardClass );
                            if( null != HMISMaterial ) //This would only be null if the Material's HazardClass options don't match the Default FireClass nodes
                            {
                                _addQuantityDataToHMISMaterial( HMISMaterial,
                                                                CswConvert.ToString( row["usetype"] ),
                                                                CswConvert.ToDouble( row["total_qty_kg"] ),
                                                                CswConvert.ToDouble( row["total_qty_lt"] ),
                                                                CswConvert.ToString( row["physstate"] ),
                                                                new CswPrimaryKey( "nodes", CswConvert.ToInt32( row["materialid"] ) ) );
                            }
                        }
                    } // foreach( DataRow row in HMISTable )
                } // if( string.IsNullOrEmpty( Request.Class ) )
                else
                {
                    // Get material information for one class
                    foreach( DataRow row in HMISTable.Rows )
                    {
                        HMISData.HMISMaterial NewMaterial = new HMISData.HMISMaterial
                            {
                                Material = CswConvert.ToString( row["materialname"] ),
                                NodeId = CswConvert.ToInt32( row["materialid"] ),
                                HazardClass = Request.Class
                            };
                        _addQuantityDataToHMISMaterial( NewMaterial,
                                                        CswConvert.ToString( row["usetype"] ),
                                                        CswConvert.ToDouble( row["total_qty_kg"] ),
                                                        CswConvert.ToDouble( row["total_qty_lt"] ),
                                                        CswConvert.ToString( row["physstate"] ),
                                                        new CswPrimaryKey( "nodes", CswConvert.ToInt32( row["materialid"] ) ) );
                        Data.Materials.Add( NewMaterial );
                    }
                } // if-else( string.IsNullOrEmpty( Request.Class ) )
            } // if( CswTools.IsPrimaryKey( ControlZoneId ) )
            return Data;
        }

        #endregion Public Methods

        #region Private Methods

        private CswNbtView getControlZonesView( string FilterToName = "" )
        {
            CswNbtView ControlZonesView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass ControlZoneOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            CswNbtMetaDataObjectClassProp ControlZoneNameOCP = ControlZoneOC.getObjectClassProp( CswNbtObjClassControlZone.PropertyName.Name );
            CswNbtViewRelationship ControlZoneVR = ControlZonesView.AddViewRelationship( ControlZoneOC, IncludeDefaultFilters: true );
            if( false == string.IsNullOrEmpty( FilterToName ) )
            {
                ControlZonesView.AddViewPropertyAndFilter( ControlZoneVR,
                                                           ControlZoneNameOCP,
                                                           Value: FilterToName );
            }
            ControlZonesView.ViewName = "HMIS Control Zones";
            return ControlZonesView;
        }

        private void _setFireClasses( CswPrimaryKey ControlZoneId, HMISData Data )
        {
            CswNbtNode ControlZone = _CswNbtResources.Nodes.GetNode( ControlZoneId );
            Double MAQOffset = Double.NaN;
            CswNbtMetaDataNodeTypeProp MAQOffsetNTP = _CswNbtResources.MetaData.getNodeTypeProp( ControlZone.NodeTypeId, "MAQ Offset %" );
            if( null != MAQOffsetNTP )
            {
                MAQOffset = ControlZone.Properties[MAQOffsetNTP].AsNumber.Value;
            }
            MAQOffset = Double.IsNaN( MAQOffset ) ? 100.0 : MAQOffset;
            CswNbtMetaDataNodeTypeProp FireClassSetNameNTP = _CswNbtResources.MetaData.getNodeTypeProp( ControlZone.NodeTypeId, "Fire Class Set Name" );
            CswPrimaryKey FCEASId = ControlZone.Properties[FireClassSetNameNTP].AsRelationship.RelatedNodeId;
            Data.FireClassExemptAmountSet = ControlZone.Properties[FireClassSetNameNTP].AsRelationship.CachedNodeName;
            if( null != FCEASId )
            {
                List<HMISData.HMISMaterial> HazardClassList = new List<HMISData.HMISMaterial>();
                CswNbtMetaDataObjectClass FCEAOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountClass );
                foreach( CswNbtObjClassFireClassExemptAmount FCEANode in FCEAOC.getNodes( false, false ) )
                {
                    if( FCEANode.SetName.RelatedNodeId == FCEASId )
                    {
                        HMISData.HMISMaterial EmptyHazardClass = new HMISData.HMISMaterial
                        {
                            HazardClass = FCEANode.HazardClass.Value,
                            HazardCategory = FCEANode.HazardCategory.Text,
                            Class = FCEANode.Class.Text,
                            SortOrder = FCEANode.SortOrder.Value
                        };
                        _setFireClassMAQData( EmptyHazardClass, FCEANode, MAQOffset );
                        HazardClassList.Add( EmptyHazardClass );
                    }
                }
                HazardClassList.Sort( ( s1, s2 ) => s1.SortOrder.CompareTo( s2.SortOrder ) );
                Data.Materials = new Collection<HMISData.HMISMaterial>( HazardClassList );
            }
        }

        private void _setFireClassMAQData( HMISData.HMISMaterial Hazard, CswNbtObjClassFireClassExemptAmount FireClass, Double MAQOffset )
        {
            Hazard.Storage.Solid.MAQ = _calculateMAQOffsetPercentage( FireClass.StorageSolidExemptAmount.Text, MAQOffset );
            Hazard.Storage.Liquid.MAQ = _calculateMAQOffsetPercentage( FireClass.StorageLiquidExemptAmount.Text, MAQOffset );
            Hazard.Storage.Gas.MAQ = _calculateMAQOffsetPercentage( FireClass.StorageGasExemptAmount.Text, MAQOffset );
            Hazard.Closed.Solid.MAQ = _calculateMAQOffsetPercentage( FireClass.ClosedSolidExemptAmount.Text, MAQOffset );
            Hazard.Closed.Liquid.MAQ = _calculateMAQOffsetPercentage( FireClass.ClosedLiquidExemptAmount.Text, MAQOffset );
            Hazard.Closed.Gas.MAQ = _calculateMAQOffsetPercentage( FireClass.ClosedGasExemptAmount.Text, MAQOffset );
            Hazard.Open.Solid.MAQ = _calculateMAQOffsetPercentage( FireClass.OpenSolidExemptAmount.Text, MAQOffset );
            Hazard.Open.Liquid.MAQ = _calculateMAQOffsetPercentage( FireClass.OpenLiquidExemptAmount.Text, MAQOffset );
        }

        private String _calculateMAQOffsetPercentage( String ExemptAmountText, Double MAQOffset )
        {
            String OffsetText = ExemptAmountText;
            if( false == string.IsNullOrEmpty( OffsetText ) && false == OffsetText.Contains( "NL" ) && MAQOffset < 100.0 )
            {
                String FormatText = "{0}";
                if( OffsetText.StartsWith( "(" ) )
                {
                    FormatText = "({0})";
                    OffsetText = OffsetText.Replace( "(", "" );
                    OffsetText = OffsetText.Replace( ")", "" );
                }
                else if( OffsetText.EndsWith( "mCi" ) )
                {
                    FormatText = "{0} mCi";
                    OffsetText = OffsetText.Replace( " mCi", "" );
                }
                else if( OffsetText.EndsWith( "Ci" ) )
                {
                    FormatText = "{0} Ci";
                    OffsetText = OffsetText.Replace( " Ci", "" );
                }
                Double ExemptAmount = Double.Parse( OffsetText );
                Double OffsetAmount = ExemptAmount * ( MAQOffset / 100.0 );
                OffsetText = String.Format( FormatText, OffsetAmount );
            }
            return OffsetText;
        }

        private void _addQuantityDataToHMISMaterial( HMISData.HMISMaterial Material, String UseType, Double Quantity_Kgs, Double Quantity_Lts, string PhysState, CswPrimaryKey MaterialId )
        {
            CswNbtUnitConversion Conversion1 = new CswNbtUnitConversion( _CswNbtResources, _getUnitIdByName( "kg" ), _getBaseUnitId( PhysState ), MaterialId );
            Double ConvertedQty1 = Conversion1.convertUnit( Quantity_Kgs );
            CswNbtUnitConversion Conversion2 = new CswNbtUnitConversion( _CswNbtResources, _getUnitIdByName( "Liters" ), _getBaseUnitId( PhysState ), MaterialId );
            Double ConvertedQty2 = Conversion2.convertUnit( Quantity_Lts );
            Double ConvertedQty = ConvertedQty1 + ConvertedQty2;

            switch( UseType )
            {
                case CswEnumNbtContainerUseTypes.Storage:
                    switch( PhysState.ToLower() )
                    {
                        case CswNbtPropertySetMaterial.CswEnumPhysicalState.Solid:
                            Material.Storage.Solid.Qty += ConvertedQty;
                            break;
                        case CswNbtPropertySetMaterial.CswEnumPhysicalState.Liquid:
                            Material.Storage.Liquid.Qty += ConvertedQty;
                            break;
                        case CswNbtPropertySetMaterial.CswEnumPhysicalState.Gas:
                            Material.Storage.Gas.Qty += ConvertedQty;
                            break;
                    }
                    break;
                case CswEnumNbtContainerUseTypes.Closed:
                    switch( PhysState.ToLower() )
                    {
                        case CswNbtPropertySetMaterial.CswEnumPhysicalState.Solid:
                            Material.Storage.Solid.Qty += ConvertedQty;
                            Material.Closed.Solid.Qty += ConvertedQty;
                            break;
                        case CswNbtPropertySetMaterial.CswEnumPhysicalState.Liquid:
                            Material.Storage.Liquid.Qty += ConvertedQty;
                            Material.Closed.Liquid.Qty += ConvertedQty;
                            break;
                        case CswNbtPropertySetMaterial.CswEnumPhysicalState.Gas:
                            Material.Storage.Gas.Qty += ConvertedQty;
                            Material.Closed.Gas.Qty += ConvertedQty;
                            break;
                    }
                    break;
                case CswEnumNbtContainerUseTypes.Open:
                    switch( PhysState.ToLower() )
                    {
                        case CswNbtPropertySetMaterial.CswEnumPhysicalState.Solid:
                            Material.Open.Solid.Qty += ConvertedQty;
                            break;
                        case CswNbtPropertySetMaterial.CswEnumPhysicalState.Liquid:
                            Material.Open.Liquid.Qty += ConvertedQty;
                            break;
                    }
                    break;
            }
        } // _addQuantityDataToHMISMaterial()

        private CswPrimaryKey _getBaseUnitId( string PhysicalState )
        {
            string UnitName;
            switch( PhysicalState.ToLower() )
            {
                case CswNbtPropertySetMaterial.CswEnumPhysicalState.Liquid:
                    UnitName = "gal";
                    break;
                case CswNbtPropertySetMaterial.CswEnumPhysicalState.Gas:
                    UnitName = "cu.ft.";
                    break;
                case CswNbtPropertySetMaterial.CswEnumPhysicalState.Solid:
                case CswNbtPropertySetMaterial.CswEnumPhysicalState.NA:
                default:
                    UnitName = "lb";
                    break;
            }
            return _getUnitIdByName( UnitName );
        }


        private Dictionary<string, CswPrimaryKey> _unitsByName = new Dictionary<string, CswPrimaryKey>();

        private CswPrimaryKey _getUnitIdByName( string UnitName )
        {
            if( _unitsByName.Keys.Count == 0 )
            {
                CswNbtMetaDataObjectClass UoMOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
                foreach( CswNbtObjClassUnitOfMeasure UoMNode in UoMOC.getNodes( false, false ) )
                {
                    _unitsByName[UoMNode.Name.Text] = UoMNode.NodeId;
                    foreach( string Alias in UoMNode.AliasesAsDelimitedString )
                    {
                        _unitsByName[Alias] = UoMNode.NodeId;
                    }
                }
            }
            return _unitsByName[UnitName];
        } // _getUnitByName()

        #endregion Private Methods
    }
}
