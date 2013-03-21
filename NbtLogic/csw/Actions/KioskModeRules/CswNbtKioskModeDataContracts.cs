using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Actions.KioskMode
{

    public sealed class Modes
    {
        public const string Move = "move";
        public const string Owner = "owner";
        public const string Transfer = "transfer";
        public const string Dispense = "dispense";
        public const string Dispose = "dispose";
        public const string Status = "status";
        public static readonly CswCommaDelimitedString All = new CswCommaDelimitedString()
            {
                Move, Owner, Transfer, Dispense, Dispose, Status
            };
    }

    [DataContract]
    public class KioskModeData
    {
        [DataMember]
        public Collection<Mode> AvailableModes = new Collection<Mode>();
        [DataMember]
        public OperationData OperationData;
    }

    [DataContract]
    public class Mode
    {
        [DataMember]
        public string name = string.Empty;
        [DataMember]
        public string imgUrl = string.Empty;
    }

    [DataContract]
    public class ActiveMode
    {
        [DataMember]
        public string field1Name = string.Empty;
        [DataMember]
        public string field2Name = string.Empty;
    }

    [DataContract]
    public class OperationData
    {
        [DataMember]
        public string Mode = string.Empty;
        [DataMember]
        public string ModeStatusMsg = string.Empty;
        [DataMember]
        public bool ModeServerValidated = false;
        [DataMember]
        public Collection<string> Log = new Collection<string>();
        [DataMember]
        public Field Field1;
        [DataMember]
        public Field Field2;
        [DataMember]
        public string LastItemScanned;
        [DataMember]
        public string ScanTextLabel;
    }

    [DataContract]
    public class Field
    {
        [DataMember]
        public string Name = string.Empty;
        [DataMember]
        public string Value = string.Empty;
        [DataMember]
        public string StatusMsg = string.Empty;
        [DataMember]
        public bool ServerValidated = false;
        [DataMember]
        public string SecondValue = string.Empty;
        [DataMember]
        public string FoundObjClass;

        public NbtObjectClass GetBarcodeExpectedOC( CswNbtResources NbtResources, ref OperationData OpData )
        {
            NbtObjectClass Ret = null;

            //CswNbtWebServiceSearch searchSvc = new CswNbtWebServiceSearch( NbtResources, null );
            //CswNbtSearch search = searchSvc.getSearch( Value, Int32.MinValue, Int32.MinValue );
            //ICswNbtTree tree = search.Results();
            //int childCount = tree.getChildNodeCount();
            //for( int i = 0; i < childCount; i++ )
            //{
            //    tree.goToNthChild( i );
            //    CswNbtNode node = tree.getNodeForCurrentPosition();
            //    string ObjClass = node.ObjClass.ObjectClass.ObjectClass;
            //
            //    if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            //    {
            //        if( ObjClass == NbtObjectClass.ContainerClass )
            //        {
            //            Ret = NbtObjectClass.ContainerClass;
            //            FoundObjClass = Ret;
            //        }
            //    }
            //
            //    if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
            //    {
            //        if( ObjClass == NbtObjectClass.EquipmentAssemblyClass )
            //        {
            //            Ret = NbtObjectClass.EquipmentAssemblyClass;
            //            FoundObjClass = Ret;
            //        }
            //
            //        if( ObjClass == NbtObjectClass.EquipmentClass )
            //        {
            //            Ret = NbtObjectClass.EquipmentClass;
            //            FoundObjClass = Ret;
            //        }
            //    }
            //    tree.goToParentNode();
            //}
            //
            //if( null == Ret ) //Now is a good time to validate the field since we know what we just searched for
            //{
            //    bool first = true;
            //    if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            //    {
            //        StatusMsg = NbtObjectClass.ContainerClass.Replace( "Class", "" );
            //        first = false;
            //    }
            //    if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
            //    {
            //        if( first )
            //        {
            //            StatusMsg = NbtObjectClass.EquipmentClass.Replace( "Class", "" );
            //        }
            //        else
            //        {
            //            StatusMsg += ", " + NbtObjectClass.EquipmentClass.Replace( "Class", "" );
            //        }
            //        StatusMsg += " or " + NbtObjectClass.EquipmentAssemblyClass.Replace( "Class", "" );
            //    }
            //    StatusMsg = "Could not find " + StatusMsg + " with barcode " + Value;
            //
            //    OpData.Field2.StatusMsg = StatusMsg;
            //    OpData.Field2.ServerValidated = false;
            //    OpData.Log.Add( DateTime.Now + " - ERROR: " + StatusMsg );
            //}

            return Ret;
        }

        public NbtObjectClass ExpectedObjClass( CswNbtResources NbtResources, int FieldNumber, ref OperationData OpData )
        {
            NbtObjectClass Ret = null;
            string OpMode = OpData.Mode.ToLower();
            switch( OpMode )
            {
                case Modes.Move:
                    if( FieldNumber == 1 )
                    {
                        Ret = NbtObjectClass.LocationClass;
                    }
                    else
                    {
                        Ret = GetBarcodeExpectedOC( NbtResources, ref OpData );
                    }
                    break;
                case Modes.Owner:
                    if( FieldNumber == 1 )
                    {
                        Ret = NbtObjectClass.UserClass;
                    }
                    else
                    {
                        Ret = NbtObjectClass.ContainerClass;
                    }
                    break;
                case Modes.Transfer:
                    if( FieldNumber == 1 )
                    {
                        Ret = NbtObjectClass.UserClass;
                    }
                    else
                    {
                        Ret = NbtObjectClass.ContainerClass;
                    }
                    break;
                case Modes.Dispense:
                    if( FieldNumber == 1 )
                    {
                        Ret = NbtObjectClass.ContainerClass;
                    }
                    break;
                case Modes.Dispose:
                    Ret = NbtObjectClass.ContainerClass;
                    break;
                case Modes.Status:
                    if( FieldNumber == 2 )
                    {
                        Ret = GetBarcodeExpectedOC( NbtResources, ref OpData );
                    }
                    break;
            }
            return Ret;
        }
    }
}
