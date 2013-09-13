using System;
using System.Data;
using System.Linq;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30564B : CswUpdateSchemaTo
    {
        public override string Title { get { return "Unify Location property layout"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30564; }
        }

        public override string ScriptName
        {
            get { return "Case30564-LocationLayout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                // Object class props first
                Int32 FirstTabId = LocationNT.getFirstNodeTypeTab().TabId;
                foreach( CswNbtMetaDataNodeTypeProp PropNTP in LocationNT.getNodeTypeProps().Where( p => Int32.MinValue != p.ObjectClassPropId ) )
                {
                    switch( PropNTP.getObjectClassPropName() )
                    {
                        case CswNbtObjClassLocation.PropertyName.Barcode:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 1, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.Name:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 2, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.Location:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 3, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.Order:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 4, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 4, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.LocationCode:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 5, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 5, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.AllowInventory:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 6, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 6, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.InventoryGroup:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 7, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 7, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.StorageCompatibility:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 8, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 8, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.ControlZone:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 9, DisplayColumn: 1 );
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: 9, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassLocation.PropertyName.Containers:
                            // fine as is
                            break;
                        case CswNbtObjClassLocation.PropertyName.InventoryLevels:
                            // fine as is
                            break;
                        case CswNbtObjClassLocation.PropertyName.Responsible:
                            PropNTP.removeFromAllLayouts();
                            break;
                        case CswNbtObjClassLocation.PropertyName.LocationTemplate:
                            PropNTP.removeFromAllLayouts();
                            break;
                        case CswNbtObjClassLocation.PropertyName.ChildLocationType:
                            PropNTP.removeFromAllLayouts();
                            break;
                        case CswNbtObjClassLocation.PropertyName.Rows:
                            PropNTP.removeFromAllLayouts();
                            break;
                        case CswNbtObjClassLocation.PropertyName.Columns:
                            PropNTP.removeFromAllLayouts();
                            break;
                    }
                }

                // NodeTypeProps next
                Int32 r = 10;
                foreach( CswNbtMetaDataNodeTypeProp PropNTP in LocationNT.getNodeTypeProps().Where( p => Int32.MinValue == p.ObjectClassPropId ) )
                {
                    if( PropNTP.PropName != "Legacy Id" )
                    {
                        PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: r, DisplayColumn: 1 );
                        PropNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, FirstTabId, DisplayRow: r, DisplayColumn: 1 );
                        r++;
                    }
                }
            }
        } // update()

    } // class CswUpdateSchema_02G_Case30564B
}