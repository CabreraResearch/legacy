using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-13
    /// </summary>
    public class CswUpdateSchemaTo01M13 : CswUpdateSchemaTo
    {
        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 13 ); } }
        //public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            //case 25290


            //remove nodetypes (should delete nodes automatically)
            Collection<CswNbtMetaDataNodeType> doomed_nts = new Collection<CswNbtMetaDataNodeType>();
            IEnumerable<CswNbtMetaDataNodeType> nts = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes();
            foreach( CswNbtMetaDataNodeType nt in nts )
            {
                if( nt.NodeTypeName == "Inspection Group"
                    || nt.NodeTypeName == "FE Inspection Point"
                    || nt.NodeTypeName == "Physical Inspection Route"
                    || nt.NodeTypeName == "Physical Inspection" ) doomed_nts.Add( nt );
            }
            foreach( CswNbtMetaDataNodeType ntd in doomed_nts )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( ntd );
            }

            // Inspection Group
            // FE Inspection Point
            // Physical Inspection Route
            // Physical Inspection


            //remove remaining views in the inspections category

            //if we have a Department nodetype, then ROOM points to department, and building doesn't
            CswNbtMetaDataNodeType deptNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Department" );
            if( null != deptNT )
            {
                CswNbtMetaDataNodeType bldg = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Building" );
                if( null != bldg )
                {
                    CswNbtMetaDataNodeTypeProp bdept = bldg.getNodeTypeProp( "Department" );
                    if( null != bdept )
                    {
                        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( bdept );
                    }
                }
                CswNbtMetaDataNodeType room = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
                if( null != room )
                {
                    CswNbtMetaDataNodeTypeProp rdept = bldg.getNodeTypeProp( "Department" );
                    if( null == rdept )
                    {
                        CswNbtMetaDataNodeTypeTab tab = room.getFirstNodeTypeTab();
                        CswNbtMetaDataNodeTypeProp deptProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( room, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Department", tab.TabId );
                        deptProp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(),deptNT.NodeTypeId);
                    }
                }
            }


            //case 25290

        }//Update()

    }//class CswUpdateSchemaTo01M13

}//namespace ChemSW.Nbt.Schema