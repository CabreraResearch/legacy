using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30849 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {                
            get { return 30849; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Unset Hidden on Undisposed Prop of Containers"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            if( null != ContainerOC )
            {
                foreach( CswNbtObjClassContainer Container in ContainerOC.getNodes( false, false, false, true ) )
                {
                    Container.Undispose.setHidden( value: false, SaveToDb: true );
                    Container.postChanges( false );
                }
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema