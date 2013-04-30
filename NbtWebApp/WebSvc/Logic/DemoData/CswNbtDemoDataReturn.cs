using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Grid.ExtJs;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    [DataContract]
    public class CswNbtDemoDataReturn : CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtDemoDataReturn()
        {
            Data = new DemoData();
        }//ctor


        [DataContract]
        public sealed class ColumnNames
        {
            public const string NodeId = "nodeId";
            public const string Name = "Name";
            public const string Type = "Type";
            public const string IsUsedBy = "Is Used By";
            public const string IsRequiredBy = "Is Required By";
            public const string Delete = "Delete";
            public const string ConvertToNonDemo = "Convert To Non Demo";
            public const string MenuOptions = "menuoptions";
            public const string IsDemo = "Is Demo";
            public const string Action = "Action";

        }//ColumnNames

        [DataContract]
        public class DemoData
        {
            public DemoData()
            {
                Grid = new CswExtJsGrid( GridPrefix );
            }

            [DataMember]
            public const string GridPrefix = "DemoData";

            [DataMember]
            public CswExtJsGrid Grid;

            [DataMember]
            public CswDictionary ColumnIds
            {
                get
                {
                    CswDictionary Ret = new CswDictionary();

                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.NodeId ).ToString(), ColumnNames.NodeId );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.Name ).ToString(), ColumnNames.Name );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.Type ).ToString(), ColumnNames.Type );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.IsUsedBy ).ToString(), ColumnNames.IsUsedBy );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.IsRequiredBy ).ToString(), ColumnNames.IsRequiredBy );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.Delete ).ToString(), ColumnNames.Delete  );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.ConvertToNonDemo ).ToString(), ColumnNames.ConvertToNonDemo );

                    return Ret;
                }
                set { var disposable = value; }
            }
        }//


        [DataMember]
        public DemoData Data;

    }//CswNbtDemoDataReturn




} // namespace ChemSW.Nbt.WebServices
