using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26575
    /// </summary>
    public class CswUpdateSchemaCase26575 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // mark as server-managed the containerclass objectclassprops:
            //   material (relationship)
            //   size (relationship) 

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp ContainerMaterialOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.MaterialPropertyName );
            CswNbtMetaDataObjectClassProp ContainerSizeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.SizePropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                            ContainerMaterialOCP,
                            CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged,
                            CswConvert.ToDbVal( true ) );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                            ContainerSizeOCP,
                            CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged,
                            CswConvert.ToDbVal( true ) );

        }//Update()

    }//class CswUpdateSchemaCase26575

}//namespace ChemSW.Nbt.Schema