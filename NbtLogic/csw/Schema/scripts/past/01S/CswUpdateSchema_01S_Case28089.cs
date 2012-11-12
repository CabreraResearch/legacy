using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28089
    /// </summary>
    public class CswUpdateSchema_01S_Case28089 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Fix missing role timeouts
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true, false ) )
            {
                if( Double.IsNaN( RoleNode.Timeout.Value ) )
                {
                    RoleNode.Timeout.Value = 30;
                    RoleNode.postChanges( false );
                }
            }
        } //Update()

    } //class CswUpdateSchema_01S_Case28089

}//namespace ChemSW.Nbt.Schema