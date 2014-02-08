using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.WebServices;
using NbtWebApp.Actions.Receiving;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtWebServiceReceiving
    {
        [DataContract]
        public class CswNbtReceivingDefinitionReturn: CswWebSvcReturn
        {
            public CswNbtReceivingDefinitionReturn()
            {
                Data = new CswNbtReceivingDefinition();
            }
            [DataMember]
            public CswNbtReceivingDefinition Data;
        }

        public static void ReceiveMaterial( ICswResources CswResources, CswNbtReceivingDefinitionReturn Response, CswNbtReceivingDefinition ReceivingDefiniton )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtActReceiving ActReceiving = new CswNbtActReceiving( NbtResources, ReceivingDefiniton.MaterialNodeId );
            JObject ActionData = ActReceiving.receiveMaterial( ReceivingDefiniton );
            Response.Data.ActionData = ActionData.ToString();


            //Create Print Job
            if( null != ReceivingDefiniton.PrintLabelId && null != ReceivingDefiniton.PrinterNodeId )
            {
                CswNbtObjClassContainer InitialContainer = NbtResources.Nodes.GetNode( ReceivingDefiniton.ContainerNodeId );

                Collection<Dictionary<string, string>> PropVals = new Collection<Dictionary<string, string>>();
                foreach( CswNbtAmountsGridQuantity Quant in ReceivingDefiniton.Quantities )
                {
                    CswNbtObjClassUnitOfMeasure UoMNode = NbtResources.Nodes.GetNode( Quant.UnitNodeId );
                    for( int i = 0; i < Quant.NumContainers; i++ )
                    {
                        Dictionary<string, string> vals = InitialContainer.Node.getPropertiesAndValues();
                        vals[InitialContainer.Barcode.PropName] = Quant.getBarcodes()[i];
                        vals[InitialContainer.Quantity.PropName] = Quant.Quantity + " " + UoMNode.BaseUnit.Text;
                        PropVals.Add( vals );
                    }
                }

                CswNbtWebServicePrintLabels.newPrintJob( CswResources, ReceivingDefiniton.PrinterNodeId, ReceivingDefiniton.PrintLabelId, ReceivingDefiniton.ContainerNodeId, PropVals );
            }
        }

    }
}
