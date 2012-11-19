using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case27947
    /// </summary>
    public class CswUpdateSchema_0T1_Case27947_part2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp statusOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( statusOCP, CswNbtObjClassContainer.Statuses.LabUseOnly );

            foreach( CswNbtObjClassContainer containerNode in containerOC.getNodes( false, false, false, true ) )
            {
                if( String.IsNullOrEmpty( containerNode.Status.Value ) )
                {
                    containerNode.Status.Value = CswNbtObjClassContainer.Statuses.LabUseOnly;
                    containerNode.postChanges( false );
                }
            }
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27947; }
        }

        //Update()

    }//class CswUpdateSchemaCaseCswUpdateSchema_01T_Case27947

}//namespace ChemSW.Nbt.Schema