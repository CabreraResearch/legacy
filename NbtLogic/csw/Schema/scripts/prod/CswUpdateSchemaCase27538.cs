using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27538
    /// </summary>
    public class CswUpdateSchemaCase27538 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            _CswNbtSchemaModTrnsctn.Modules.triggerEvent( CswNbtModuleName.CISPro );

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema