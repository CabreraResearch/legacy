using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31210 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31210; }
        }

        public override string Title
        {
            get { return "Remove ChemWatch property from all layouts"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            // Remove the ChemWatch property from all layouts
            CswNbtMetaDataObjectClass SDSDocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
            foreach( CswNbtMetaDataNodeType SDSDocNT in SDSDocumentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ChemWatchNTP = SDSDocNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.ChemWatch );
                ChemWatchNTP.removeFromAllLayouts();
                ChemWatchNTP.ReadOnly = true;
                ChemWatchNTP.Hidden = true;
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema