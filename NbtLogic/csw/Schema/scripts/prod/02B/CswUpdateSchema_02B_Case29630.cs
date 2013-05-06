using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29630
    /// </summary>
    public class CswUpdateSchema_02B_Case29630: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29630; }
        }

        public override void update()
        {
            //Add an image prop to Inspection Designs
            CswNbtMetaDataObjectClass inspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
            foreach( CswNbtMetaDataNodeType inspectionDesignNT in inspectionDesignOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab picturesTab = inspectionDesignNT.getNodeTypeTab( "Pictures" ) ??
                    _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( inspectionDesignNT, "Pictures", inspectionDesignNT.getMaximumTabOrder() + 1 );

                CswNbtMetaDataNodeTypeProp picturesProp = inspectionDesignNT.getNodeTypeProp( CswNbtObjClassInspectionDesign.PropertyName.Pictures );
                if( null == picturesProp )
                {
                    picturesProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( inspectionDesignNT, CswEnumNbtFieldType.Image, CswNbtObjClassInspectionDesign.PropertyName.Pictures, picturesTab.TabId );
                }
                else
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, inspectionDesignNT.NodeTypeId, picturesProp, true, picturesTab.TabId );
                }

                picturesProp.MaxValue = 15;
            }

        } // update()

    }//class CswUpdateSchema_02B_Case29630

}//namespace ChemSW.Nbt.Schema