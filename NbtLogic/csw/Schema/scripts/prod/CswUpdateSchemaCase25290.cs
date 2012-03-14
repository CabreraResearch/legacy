using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25290
    /// </summary>
    public class CswUpdateSchemaCase25290 : CswUpdateSchemaTo
    {
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

    }//class CswUpdateSchemaCase25290

}//namespace ChemSW.Nbt.Schema