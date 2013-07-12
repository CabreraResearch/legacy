using ChemSW.Core;
using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.MetaData
{
    public sealed class CswEnumNbtNodeTypePropAttributes : IEquatable<CswEnumNbtNodeTypePropAttributes>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                {
                                                                    {append,append },
                                                                    {        auditlevel, auditlevel},
                                                                    {        datetoday,datetoday },
                                                                    {        fieldtypeid, fieldtypeid},
                                                                    {        isbatchentry,isbatchentry },
                                                                    {        isrequired, isrequired},
                                                                    {        isunique,isunique},
                                                                    {        iscompoundunique,iscompoundunique },
                                                                    {        isfk,isfk },
                                                                    {        fktype, fktype},
                                                                    {        fkvalue,fkvalue },
                                                                    {        servermanaged, servermanaged},
                                                                    {        textareacols, textareacols},
                                                                    {        textarearows,textarearows },
                                                                    {        textlength,textlength },
                                                                    {        url,url },
                                                                    {        valuepropid, valuepropid},
                                                                    {        width,width },
                                                                    {        sequenceid, sequenceid},
                                                                    {        numberprecision,numberprecision },
                                                                    {        listoptions,listoptions },
                                                                    {        compositetemplate,compositetemplate },
                                                                    {        valueproptype,valueproptype },
                                                                    {        statictext, statictext},
                                                                    {        multi,multi },
                                                                    {        nodeviewid,nodeviewid },
                                                                    {        readOnly,readOnly },
                                                                    {        numberminvalue,numberminvalue },
                                                                    {        numbermaxvalue, numbermaxvalue},
                                                                    {        usenumbering,usenumbering },
                                                                    {        questionno,questionno },
                                                                    {        subquestionno,subquestionno },
                                                                    {        filter, filter},
                                                                    {        filterpropid,filterpropid },
                                                                    {        valueoptions,valueoptions},
                                                                    {        helptext,helptext },
                                                                    {        propname, propname},
                                                                    {        isquicksearch,isquicksearch },
                                                                    {        extended,extended },
                                                                    {        attribute1, attribute1},
                                                                    {        attribute2, attribute2},
                                                                    {        attribute3, attribute3},
                                                                    {        attribute4, attribute4},
                                                                    {        attribute5, attribute5}
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
        public CswEnumNbtNodeTypePropAttributes(string ItemName = CswResources.UnknownEnum)
        {
            Value = _Parse(ItemName);
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtNodeTypePropAttributes(string Val)
        {
            return new CswEnumNbtNodeTypePropAttributes(Val);
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string(CswEnumNbtNodeTypePropAttributes item)
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

        /// <summary>
        /// Enum member 1
        /// </summary>
        public const string append = "append";
        public const string auditlevel = "auditlevel";
        public const string datetoday = "datetoday";
        public const string fieldtypeid = "fieldtypeid";
        public const string isbatchentry = "isbatchentry";
        public const string isrequired = "isrequired";
        public const string isunique = "isunique";
        public const string iscompoundunique = "iscompoundunique";
        public const string isfk = "isfk";
        public const string fktype = "fktype";
        public const string fkvalue = "fkvalue";
        public const string servermanaged = "servermanaged";
        public const string textareacols = "textareacols";
        public const string textarearows = "textarearows";
        public const string textlength = "textlength";
        public const string url = "url";
        public const string valuepropid = "valuepropid";
        public const string width = "width";
        public const string sequenceid = "sequenceid";
        public const string numberprecision = "numberprecision";
        public const string listoptions = "listoptions";
        public const string compositetemplate = "compositetemplate";
        public const string valueproptype = "valueproptype";
        public const string statictext = "statictext";
        public const string multi = "multi";
        public const string nodeviewid = "nodeviewid";
        public const string readOnly = "readOnly";
        public const string numberminvalue = "numberminvalue";
        public const string numbermaxvalue = "numbermaxvalue";
        public const string usenumbering = "usenumbering";
        public const string questionno = "questionno";
        public const string subquestionno = "subquestionno";
        public const string filter = "filter";
        public const string filterpropid = "filterpropid";
        public const string valueoptions = "valueoptions";
        public const string helptext = "helptext";
        public const string propname = "propname";
        public const string isquicksearch = "isquicksearch";
        public const string extended = "extended";
        public const string attribute1 = "attribute1";
        public const string attribute2 = "attribute2";
        public const string attribute3 = "attribute3";
        public const string attribute4 = "attribute4";
        public const string attribute5 = "attribute5";

        #endregion Enum members

        #region IEquatable (CswEnumNbtNodeTypePropAttributes)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==(CswEnumNbtNodeTypePropAttributes ft1, CswEnumNbtNodeTypePropAttributes ft2)
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString(ft1) == CswConvert.ToString(ft2);
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=(CswEnumNbtNodeTypePropAttributes ft1, CswEnumNbtNodeTypePropAttributes ft2)
        {
            return !(ft1 == ft2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswEnumNbtNodeTypePropAttributes))
            {
                return false;
            }
            return this == (CswEnumNbtNodeTypePropAttributes)obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(CswEnumNbtNodeTypePropAttributes obj)
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

        #endregion IEquatable (CswEnumNbtNodeTypePropAttributes)

    };

}
