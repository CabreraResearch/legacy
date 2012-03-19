using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25290b
    /// </summary>
    public class CswUpdateSchemaCase25290b : CswUpdateSchemaTo
    {
        public override void update()
        {
            //validate role nodetype permissions
            foreach( CswNbtNode roleNode in _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass ).getNodes( false, true ) )
            {
                CswNbtObjClassRole nodeAsRole = CswNbtNodeCaster.AsRole( roleNode );
                CswNbtNodePropMultiList prop = (CswNbtNodePropMultiList) nodeAsRole.NodeTypePermissions;
                prop.ValidateValues();
                roleNode.postChanges( false );
            }

        }//Update()

    }//class CswUpdateSchemaCase25290b

}//namespace ChemSW.Nbt.Schema