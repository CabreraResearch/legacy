using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30894 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30894; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Fix container add layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.clearLayout( CswEnumNbtLayoutType.Add, ContainerNT.NodeTypeId );

                CswNbtMetaDataNodeTypeProp OwnerNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner );
                CswNbtMetaDataNodeTypeProp LocationNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
                CswNbtMetaDataNodeTypeProp LabelFormatNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.LabelFormat );
                CswNbtMetaDataNodeTypeProp ExpirationDateNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );

                OwnerNTP.updateLayout( CswEnumNbtLayoutType.Add, false, DisplayRow: 1, DisplayColumn: 1 );
                LocationNTP.updateLayout( CswEnumNbtLayoutType.Add, false, DisplayRow: 2, DisplayColumn: 1 );
                LabelFormatNTP.updateLayout( CswEnumNbtLayoutType.Add, false, DisplayRow: 3, DisplayColumn: 1 );
                ExpirationDateNTP.updateLayout( CswEnumNbtLayoutType.Add, false, DisplayRow: 4, DisplayColumn: 1 );
            }
        } // update()

    } // class CswUpdateSchema_02G_Case30894
}//namespace ChemSW.Nbt.Schema