using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Services;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceProperties
    {

        #region Data Contracts

        [DataContract]
        public class CswNbtButtonData
        {
            [DataMember]
            public string Mode = "button"; //default is button

            [DataMember]
            public Collection<string> Opts = new Collection<string>();
        }

        #endregion

        public static void GetButtonMode( ICswResources Resources, CswNbtPropertyReturn Return, CswPropIdAttr PropId )
        {
            CswNbtResources NbtResources = (CswNbtResources) Resources;
            CswNbtMetaDataNodeTypeProp BtnNTP = NbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            CswNbtNode Node = NbtResources.Nodes.GetNode( PropId.NodeId );
            CswNbtNodePropButton btnProp = Node.Properties[BtnNTP].AsButton;
            if( false == string.IsNullOrEmpty( btnProp.Mode ) )
            {
                Return.Data.Mode = Node.Properties[BtnNTP].AsButton.Mode;
            }
        }

        public static void GetButtonOpts( ICswResources Resources, CswNbtPropertyReturn Return, CswPropIdAttr PropId )
        {
            CswNbtResources NbtResources = (CswNbtResources) Resources;
            CswNbtMetaDataNodeTypeProp BtnNTP = NbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            CswNbtNode Node = NbtResources.Nodes.GetNode( PropId.NodeId );
            CswCommaDelimitedString MenuOpts = new CswCommaDelimitedString();
            MenuOpts.FromString( Node.Properties[BtnNTP].AsButton.MenuOptions );
            foreach( string Opt in MenuOpts )
            {
                Return.Data.Opts.Add( Opt );
            }
        }

    }
}