using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public enum CswEnumNbtMainMenuActions
    {
        Unknown,
        AddNode,
        CopyNode,
        DeleteNode,
        editview,
        //GenericSearch,
        multiedit,
        PrintView,
        PrintLabel,
        SaveViewAs //,
        //ViewSearch
    };

} // namespace ChemSW.Nbt.WebServices
