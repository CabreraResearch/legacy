using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData
{
    public enum CswEnumNbtNodeTypePropAttributes
    {
        unknown,
        append,
        auditlevel,
        datetoday,
        fieldtypeid,
        isbatchentry,
        isrequired,
        isunique,
        iscompoundunique,
        isfk,
        fktype,
        fkvalue,
        servermanaged,
        textareacols,
        textarearows,
        textlength,
        url,
        valuepropid,
        width,
        sequenceid,
        numberprecision,
        listoptions,
        compositetemplate,
        valueproptype,
        statictext,
        multi,
        nodeviewid,
        readOnly,
        numberminvalue,
        numbermaxvalue,
        usenumbering,
        questionno,
        subquestionno,
        filter,
        filterpropid,
        valueoptions,
        helptext,
        propname,
        isquicksearch,
        extended,
        attribute1,
        attribute2,
        attribute3,
        attribute4,
        attribute5
    }
}
