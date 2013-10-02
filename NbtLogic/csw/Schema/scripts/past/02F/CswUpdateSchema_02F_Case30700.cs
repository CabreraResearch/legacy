using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30700: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30700; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30700"; }
        }

        public override void update()
        {

            //Make all existing Roles timeout prop a max of 90
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true, false, true ) )
            {
                if( RoleNode.Timeout.Value > 90 )
                {
                    RoleNode.Timeout.Value = 90;
                    RoleNode.postChanges( false );
                }
            }

            //Make Role.Timeout property Max Value 90 minutes
            foreach( CswNbtMetaDataNodeType RoleNT in RoleOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp TimeoutNTP = RoleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRole.PropertyName.Timeout );
                TimeoutNTP.MaxValue = 90;
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema