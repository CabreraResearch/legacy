using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27629
    /// </summary>
    public class CswUpdateSchemaCase27629 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataObjectClass problemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass );
            CswNbtMetaDataObjectClassProp closedOCP = problemOC.getObjectClassProp( CswNbtObjClassProblem.PropertyName.Closed );
            foreach( CswNbtMetaDataNodeType problemNT in problemOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp closedNTP = problemNT.getNodeTypePropByObjectClassProp( closedOCP.ObjectClassPropId );
                closedNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema