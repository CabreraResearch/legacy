using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to case 25290
    /// </summary>
    public class CswUpdateSchemaTo25290 : CswUpdateSchemaTo
    {
        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 12 ); } }
        //public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            
            //validate role nodetype permissions
            foreach( CswNbtNode roleNode in _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass ).getNodes( false, true ) )
            {
                CswNbtObjClassRole nodeAsRole = CswNbtNodeCaster.AsRole( roleNode );
                CswNbtNodePropMultiList prop = (CswNbtNodePropMultiList) nodeAsRole.NodeTypePermissions;
                prop.ValidateValues();
            }

        }//Update()

    }//class CswUpdateSchemaTo25290

}//namespace ChemSW.Nbt.Schema