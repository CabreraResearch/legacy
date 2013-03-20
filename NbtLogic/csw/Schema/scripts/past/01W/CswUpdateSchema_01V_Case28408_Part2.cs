using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28408_Part2
    /// </summary>
    public class CswUpdateSchema_01V_Case28408_Part2 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28408; }
        }

        public override void update()
        {
            //Part 2: Set the barcode property on pre-existing nodes
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );

            foreach( CswNbtMetaDataNodeType UserOCNT in UserOC.getNodeTypes() )
            {
                foreach( CswNbtObjClassUser UserNode in UserOCNT.getNodes( false, true ) )
                {
                    UserNode.Barcode.setBarcodeValue();
                    UserNode.postChanges( false );
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28408_Part2

}//namespace ChemSW.Nbt.Schema