using ChemSW.Nbt.csw.Dev;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27912
    /// </summary>
    public class CswUpdateSchema_01T_Case27912 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27912; }
        }

        public override void update()
        {
            //add Inspection Design Status prop to the table layout
            CswNbtMetaDataObjectClass inspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
            foreach( CswNbtMetaDataNodeType inspectionNT in inspectionDesignOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp statusNTP = inspectionNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status );
                statusNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, null, false );
            }

        }

        //Update()

    }//class CswUpdateSchemaCase27912

}//namespace ChemSW.Nbt.Schema