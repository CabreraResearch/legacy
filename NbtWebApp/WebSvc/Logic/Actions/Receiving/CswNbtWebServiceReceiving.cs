using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
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
        public class CswNbtContainerBarcodeCheckReturn: CswWebSvcReturn
        {
            public CswNbtContainerBarcodeCheckReturn()
            {
                Data = string.Empty;
            }
            [DataMember]
            public string Data;
        }

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

        public static void CheckContainerBarcodes( ICswResources CswResources, CswNbtContainerBarcodeCheckReturn ErrorMsg, Collection<CswNbtAmountsGridQuantity> Quantities )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtMetaDataObjectClass ContainerOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp BarcodeOCP = (CswNbtMetaDataObjectClassProp) ContainerOC.getBarcodeProperty();
            CswCommaDelimitedString barcodeNTPs = new CswCommaDelimitedString();
            foreach( CswNbtMetaDataNodeTypeProp barcodeNTP in BarcodeOCP.getNodeTypeProps() )
            {
                barcodeNTPs.Add( barcodeNTP.PropId.ToString() );
            }
            CswNbtFieldTypeRuleBarCode BarcodeFTR = (CswNbtFieldTypeRuleBarCode) BarcodeOCP.getFieldTypeRule();

            CswCommaDelimitedString AllProposedBarcodes = new CswCommaDelimitedString();
            HashSet<string> DuplicateBarcodes = new HashSet<string>();
            HashSet<string> BarcodeLookup = new HashSet<string>();
            foreach( CswNbtAmountsGridQuantity quantity in Quantities )
            {
                CswCommaDelimitedString barcodes = quantity.getBarcodes();
                foreach( string barcode in barcodes )
                {
                    AllProposedBarcodes.Add( "'" + barcode.ToLower() + "'" );
                    if( BarcodeLookup.Contains( barcode ) )
                    {
                        DuplicateBarcodes.Add( barcode );
                    }
                    else
                    {
                        BarcodeLookup.Add( barcode );
                    }
                }
            }

            if( AllProposedBarcodes.Count > 0 )
            {
                if( DuplicateBarcodes.Count > 0 )
                {
                    CswCommaDelimitedString dupeCDS = HashSetToCDS( DuplicateBarcodes );
                    ErrorMsg.Data = "There are following barcodes appear more than once: " + dupeCDS;
                }
                else
                {
                    string barcodeSQL = @"select nodeid, field1 from jct_nodes_props where nodetypepropid in (" + barcodeNTPs + ") and lower(" + BarcodeFTR.BarcodeSubField.Column + ") in (" + AllProposedBarcodes + ")";
                    DataTable barcodesTbl = NbtResources.execArbitraryPlatformNeutralSqlSelect( "WebServiceReceiving.CheckContainerBarcodes", barcodeSQL );
                    if( barcodesTbl.Rows.Count > 0 )
                    {
                        foreach( DataRow row in barcodesTbl.Rows )
                        {
                            DuplicateBarcodes.Add( row[BarcodeFTR.BarcodeSubField.Column.ToString()].ToString() );
                        }
                        CswCommaDelimitedString dupeCDS = HashSetToCDS( DuplicateBarcodes );
                        ErrorMsg.Data = "There are following barcodes have already been assigned: " + dupeCDS;
                    }
                }
            }
        }

        private static CswCommaDelimitedString HashSetToCDS( HashSet<string> set )
        {
            string[] dupeArray = new string[set.Count];
            set.CopyTo( dupeArray );
            CswCommaDelimitedString cds = new CswCommaDelimitedString()
                    {
                        dupeArray
                    };
            return cds;
        }

    }
}
