using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case29446B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29446; }
        }

        public override string Title
        {
            get { return "Make Container Opened Date read only"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp OpenedDateOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.OpenedDate );
            foreach( CswNbtMetaDataNodeTypeProp OpenedDateNTP in OpenedDateOCP.getNodeTypeProps() )
            {
                OpenedDateNTP.DesignNode.ReadOnly.Checked = CswEnumTristate.True;
                OpenedDateNTP.DesignNode.postChanges( false );
                CswNbtMetaDataNodeType nt = OpenedDateNTP.getNodeType();
                OpenedDateNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, nt.getFirstNodeTypeTab().TabId );
            }

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp OpenExpireIntervalOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.OpenExpireInterval );
            foreach( CswNbtMetaDataNodeTypeProp OpenExpireIntervalNTP in OpenExpireIntervalOCP.getNodeTypeProps() )
            {
                CswNbtMetaDataNodeType nt = OpenExpireIntervalNTP.getNodeType();
                OpenExpireIntervalNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, nt.getFirstNodeTypeTab().TabId );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema