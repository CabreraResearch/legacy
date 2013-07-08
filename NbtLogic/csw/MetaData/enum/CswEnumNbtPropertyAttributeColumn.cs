using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Nbt Object Class Name
    /// </summary>
    public sealed class CswEnumNbtPropertyAttributeColumn : IEquatable<CswEnumNbtPropertyAttributeColumn>, IComparable<CswEnumNbtPropertyAttributeColumn>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
        {
            { Append             , Append             },
            { Datetoday          , Datetoday          },
            { Fieldtypeid        , Fieldtypeid        },
            { Fkvalue            , Fkvalue            },
            { Isbatchentry       , Isbatchentry       },
            { Isfk               , Isfk               },
            { Isrequired         , Isrequired         },
            { Isunique           , Isunique           },
            { Nodetypeid         , Nodetypeid         },
            { Nodetypepropid     , Nodetypepropid     },
            { Nodetypetabsetid   , Nodetypetabsetid   },
            { Objectclasspropid  , Objectclasspropid  },
            { Servermanaged      , Servermanaged      },
            { Textareacols       , Textareacols       },
            { Textarearows       , Textarearows       },
            { Textlength         , Textlength         },
            { Url                , Url                },
            { Valuepropid        , Valuepropid        },
            { Width              , Width              },
            { Sequenceid         , Sequenceid         },
            { Numberprecision    , Numberprecision    },
            { Listoptions        , Listoptions        },
            { Compositetemplate  , Compositetemplate  },
            { Fktype             , Fktype             },
            { Valueproptype      , Valueproptype      },
            { Statictext         , Statictext         },
            { Multi              , Multi              },
            { Nodeviewid         , Nodeviewid         },
            { Readonly           , Readonly           },
            { Numberminvalue     , Numberminvalue     },
            { Numbermaxvalue     , Numbermaxvalue     },
            { Usenumbering       , Usenumbering       },
            { Questionno         , Questionno         },
            { Subquestionno      , Subquestionno      },
            { Filter             , Filter             },
            { Filterpropid       , Filterpropid       },
            { Firstpropversionid , Firstpropversionid },
            { Priorpropversionid , Priorpropversionid },
            { Valueoptions       , Valueoptions       },
            { Defaultvalue       , Defaultvalue       },
            { Helptext           , Helptext           },
            { Propname           , Propname           },
            { Defaultvalueid     , Defaultvalueid     },
            { Isquicksearch      , Isquicksearch      },
            { Hideinmobile       , Hideinmobile       },
            { Mobilesearch       , Mobilesearch       },
            { Isdemo             , Isdemo             },
            { Auditlevel         , Auditlevel         },
            { Iscompoundunique   , Iscompoundunique   },
            { Attribute1         , Attribute1         },
            { Attribute2         , Attribute2         },
            { Attribute3         , Attribute3         },
            { Attribute4         , Attribute4         },
            { Attribute5         , Attribute5         },
            { Extended           , Extended           }
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
        public CswEnumNbtPropertyAttributeColumn( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtPropertyAttributeColumn( string Val )
        {
            return new CswEnumNbtPropertyAttributeColumn( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtPropertyAttributeColumn item )
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

        public const string Append             = "append"                   ;
        public const string Datetoday          = "datetoday"                ;
        public const string Fieldtypeid        = "fieldtypeid"              ;
        public const string Fkvalue            = "fkvalue"                  ;
        public const string Isbatchentry       = "isbatchentry"             ;
        public const string Isfk               = "isfk"                     ;
        public const string Isrequired         = "isrequired"               ;
        public const string Isunique           = "isunique"                 ;
        public const string Nodetypeid         = "nodetypeid"               ;
        public const string Nodetypepropid     = "nodetypepropid"           ;
        public const string Nodetypetabsetid   = "nodetypetabsetid"         ;
        public const string Objectclasspropid  = "objectclasspropid"        ;
        public const string Servermanaged      = "servermanaged"            ;
        public const string Textareacols       = "textareacols"             ;
        public const string Textarearows       = "textarearows"             ;
        public const string Textlength         = "textlength"               ;
        public const string Url                = "url"                      ;
        public const string Valuepropid        = "valuepropid"              ;
        public const string Width              = "width"                    ;
        public const string Sequenceid         = "sequenceid"               ;
        public const string Numberprecision    = "numberprecision"          ;
        public const string Listoptions        = "listoptions"              ;
        public const string Compositetemplate  = "compositetemplate"        ;
        public const string Fktype             = "fktype"                   ;
        public const string Valueproptype      = "valueproptype"            ;
        public const string Statictext         = "statictext"               ;
        public const string Multi              = "multi"                    ;
        public const string Nodeviewid         = "nodeviewid"               ;
        public const string Readonly           = "readonly"                 ;
        public const string Numberminvalue     = "numberminvalue"           ;
        public const string Numbermaxvalue     = "numbermaxvalue"           ;
        public const string Usenumbering       = "usenumbering"             ;
        public const string Questionno         = "questionno"               ;
        public const string Subquestionno      = "subquestionno"            ;
        public const string Filter             = "filter"                   ;
        public const string Filterpropid       = "filterpropid"             ;
        public const string Firstpropversionid = "firstpropversionid"       ;
        public const string Priorpropversionid = "priorpropversionid"       ;
        public const string Valueoptions       = "valueoptions"             ;
        public const string Defaultvalue       = "defaultvalue"             ;
        public const string Helptext           = "helptext"                 ;
        public const string Propname           = "propname"                 ;
        public const string Defaultvalueid     = "defaultvalueid"           ;
        public const string Isquicksearch      = "isquicksearch"            ;
        public const string Hideinmobile       = "hideinmobile"             ;
        public const string Mobilesearch       = "mobilesearch"             ;
        public const string Isdemo             = "isdemo"                   ;
        public const string Auditlevel         = "auditlevel"               ;
        public const string Iscompoundunique   = "iscompoundunique"         ;
        public const string Attribute1         = "attribute1"               ;
        public const string Attribute2         = "attribute2"               ;
        public const string Attribute3         = "attribute3"               ;
        public const string Attribute4         = "attribute4"               ;
        public const string Attribute5         = "attribute5"               ;
        public const string Extended           = "extended"                 ;

        #endregion Enum members

        #region IComparable

        public int CompareTo( CswEnumNbtPropertyAttributeColumn other )
        {
            return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
        }

        #endregion IComparable

        #region IEquatable (CswEnumNbtPropertyAttribute)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumNbtPropertyAttributeColumn ft1, CswEnumNbtPropertyAttributeColumn ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtPropertyAttributeColumn ft1, CswEnumNbtPropertyAttributeColumn ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtPropertyAttributeColumn ) )
            {
                return false;
            }
            return this == (CswEnumNbtPropertyAttributeColumn) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtPropertyAttributeColumn obj )
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
