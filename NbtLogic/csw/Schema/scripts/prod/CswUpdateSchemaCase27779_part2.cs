using ChemSW.DB;
using ChemSW.Core;
using System;
using System.Data;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27779_part2
    /// </summary>
    public class CswUpdateSchemaCase27779_part2 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            //delete the length column
            _CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", "length" );

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema