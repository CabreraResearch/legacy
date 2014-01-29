using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case31329A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31329; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override string Title
        {
            get { return "Add 'Cache Size' property to Sequences"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass DesignSequenceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );

            CswNbtMetaDataObjectClassProp CacheSizeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( DesignSequenceOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DesignSequenceOC )
                {
                    PropName = CswNbtObjClassDesignSequence.PropertyName.CacheSize,
                    FieldType = CswEnumNbtFieldType.Number,
                    IsRequired = true
                } );
            
        } // update()

    }//class CswUpdateMetaData_02K_Case31672

}//namespace ChemSW.Nbt.Schema