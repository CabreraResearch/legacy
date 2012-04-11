using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24438, part B
    /// </summary>
    public class CswUpdateSchemaCase24438B : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Add Employee ID (Text) as a nodetype property on User to the master data.  Set it unique.

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp EmployeeIdNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( UserNT, CswNbtMetaDataFieldType.NbtFieldType.Text, "Employee ID", UserNT.getFirstNodeTypeTab().TabId );
                EmployeeIdNTP.setIsUnique( true );
            }

        }//Update()

    }//class CswUpdateSchemaCase24438B

}//namespace ChemSW.Nbt.Schema