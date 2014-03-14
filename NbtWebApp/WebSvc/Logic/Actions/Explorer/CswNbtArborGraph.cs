using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.Actions.Explorer
{
    [DataContract]
    public class CswNbtExplorerRequest
    {
        [DataMember]
        public string NodeId { get; set; }

        [DataMember]
        public int Depth { get; set; }

        [DataMember]
        public string FilterVal { get; set; }
    }

    [DataContract]
    public class CswNbtExplorerReturn: CswWebSvcReturn
    {
        public CswNbtExplorerReturn()
        {
            Data = new CswNbtArborGraph();
        }

        [DataMember]
        public CswNbtArborGraph Data;
    }

    [DataContract]
    public class CswNbtArborGraph: CswWebSvcReturn
    {
        [DataMember]
        public string FilterVal = string.Empty;

        [DataMember]
        public Collection<ArborFilterOpt> Opts = new Collection<ArborFilterOpt>();

        [DataMember]
        public Dictionary<string, Collection<string>> Graph = new Dictionary<string, Collection<string>>();

        [DataMember]
        public Collection<CswNbtArborNode> Nodes = new Collection<CswNbtArborNode>();

        [DataMember]
        public Collection<CswNbtArborEdge> Edges = new Collection<CswNbtArborEdge>();

        public static string _setDefaultFilterVal( CswNbtResources NbtResources )
        {
            CswCommaDelimitedString Ret = new CswCommaDelimitedString();

            _populate( NbtResources, CswEnumNbtObjectClass.RoleClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.UserClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.WorkUnitClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.ChemicalClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.NonChemicalClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.ContainerClass, Ret );
            //_populate( NbtResources, CswEnumNbtObjectClass.LocationClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.EquipmentAssemblyClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.EquipmentClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.EquipmentTypeClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.FeedbackClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.SDSDocumentClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.CofADocumentClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.DocumentClass, Ret );
            _populate( NbtResources, CswEnumNbtObjectClass.VendorClass, Ret );

            return Ret.ToString();
        }

        private static void _populate( CswNbtResources NbtResources, CswEnumNbtObjectClass ObjectClass, CswCommaDelimitedString Ret )
        {
            CswNbtMetaDataObjectClass ObjClass = NbtResources.MetaData.getObjectClass( ObjectClass );
            if( null != ObjClass ) //Module permissions might cause this to be null
            {
                foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                {
                    Ret.Add( "NT_" + NodeType.NodeTypeId.ToString() );
                }

                Ret.Add( "OC_" + ObjClass.ObjectClassId );
            }
        }
    }

    [DataContract]
    public class ArborFilterOpt
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public bool selected { get; set; }
    }


}