using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Sched
{
    public sealed class CswEnumNbtScheduleRuleNames : IEquatable<CswEnumNbtScheduleRuleNames>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { UpdtPropVals                  , UpdtPropVals                   },
                                                                       { UpdtMTBF                      , UpdtMTBF                       },
                                                                       { UpdtInspection                , UpdtInspection                 },
                                                                       { GenNode                       , GenNode                        },
                                                                       { GenRequest                    , GenRequest                     },
                                                                       { GenEmailRpt                   , GenEmailRpt                    },
                                                                       { DisableChemSwAdmin            , DisableChemSwAdmin             },
                                                                       { BatchOp                       , BatchOp                        },
                                                                       { ExpiredContainers             , ExpiredContainers              },
                                                                       { MolFingerprints               , MolFingerprints                },
                                                                       { ContainerReconciliationActions, ContainerReconciliationActions },
                                                                       { Reconciliation                , Reconciliation                 },
                                                                       { TierII                        , TierII                         },
                                                                       { CAFImport                     , CAFImport                      },
                                                                       { NodeCounts                    , NodeCounts                     }
                                                                   };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        private static string _Parse( string Val )
        {
            string ret = CswResources.UnknownEnum;
            if( _Enums.ContainsKey( Val ) )
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// The enum constructor
        /// </summary>
        public CswEnumNbtScheduleRuleNames( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtScheduleRuleNames( string Val )
        {
            return new CswEnumNbtScheduleRuleNames( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtScheduleRuleNames item )
        {
            return item.Value;
        }

        /// <summary>
        /// Override of ToString
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        #endregion Internals

        #region Enum members


        public const string UpdtPropVals = "UpdtPropVals";
        public const string UpdtMTBF = "UpdtMTBF";
        public const string UpdtInspection = "UpdtInspection";
        public const string GenNode = "GenNode";
        public const string GenRequest = "GenRequest";
        public const string GenEmailRpt = "GenEmailRpt";
        public const string DisableChemSwAdmin = "DisableChemSwAdmin";
        public const string BatchOp = "BatchOp";
        public const string ExpiredContainers = "ExpiredContainers";
        public const string MolFingerprints = "MolFingerprints";
        public const string ContainerReconciliationActions = "ContainerReconciliationActions";
        public const string Reconciliation = "Reconciliation";
        public const string TierII = "TierII";
        public const string CAFImport = "CAFImport";
        public const string ExtChemDataSync = "ExtChemDataSync";
        public const string PurgeSessionData = "PurgeSessionData";
        public const string NodeCounts = "NodeCounts";

        #endregion Enum members

        #region IEquatable (NbtScheduleRuleNames)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumNbtScheduleRuleNames ft1, CswEnumNbtScheduleRuleNames ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtScheduleRuleNames ft1, CswEnumNbtScheduleRuleNames ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtScheduleRuleNames ) )
            {
                return false;
            }
            return this == (CswEnumNbtScheduleRuleNames) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtScheduleRuleNames obj )
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = ( ret * prime ) + Value.GetHashCode();
            ret = ( ret * prime ) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (NbtScheduleRuleNames)

    }

}
