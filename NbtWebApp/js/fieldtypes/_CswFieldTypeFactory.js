
$.CswFieldTypeFactory = function (method) {

    var methods = {
        'make': function (nodeid, fieldtype, $propdiv, $propxml) {
            switch (fieldtype) {
                case "Barcode":
                    $propdiv.CswFieldTypeBarcode('init', nodeid, $propxml);
                    break;
                case "Date":
                    $propdiv.CswFieldTypeDate('init', nodeid, $propxml);
                    break;
                case "Image":
                    $propdiv.CswFieldTypeImage('init', nodeid, $propxml);
                    break;
                case "Link":
                    $propdiv.CswFieldTypeLink('init', nodeid, $propxml);
                    break;
                case "List":
                    $propdiv.CswFieldTypeList('init', nodeid, $propxml);
                    break;
                case "Location":
                    $propdiv.CswFieldTypeLocation('init', nodeid, $propxml);
                    break;
                case "Logical":
                    $propdiv.CswFieldTypeLogical('init', nodeid, $propxml);
                    break;
                case "LogicalSet":
                    $propdiv.CswFieldTypeLogicalSet('init', nodeid, $propxml);
                    break;
                case "Memo":
                    $propdiv.CswFieldTypeMemo('init', nodeid, $propxml);
                    break;
                case "MTBF":
                    $propdiv.CswFieldTypeMTBF('init', nodeid, $propxml);
                    break;
                case "Number":
                    $propdiv.CswFieldTypeNumber('init', nodeid, $propxml);
                    break;
                case "Password":
                    $propdiv.CswFieldTypePassword('init', nodeid, $propxml);
                    break;
                case "PropertyReference":
                    $propdiv.CswFieldTypePropertyReference('init', nodeid, $propxml);
                    break;
                case "Relationship":
                    $propdiv.CswFieldTypeRelationship('init', nodeid, $propxml);
                    break;
                case "Sequence":
                    $propdiv.CswFieldTypeSequence('init', nodeid, $propxml);
                    break;
                case "Static":
                    $propdiv.CswFieldTypeStatic('init', nodeid, $propxml);
                    break;
                case "Text":
                    $propdiv.CswFieldTypeText('init', nodeid, $propxml);
                    break;
                default:
                    $propdiv.append($propxml.attr('gestalt'));
                    break;
            } // switch (fieldtype)
        }, // make

        'save': function (fieldtype, nodeid, $propdiv, $propxml) {
            switch (fieldtype) {
                case "Barcode":
                    $propdiv.CswFieldTypeBarcode('save', $propdiv, $propxml);
                    break;
                case "Date":
                    $propdiv.CswFieldTypeDate('save', $propdiv, $propxml);
                    break;
                case "Image":
                    $propdiv.CswFieldTypeImage('save', $propdiv, $propxml);
                    break;
                case "Link":
                    $propdiv.CswFieldTypeLink('save', $propdiv, $propxml);
                    break;
                case "List":
                    $propdiv.CswFieldTypeList('save', $propdiv, $propxml);
                    break;
                case "Location":
                    $propdiv.CswFieldTypeLocation('save', $propdiv, $propxml);
                    break;
                case "Logical":
                    $propdiv.CswFieldTypeLogical('save', $propdiv, $propxml);
                    break;
                case "LogicalSet":
                    $propdiv.CswFieldTypeLogicalSet('save', $propdiv, $propxml);
                    break;
                case "Memo":
                    $propdiv.CswFieldTypeMemo('save', $propdiv, $propxml);
                    break;
                case "MTBF":
                    $propdiv.CswFieldTypeMTBF('save', $propdiv, $propxml);
                    break;
                case "Number":
                    $propdiv.CswFieldTypeNumber('save', $propdiv, $propxml);
                    break;
                case "Password":
                    $propdiv.CswFieldTypePassword('save', $propdiv, $propxml);
                    break;
                case "PropertyReference":
                    $propdiv.CswFieldTypePropertyReference('save', $propdiv, $propxml);
                    break;
                case "Relationship":
                    $propdiv.CswFieldTypeRelationship('save', $propdiv, $propxml);
                    break;
                case "Sequence":
                    $propdiv.CswFieldTypeSequence('save', $propdiv, $propxml);
                    break;
                case "Static":
                    $propdiv.CswFieldTypeStatic('save', $propdiv, $propxml);
                    break;
                case "Text":
                    $propdiv.CswFieldTypeText('save', $propdiv, $propxml);
                    break;
                default:
                    break;
            } // switch(fieldtype)
        } // save
    };

    // Method calling logic
    if (methods[method]) {
        return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
    } else if (typeof method === 'object' || !method) {
        return methods.init.apply(this, arguments);
    } else {
        $.error('Method ' + method + ' does not exist on ' + PluginName);
    }
} // $.CswFieldTypeFactory