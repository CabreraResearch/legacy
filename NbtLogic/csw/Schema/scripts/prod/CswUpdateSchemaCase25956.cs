using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25956
    /// </summary>
    public class CswUpdateSchemaCase25956 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // create the RelationshipOptionLimit config var

            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswNbtResources.ConfigurationVariables.relationshipoptionlimit, "Maximum number of options for relationships before search is required", "250", false );

        }//Update()

    }//class CswUpdateSchemaCase25956

}//namespace ChemSW.Nbt.Schema