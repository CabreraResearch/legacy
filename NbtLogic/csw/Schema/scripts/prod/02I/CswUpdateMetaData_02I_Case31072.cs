using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31072 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31072; }
        }

        public override string Title
        {
            get { return "Make UnitOfMeasure Name Readonly"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp NameOCP = UoMOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOCP, CswEnumNbtObjectClassPropAttributes.readOnly, true );
            CswNbtMetaDataObjectClassProp BaseUnitOCP = UoMOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.BaseUnit );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( BaseUnitOCP, CswEnumNbtObjectClassPropAttributes.servermanaged, true );
            foreach( CswNbtMetaDataNodeType UoMNT in UoMOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp NameNTP = UoMNT.getNodeTypePropByObjectClassProp( NameOCP );
                NameNTP.ReadOnly = true;
                CswNbtMetaDataNodeTypeProp BaseUnitNTP = UoMNT.getNodeTypePropByObjectClassProp( BaseUnitOCP );
                BaseUnitNTP.ServerManaged = true;
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema