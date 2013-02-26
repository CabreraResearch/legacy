using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28916
    /// </summary>
    public class CswUpdateSchema_01Y_Case28916 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28916; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.DocumentClass );
            CswNbtMetaDataNodeType SDSNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( DocumentOC )
            {
                NodeTypeName = "SDS Document",
                Category = "Materials"
            } );
            CswNbtMetaDataNodeTypeProp TradeNameNTP = SDSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                TradeNameNTP.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), ChemicalNT.NodeTypeId );
            }
            TradeNameNTP.PropName = "Tradename";
            CswNbtMetaDataNodeTypeProp RevisionDateNTP =
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp(
                    SDSNT,
                    _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.DateTime ),
                    "Revision Date" )
                );
            CswNbtMetaDataNodeTypeProp LanguageNTP = SDSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
            LanguageNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, SDSNT.getFirstNodeTypeTab().TabId );
            CswNbtMetaDataNodeTypeProp FormatNTP = SDSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
            FormatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, SDSNT.getFirstNodeTypeTab().TabId );
            SDSNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDocument.PropertyName.Title ) );
            //Part 3 - remove docclass (OCP), language (OCP), and format(OCP), revision date (NTP) from all non-SDS types
        } //Update()
    }//class CswUpdateSchema_01Y_Case28916
}//namespace ChemSW.Nbt.Schema