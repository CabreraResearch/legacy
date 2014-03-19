using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case53015: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE;} 
        }

        public override int CaseNo
        {
            get { return 53015; }
        }

        public override string Title
        {
            get { return "Rename duplicate vendor nodes so that script 52285 can run safely"; }
        }

        public override void update()
        {
            //Enforce vendor name uniqueness

            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            foreach( CswNbtObjClassVendor _currentNode in VendorOC.getNodes( false, false, false ) )
            {
                _currentNode.VendorName.makeUnique();
                _currentNode.postChanges( false );
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema