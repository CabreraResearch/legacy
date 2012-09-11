using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27703
    /// </summary>
    public class CswUpdateSchemaCase27703 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataObjectClassProp nameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( requestItemOC )
            {
                PropName = "Name",
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                ServerManaged = true
            } );

            string newNameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRequestItem.PropertyName.Name );
            foreach( CswNbtMetaDataNodeType requestItemNT in requestItemOC.getNodeTypes() )
            {
                requestItemNT.setNameTemplateText( newNameTemplate );
                CswNbtMetaDataNodeTypeProp nameNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( requestItemNT.NodeTypeId, nameOCP.ObjectClassPropId );
                nameNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema