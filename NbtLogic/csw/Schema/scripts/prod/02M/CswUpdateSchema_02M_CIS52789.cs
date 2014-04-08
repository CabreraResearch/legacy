using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52789 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52789; }
        }

        public override string Title
        {
            get { return "Constituents: REMOVE ALL THE THINGS!"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        private CswNbtMetaDataNodeType ConstituentNT;

        public override void update()
        {
            ConstituentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Constituent" );
            if( null != ConstituentNT )
            {
                _removeFromConstituentLayout( CswNbtObjClassChemical.PropertyName.Receive );
                _removeFromConstituentLayout( CswNbtObjClassChemical.PropertyName.OpenExpireInterval );
                _removeFromConstituentLayout( CswNbtObjClassChemical.PropertyName.ApprovedForReceiving );
                _removeFromConstituentLayout( CswNbtObjClassChemical.PropertyName.ContainerExpirationLocked );
                _removeFromConstituentLayout( CswNbtObjClassChemical.PropertyName.ViewSDS );
                _removeFromConstituentLayout( CswNbtObjClassChemical.PropertyName.Request );
            }
        }

        private void _removeFromConstituentLayout( string NTPName )
        {
            CswNbtMetaDataNodeTypeProp NTP = ConstituentNT.getNodeTypePropByObjectClassProp( NTPName );
            NTP.removeFromAllLayouts();
        }
    }
}