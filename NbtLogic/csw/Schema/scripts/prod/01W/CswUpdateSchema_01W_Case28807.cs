using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28807
    /// </summary>
    public class CswUpdateSchema_01W_Case28807 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28807; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach ( CswNbtObjClassMaterial MaterialNode in MaterialOC.getNodes( false, false ) )
            {
                if( MaterialNode.TradeName.Text.Contains("Default") )
                {
                    MaterialNode.ApprovedForReceiving.Checked = Tristate.True;
                    MaterialNode.postChanges( true );
                    MaterialNode.ApprovedForReceiving.Checked = Tristate.False;
                    MaterialNode.postChanges( true );
                }
            }
        }//Update()
    }//class CswUpdateSchemaCase_01W_28807
}//namespace ChemSW.Nbt.Schema