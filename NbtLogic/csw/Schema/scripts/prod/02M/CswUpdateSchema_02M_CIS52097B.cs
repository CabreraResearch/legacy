using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateSchema_02M_CIS52097B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52097; }
        }

        public override string Title
        {
            get { return "Create a Containers tab displaying Containers Grid Prop in the Receipt Lot NT" + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );

            foreach( CswNbtMetaDataNodeType thisReceiptLotNT in ReceiptLotOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab ContainerTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( thisReceiptLotNT, "Containers", 2 );
                CswNbtMetaDataNodeTypeProp ContainerProp = thisReceiptLotNT.getNodeTypePropByObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.Containers );
                ContainerProp.updateLayout(CswEnumNbtLayoutType.Edit, true, ContainerTab.TabId, 1,1 );
                
            }

        }
    }
}


