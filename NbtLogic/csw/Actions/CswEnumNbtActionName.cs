using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.Actions
{
    /// <summary>
    /// Template for new CswEnumNbtActionName class
    /// </summary>
    public sealed class CswEnumNbtActionName : IEquatable<CswEnumNbtActionName>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
                                                                    
            {   Create_Material,           Create_Material           },
            {   Design,                    Design                    },
            {   Edit_View,                 Edit_View                 },
            {   Future_Scheduling,         Future_Scheduling         },
            {   Create_Inspection,         Create_Inspection         },
            {   Multi_Edit,                Multi_Edit                },
            {   Quotas,                    Quotas                    },
            {   Sessions,                  Sessions                  },
            {   View_Scheduled_Rules,      View_Scheduled_Rules      },
            {   Modules,                   Modules                   },
            {   Submit_Request,            Submit_Request            },
            {   DispenseContainer,         DispenseContainer         },
            {   DisposeContainer,          DisposeContainer          },
            {   UndisposeContainer,        UndisposeContainer        },
            {   Receiving,                 Receiving                 },
            {   Subscriptions,             Subscriptions             },
            {   Reconciliation,            Reconciliation            },
            {   Upload_Legacy_Mobile_Data, Upload_Legacy_Mobile_Data },
            {   HMIS_Reporting,            HMIS_Reporting            },
            {   Kiosk_Mode,                Kiosk_Mode                },
            {   Tier_II_Reporting,         Tier_II_Reporting         },
            {   Material_Approval,         Material_Approval         },
            {   Login_Data,                Login_Data                },
            {   Manage_Locations,          Manage_Locations          },
            {   Delete_Demo_Data,          Delete_Demo_Data          },
            {   Container_Expiration_Lock, Container_Expiration_Lock }

        };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        private static string _Parse(string Val)
        {
            string ret = CswResources.UnknownEnum;
            if (_Enums.ContainsKey(Val))
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// The enum constructor
        /// </summary>
        public CswEnumNbtActionName(string ItemName = CswResources.UnknownEnum)
        {
            Value = _Parse(ItemName);
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtActionName(string Val)
        {
            return new CswEnumNbtActionName(Val);
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string(CswEnumNbtActionName item)
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

        public const string Create_Material = "Create_Material";
        public const string Design = "Design";
        public const string Edit_View = "Edit_View";
        public const string Future_Scheduling = "Future_Scheduling";
        public const string Create_Inspection = "Create_Inspection";
        public const string Multi_Edit = "Multi_Edit";
        public const string Quotas = "Quotas";
        public const string Sessions = "Sessions";
        public const string View_Scheduled_Rules = "View_Scheduled_Rules";
        public const string Modules = "Modules";
        public const string Submit_Request = "Submit_Request";
        public const string DispenseContainer = "DispenseContainer";
        public const string DisposeContainer = "DisposeContainer";
        public const string UndisposeContainer = "UndisposeContainer";
        public const string Receiving = "Receiving";
        public const string Subscriptions = "Subscriptions";
        public const string Reconciliation = "Reconciliation";
        public const string Upload_Legacy_Mobile_Data = "Upload_Legacy_Mobile_Data";
        public const string HMIS_Reporting = "HMIS_Reporting";
        public const string Kiosk_Mode = "Kiosk_Mode";
        public const string Tier_II_Reporting = "Tier_II_Reporting";
        public const string Material_Approval = "Material_Approval";
        public const string Login_Data = "Login_Data";
        public const string Manage_Locations = "Manage_Locations";
        public const string Delete_Demo_Data = "Delete_Demo_Data";
        public const string Container_Expiration_Lock = "Container_Expiration_Lock";

        #endregion Enum members

        #region IEquatable (CswEnumNbtActionName)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==(CswEnumNbtActionName ft1, CswEnumNbtActionName ft2)
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString(ft1) == CswConvert.ToString(ft2);
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=(CswEnumNbtActionName ft1, CswEnumNbtActionName ft2)
        {
            return !(ft1 == ft2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswEnumNbtActionName))
            {
                return false;
            }
            return this == (CswEnumNbtActionName)obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(CswEnumNbtActionName obj)
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = (ret * prime) + Value.GetHashCode();
            ret = (ret * prime) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (CswEnumNbtActionName)

    };

}
