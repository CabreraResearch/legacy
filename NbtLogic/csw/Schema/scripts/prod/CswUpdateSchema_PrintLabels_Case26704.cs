using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for PrintLabels_Case26704
    /// </summary>
    public class CswUpdateSchema_PrintLabels_Case26704 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp ControlTypeOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PrintLabelOc )
                {
                    PropName = CswNbtObjClassPrintLabel.PropertyName.ControlType,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                    ListOptions = CswNbtObjClassPrintLabel.ControlTypes.Options.ToString(),
                    IsRequired = true
                } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ControlTypeOcp, ControlTypeOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtObjClassPrintLabel.ControlTypes.jZebra );

            foreach( CswNbtObjClassPrintLabel PrintLabel in PrintLabelOc.getNodes( forceReInit: true, includeSystemNodes: false ) )
            {
                if( null != PrintLabel )
                {
                    PrintLabel.ControlType.Value = CswNbtObjClassPrintLabel.ControlTypes.jZebra;
                    PrintLabel.postChanges( ForceUpdate: false );
                }
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema