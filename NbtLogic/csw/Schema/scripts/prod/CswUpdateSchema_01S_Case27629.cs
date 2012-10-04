using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27629
    /// </summary>
    public class CswUpdateSchema_01S_Case27629 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataObjectClass problemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.ProblemClass );
            CswNbtMetaDataObjectClassProp closedOCP = problemOC.getObjectClassProp( CswNbtObjClassProblem.PropertyName.Closed );
            foreach( CswNbtMetaDataNodeType problemNT in problemOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp closedNTP = problemNT.getNodeTypePropByObjectClassProp( closedOCP.ObjectClassPropId );
                closedNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27629; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema