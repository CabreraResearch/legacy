using System.Linq;
using ChemSW.Config;
using ChemSW.Log;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24508D
    /// </summary>
    public class CswUpdateSchemaCase24508D : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContDispTransOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataObjectClassProp DispensedDate = ContDispTransOc.getObjectClassProp( CswNbtObjClassContainerDispenseTransaction.DispensedDatePropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                DispensedDate,
                CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended,
                ChemSW.Nbt.PropTypes.CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString()
            );

            CswNbtMetaDataNodeType ContDispTransNt = ContDispTransOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null == ContDispTransNt )
            {
                CswStatusMessage Msg = new CswStatusMessage
                {
                    AppType = AppType.SchemUpdt,
                    ContentType = ContentType.Error
                };
                Msg.Attributes.Add( Log.LegalAttribute.escoteric_message, "Could not get a Container Dispense Transaction NodeType" );
                _CswNbtSchemaModTrnsctn.CswLogger.send( Msg );
            }
            else
            {
                CswNbtView GridView = _CswNbtSchemaModTrnsctn.restoreView( "Container Container Dispense Transactions Grid Property View" );
                if( GridView != null )
                {
                    GridView.ViewName = "Container Dispense Transactions";
                    GridView.save();
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase24508D

}//namespace ChemSW.Nbt.Schema