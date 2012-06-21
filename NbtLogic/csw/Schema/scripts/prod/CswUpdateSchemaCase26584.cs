using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26584
    /// </summary>
    public class CswUpdateSchemaCase26584 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Inspection Design Status property is no longer servermanaged

            CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp StatusOCP = InspectionOC.getObjectClassProp( CswNbtObjClassContainer.StatusPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                            StatusOCP,
                            CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged,
                            CswConvert.ToDbVal( false ) );

        }//Update()

    }//class CswUpdateSchemaCase26584

}//namespace ChemSW.Nbt.Schema