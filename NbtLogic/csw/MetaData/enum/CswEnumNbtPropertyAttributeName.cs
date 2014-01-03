using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Nbt Object Class Name
    /// </summary>
    public sealed class CswEnumNbtPropertyAttributeName : IEquatable<CswEnumNbtPropertyAttributeName>, IComparable<CswEnumNbtPropertyAttributeName>
    {
        #region Internals

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
            {
              { AuditLevel                    , AuditLevel                   },
              { CompoundUnique                , CompoundUnique               },
              { DisplayConditionFilter        , DisplayConditionFilter       },
              { DisplayConditionProperty      , DisplayConditionProperty     },
              { DisplayConditionSubfield      , DisplayConditionSubfield     },
              { DisplayConditionValue         , DisplayConditionValue        },
              { FieldType                     , FieldType                    },
              { HelpText                      , HelpText                     },
              { NodeTypeValue                 , NodeTypeValue                },
              { ObjectClassPropName           , ObjectClassPropName          },
              { PropName                      , PropName                     },
              { ReadOnly                      , ReadOnly                     },
              { Required                      , Required                     },
              { ServerManaged                 , ServerManaged                },
              { Unique                        , Unique                       },
              { UseNumbering                  , UseNumbering                 },
            
              { AllowMultipleValues           , AllowMultipleValues           }, 
              { ButtonText                    , ButtonText                    }, 
              { ChildRelationship             , ChildRelationship             }, 
              { Columns                       , Columns                       }, 
              { CompliantAnswers              , CompliantAnswers              }, 
              { ConfirmationDialogMessage     , ConfirmationDialogMessage     }, 
              { ConstrainToObjectClass        , ConstrainToObjectClass        }, 
              { DateType                      , DateType                      }, 
              { DefaultToToday                , DefaultToToday                }, 
              { DefaultValue                  , DefaultValue                  }, 
              { DisplayMode                   , DisplayMode                   }, 
              { ExcludeRangeLimits            , ExcludeRangeLimits            }, 
              { FKType                        , FKType                        }, 
              { FKValue                       , FKValue                       }, 
              { HeightInPixels                , HeightInPixels                }, 
              { HideSpecial                   , HideSpecial                   }, 
              { ImageNames                    , ImageNames                    }, 
              { ImageUrls                     , ImageUrls                     }, 
              { IsFK                          , IsFK                          }, 
              { Length                        , Length                        }, 
              { MaximumLength                 , MaximumLength                 }, 
              { MaximumRows                   , MaximumRows                   }, 
              { MaximumValue                  , MaximumValue                  }, 
              { MinimumValue                  , MinimumValue                  }, 
              { Options                       , Options                       }, 
              { PossibleAnswers               , PossibleAnswers               }, 
              { Precision                     , Precision                     }, 
              { PreferredAnswer               , PreferredAnswer               }, 
              { Prefix                        , Prefix                        }, 
              { QuantityOptional              , QuantityOptional              },
              { ReadOnlyDelimiter             , ReadOnlyDelimiter             }, 
              { ReadOnlyHideThreshold         , ReadOnlyHideThreshold         }, 
              { RegexMessage                  , RegexMessage                  }, 
              { Relationship                  , Relationship                  }, 
              { RelatedProperty               , RelatedProperty               }, 
              { RelatedPropType               , RelatedPropType               },
              { Rows                          , Rows                          }, 
              { SelectMode                    , SelectMode                    }, 
              { Sequence                      , Sequence                      }, 
              { ShowHeaders                   , ShowHeaders                   }, 
              { Size                          , Size                          }, 
              { Suffix                        , Suffix                        }, 
              { Target                        , Target                        }, 
              { Template                      , Template                      }, 
              { Text                          , Text                          }, 
              { UseSequence                   , UseSequence                   }, 
              { UnitTarget                    , UnitTarget                    }, 
              { UnitView                      , UnitView                      }, 
              { ValidationRegex               , ValidationRegex               }, 
              { View                          , View                          }, 
              { WidthInPixels                 , WidthInPixels                 }, 
              { XOptions                      , XOptions                      }, 
              { YOptions                      , YOptions                      } 
            };



        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        public static IEnumerable<string> All
        {
            get { return _Enums.Values; }
        }

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
        public CswEnumNbtPropertyAttributeName( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtPropertyAttributeName( string Val )
        {
            return new CswEnumNbtPropertyAttributeName( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtPropertyAttributeName item )
        {
            return ( null != item ) ? item.Value : CswNbtResources.UnknownEnum;
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

        public const string AuditLevel = "Audit Level";
        public const string CompoundUnique = "Compound Unique";
        public const string DisplayConditionFilter = "Display Condition Filter";
        public const string DisplayConditionProperty = "Display Condition Property";
        public const string DisplayConditionSubfield = "Display Condition Subfield";
        public const string DisplayConditionValue = "Display Condition Value";
        public const string FieldType = "Field Type";
        public const string HelpText = "Help Text";
        public const string NodeTypeValue = "NodeType";
        public const string ObjectClassPropName = "Original Name";
        public const string PropName = "Prop Name";
        public const string ReadOnly = "Read Only";
        public const string Required = "Required";
        public const string ServerManaged = "Server Managed";
        public const string Unique = "Unique";
        public const string UseNumbering = "Use Numbering";

        public const string AllowMultipleValues = "Allow Multiple Values";
        public const string ButtonText = "Button Text";
        public const string ChildRelationship = "Child Relationship";
        public const string Columns = "Columns";
        public const string CompliantAnswers = "CompliantAnswers";
        public const string ConfirmationDialogMessage = "Confirmation Dialog Message";
        public const string ConstrainToObjectClass = "Constrain to Object Class";
        public const string DateType = "Date Type";
        public const string DefaultToToday = "Default to Today";
        public const string DefaultValue = "Default Value";
        public const string DisplayMode = "Display Mode";
        public const string ExcludeRangeLimits = "Exclude Range Limits";
        public const string FKType = "FK Type";
        public const string FKValue = "FK Value";
        public const string HeightInPixels = "Height in Pixels";
        public const string HideSpecial = "Hide Special";
        public const string ImageNames = "Image Names, separated by newlines";
        public const string ImageUrls = "Image URLs, separated by newlines";
        public const string IsFK = "Is FK";
        public const string Length = "Length";
        public const string MaximumLength = "Maximum Length";
        public const string MaximumRows = "Maximum Rows";
        public const string MaximumValue = "Maximum Value";
        public const string MinimumValue = "Minimum Value";
        public const string Options = "Options";
        public const string PossibleAnswers = "PossibleAnswers";
        public const string Precision = "Precision";
        public const string PreferredAnswer = "PreferredAnswer";
        public const string Prefix = "Prefix";
        public const string QuantityOptional = "Quantity Optional";
        public const string ReadOnlyDelimiter = "ReadOnly Delimiter";
        public const string ReadOnlyHideThreshold = "ReadOnly Hide Threshold";
        public const string RegexMessage = "Regex Message";
        public const string Relationship = "Relationship";
        public const string RelatedProperty = "Related Property";
        public const string RelatedPropType = "Related Property Type";
        public const string Rows = "Rows";
        public const string SelectMode = "Select Mode";
        public const string Sequence = "Sequence";
        public const string ShowHeaders = "Show Headers";
        public const string Size = "Size";
        public const string Suffix = "Suffix";
        public const string Target = "Target";
        public const string Template = "Template";
        public const string Text = "Text";
        public const string UseSequence = "Use Sequence";
        public const string UnitTarget = "UnitTarget";
        public const string UnitView = "UnitView";
        public const string ValidationRegex = "Validation Regex";
        public const string View = "View";
        public const string WidthInPixels = "Width in Pixels";
        public const string XOptions = "X Options";
        public const string YOptions = "Y Options";

        #endregion Enum members

        #region IComparable

        public int CompareTo( CswEnumNbtPropertyAttributeName other )
        {
            return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
        }

        #endregion IComparable

        #region IEquatable (CswEnumNbtPropertyAttribute)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumNbtPropertyAttributeName ft1, CswEnumNbtPropertyAttributeName ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtPropertyAttributeName ft1, CswEnumNbtPropertyAttributeName ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtPropertyAttributeName ) )
            {
                return false;
            }
            return this == (CswEnumNbtPropertyAttributeName) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtPropertyAttributeName obj )
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

        #endregion IEquatable (CswEnumNbtPropertyAttribute)

    };
}
