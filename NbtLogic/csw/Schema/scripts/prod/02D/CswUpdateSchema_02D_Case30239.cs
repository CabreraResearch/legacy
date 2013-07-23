using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02D_Case30239 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30239; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerLocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            foreach( CswNbtObjClassContainerLocation ContLocNode in ContainerLocationOC.getNodes( false, false, false, true ) )
            {
                if( ContLocNode.Action.Value == "No Action" )
                {
                    ContLocNode.Action.Value = CswEnumNbtContainerLocationActionOptions.Ignore.ToString();
                    ContLocNode.postChanges( false );
                }
            }
        } // update()
    }
}//namespace ChemSW.Nbt.Schema