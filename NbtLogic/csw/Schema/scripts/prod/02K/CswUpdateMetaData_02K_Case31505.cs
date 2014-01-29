using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02K_Case31505 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31505; }
        }

        public override string Title
        {
            get { return "Update Container Location Type Options"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContLocOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClassProp TypeOCP = ContLocOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Type );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOCP, CswEnumNbtObjectClassPropAttributes.listoptions, 
                new CswCommaDelimitedString
                    {
                        CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString(),
                        CswEnumNbtContainerLocationTypeOptions.Receipt.ToString(),
                        CswEnumNbtContainerLocationTypeOptions.Move.ToString(),
                        CswEnumNbtContainerLocationTypeOptions.Dispense.ToString(),
                        CswEnumNbtContainerLocationTypeOptions.Dispose.ToString(),
                        CswEnumNbtContainerLocationTypeOptions.Undispose.ToString(),
                        CswEnumNbtContainerLocationTypeOptions.Missing.ToString(),
                        CswEnumNbtContainerLocationTypeOptions.Ignore.ToString()
                    }.ToString() );
        }
    }
}