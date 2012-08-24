
using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27435
    /// </summary>
    public class CswUpdateSchema_MSDS_Case27435 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass DocumentOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass );
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass );
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass );

            Collection<Int32> MaterialNtIds = MaterialOc.getNodeTypeIds();
            Collection<Int32> ContainerNtIds = ContainerOc.getNodeTypeIds();
            foreach( CswNbtMetaDataNodeType DocumentNt in DocumentOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp OwnerNtp = DocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
                CswNbtMetaDataNodeTypeProp DocumentClassNtp = DocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );
                if( ( OwnerNtp.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() && MaterialNtIds.Contains( OwnerNtp.FKValue ) ) ||
                    ( OwnerNtp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() && OwnerNtp.FKValue == MaterialOc.ObjectClassId ) )
                {
                    DocumentClassNtp.DefaultValue.AsList.Value = CswNbtObjClassDocument.DocumentClasses.MSDS;
                }
                else if( ( OwnerNtp.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() && ContainerNtIds.Contains( OwnerNtp.FKValue ) ) ||
                         ( OwnerNtp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() && OwnerNtp.FKValue == ContainerOc.ObjectClassId ) )
                {
                    DocumentClassNtp.DefaultValue.AsList.Value = CswNbtObjClassDocument.DocumentClasses.CofA;
                }
            }


        }//Update()

    }

}//namespace ChemSW.Nbt.Schema