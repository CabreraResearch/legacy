using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52446 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 52446; }
        }

        public override string Title
        {
            get { return "Remove default value from Specific Gravity and Physical State"; }
        }

        public override void update()
        {
            // Remove default value from Specific Gravity and Physical State
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp SpecificGravityNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.SpecificGravity );
                SpecificGravityNTP.getDefaultValue( false ).ClearValue();
                SpecificGravityNTP.updateLayout( CswEnumNbtLayoutType.Add, true, Int32.MinValue, 5, 1 );

                CswNbtMetaDataNodeTypeProp PhysicalStateNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.PhysicalState );
                PhysicalStateNTP.getDefaultValue( false ).ClearValue();
                PhysicalStateNTP.updateLayout( CswEnumNbtLayoutType.Add, true, Int32.MinValue, 3, 1 );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema