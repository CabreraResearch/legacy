using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31072: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31072; }
        }

        public override string AppendToScriptName()
        {
            return "BDC";
        }

        public override string Title
        {
            get { return "Make special UnitsOfMeasure names readonly"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            foreach( CswNbtObjClassUnitOfMeasure UoMNode in UoMOC.getNodes( false, false ) )
            {
                if( UoMNode.Name.Text == UoMNode.BaseUnit.Text || UoMNode.Name.Text == "lb" || UoMNode.Name.Text == "gal" || UoMNode.Name.Text == "cu.ft." )
                {
                    UoMNode.Name.setReadOnly( true, true );
                    UoMNode.postChanges( true );
                }
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema