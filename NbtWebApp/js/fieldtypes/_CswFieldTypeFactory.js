
$.CswFieldTypeFactory = function (method, methodOpt)
{
    var m = {
        make: '',
        save: '',
        nodeid: '',
        fieldtype: '',
        propdiv: '',
        propxml: '',
        onchange: '',
        cswnbtnodekey: ''
    }
    if (methodOpt)
    {
        $.extend(m, methodOpt);
    }
    var methods = {
        'make': function(m)
        {
            switch (m.fieldtype)
            {
                case "Barcode":
                    $propdiv.CswFieldTypeBarcode(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Composite":
                    $propdiv.CswFieldTypeComposite(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Date":
                    $propdiv.CswFieldTypeDate(m); //'init', nodeid, $propxml, onchange
                    break;
                case "File":
                    $propdiv.CswFieldTypeFile(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Grid":
                    $propdiv.CswFieldTypeGrid(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Image":
                    $propdiv.CswFieldTypeImage(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Link":
                    $propdiv.CswFieldTypeLink(m); //'init', nodeid, $propxml, onchange
                    break;
                case "List":
                    $propdiv.CswFieldTypeList(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Location":
                    $propdiv.CswFieldTypeLocation(m); //'init', nodeid, $propxml, onchange
                    break;
                case "LocationContents":
                    $propdiv.CswFieldTypeLocationContents(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Logical":
                    $propdiv.CswFieldTypeLogical(m); //'init', nodeid, $propxml, onchange
                    break;
                case "LogicalSet":
                    $propdiv.CswFieldTypeLogicalSet(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Memo":
                    $propdiv.CswFieldTypeMemo(m); //'init', nodeid, $propxml, onchange
                    break;
                case "MTBF":
                    $propdiv.CswFieldTypeMTBF(m); //'init', nodeid, $propxml, onchange
                    break;
                case "NodeTypeSelect":
                    $propdiv.CswFieldTypeNodeTypeSelect(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Number":
                    $propdiv.CswFieldTypeNumber(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Password":
                    $propdiv.CswFieldTypePassword(m); //'init', nodeid, $propxml, onchange
                    break;
                case "PropertyReference":
                    $propdiv.CswFieldTypePropertyReference(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Quantity":
                    $propdiv.CswFieldTypeQuantity(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Question":
                    $propdiv.CswFieldTypeQuestion(m); //'init', nodeid, $propxml, onchange
                    break;
                case "Relationship":
                    $propdiv.CswFieldTypeRelationship(m); //('init', nodeid, $propxml, onchange);
                    break;
                case "Sequence":
                    $propdiv.CswFieldTypeSequence(m); //('init', nodeid, $propxml, onchange);
                    break;
                case "Static":
                    $propdiv.CswFieldTypeStatic(m); //('init', nodeid, $propxml, onchange);
                    break;
                case "Text":
                    $propdiv.CswFieldTypeText(m); //('init', nodeid, $propxml, onchange);
                    break;
                case "Time":
                    $propdiv.CswFieldTypeTime(m); //('init', nodeid, $propxml, onchange);
                    break;
                case "TimeInterval":
                    $propdiv.CswFieldTypeTimeInterval(m); //('init', nodeid, $propxml, onchange);
                    break;
                case "UserSelect":
                    $propdiv.CswFieldTypeUserSelect(m); //('init', nodeid, $propxml, onchange);
                    break;
                case "ViewPickList":
                    $propdiv.CswFieldTypeViewPickList(m); //('init', nodeid, $propxml, onchange);
                    break;
                case "ViewReference":
                    $propdiv.CswFieldTypeViewReference(m); //('init', nodeid, $propxml, onchange);
                    break;
                default:
                    $propdiv.append($propxml.attr(m); //('gestalt'));
                    break;
            } // switch (fieldtype)
        }, // make
        
       'save': function (m) {
            switch (m.fieldtype)
            {
                case "Barcode":
                    $propdiv.CswFieldTypeBarcode(m); //('save', $propdiv, $propxml);
                    break;
                case "Composite":
                    $propdiv.CswFieldTypeComposite(m); //('save', $propdiv, $propxml);
                    break;
                case "Date":
                    $propdiv.CswFieldTypeDate(m); //('save', $propdiv, $propxml);
                    break;
                case "File":
                    $propdiv.CswFieldTypeFile(m); //('save', $propdiv, $propxml);
                    break;
                case "Grid":
                    $propdiv.CswFieldTypeGrid(m); //('save', $propdiv, $propxml, cswnbtnodekey);
                    break;
                case "Image":
                    $propdiv.CswFieldTypeImage(m); //('save', $propdiv, $propxml);
                    break;
                case "Link":
                    $propdiv.CswFieldTypeLink(m); //('save', $propdiv, $propxml);
                    break;
                case "List":
                    $propdiv.CswFieldTypeList(m); //('save', $propdiv, $propxml);
                    break;
                case "Location":
                    $propdiv.CswFieldTypeLocation(m); //('save', $propdiv, $propxml);
                    break;
                case "LocationContents":
                    $propdiv.CswFieldTypeLocationContents(m); //('save', $propdiv, $propxml);
                    break;
                case "Logical":
                    $propdiv.CswFieldTypeLogical(m); //('save', $propdiv, $propxml);
                    break;
                case "LogicalSet":
                    $propdiv.CswFieldTypeLogicalSet(m); //('save', $propdiv, $propxml);
                    break;
                case "Memo":
                    $propdiv.CswFieldTypeMemo(m); //('save', $propdiv, $propxml);
                    break;
                case "MTBF":
                    $propdiv.CswFieldTypeMTBF(m); //('save', $propdiv, $propxml);
                    break;
                case "NodeTypeSelect":
                    $propdiv.CswFieldTypeNodeTypeSelect(m); //('save', $propdiv, $propxml);
                    break;
                case "Number":
                    $propdiv.CswFieldTypeNumber(m); //('save', $propdiv, $propxml);
                    break;
                case "Password":
                    $propdiv.CswFieldTypePassword(m); //('save', $propdiv, $propxml);
                    break;
                case "PropertyReference":
                    $propdiv.CswFieldTypePropertyReference(m); //('save', $propdiv, $propxml);
                    break;
                case "Quantity":
                    $propdiv.CswFieldTypeQuantity(m); //('save', $propdiv, $propxml);
                    break;
                case "Question":
                    $propdiv.CswFieldTypeQuestion(m); //('save', $propdiv, $propxml);
                    break;
                case "Relationship":
                    $propdiv.CswFieldTypeRelationship(m); //('save', $propdiv, $propxml);
                    break;
                case "Sequence":
                    $propdiv.CswFieldTypeSequence(m); //('save', $propdiv, $propxml);
                    break;
                case "Static":
                    $propdiv.CswFieldTypeStatic(m); //('save', $propdiv, $propxml);
                    break;
                case "Text":
                    $propdiv.CswFieldTypeText(m); //('save', $propdiv, $propxml);
                    break;
                case "Time":
                    $propdiv.CswFieldTypeTime(m); //('save', $propdiv, $propxml);
                    break;
                case "TimeInterval":
                    $propdiv.CswFieldTypeTimeInterval(m); //('save', $propdiv, $propxml);
                    break;
                case "UserSelect":
                    $propdiv.CswFieldTypeUserSelect(m); //('save', $propdiv, $propxml);
                    break;
                case "ViewPickList":
                    $propdiv.CswFieldTypeViewPickList(m); //('save', $propdiv, $propxml);
                    break;
                case "ViewReference":
                    $propdiv.CswFieldTypeViewReference(m); //('save', $propdiv, $propxml);
                    break;
                default:
                    break;
            } // switch(fieldtype)
        } // save
    };

    // Method calling logic
    if (m.methods[method])
    {
        return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
    } else if (typeof method === 'object' || !method)
    {
        return methods.init.apply(this, arguments);
    } else
    {
        $.error('Method ' + method + ' does not exist on ' + PluginName);
    }
}    // $.CswFieldTypeFactory
