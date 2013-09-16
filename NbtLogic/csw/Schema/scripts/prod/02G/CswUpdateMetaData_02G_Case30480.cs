using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02G_Case30480 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Inventory Group is a Required Property"; } }

        public override string ScriptName
        {
            get { return "Locations Inventory Group = Required"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30480; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp InventoryGroupOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InventoryGroupOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );
        }
    }
}


