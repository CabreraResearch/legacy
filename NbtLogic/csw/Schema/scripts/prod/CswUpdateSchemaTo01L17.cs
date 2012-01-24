using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-17
    /// </summary>
    public class CswUpdateSchemaTo01L17 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 17 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 24587

            // The FKType and FKValue of all Location properties on Location nodetypes should be the Location object class in the master.
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass);
            foreach(CswNbtMetaDataNodeType LocationNT in LocationOC.NodeTypes)
            {
                CswNbtMetaDataNodeTypeProp LocationProp = LocationNT.getNodeTypePropByObjectClassPropName(CswNbtObjClassLocation.LocationPropertyName);
                LocationProp.SetFK(CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), LocationOC.ObjectClassId);
            }
            
            #endregion case 24587


        }//Update()

    }//class CswUpdateSchemaTo01L17

}//namespace ChemSW.Nbt.Schema


