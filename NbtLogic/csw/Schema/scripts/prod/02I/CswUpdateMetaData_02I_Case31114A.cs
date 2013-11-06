using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31114A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31114; }
        }

        public override string Title
        {
            get { return "Add ChemWatch Module, Link ChemWatch button"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            if( Int32.MinValue == _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.ChemWatch ) )
            {
                _CswNbtSchemaModTrnsctn.createModule( "ChemWatch", CswEnumNbtModuleName.ChemWatch.ToString() );
            }

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.LinkChemWatch,
                    FieldType = CswEnumNbtFieldType.Button
                } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema