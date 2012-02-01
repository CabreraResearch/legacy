/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    var preparePropJsonForSave = function (isMulti, propData, attributes) {
        ///<summary>Takes property JSON from the form and modifies it in order to send back to the server.</summary>
        ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
        ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
        ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
        ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
        var wasModified = false;
        if (false === Csw.isNullOrEmpty(propData)) {
            if (contains(propData, 'values')) {
                var propVals = propData.values;
                wasModified = preparePropJsonForSaveRecursive(isMulti, propVals, attributes);
            }
            propData.wasmodified = propData.wasmodified || wasModified;
        }
    };
    Csw.register('preparePropJsonForSave', preparePropJsonForSave);
    Csw.preparePropJsonForSave = Csw.preparePropJsonForSave || preparePropJsonForSave;

    var preparePropJsonForSaveRecursive = function (isMulti, propVals, attributes) {
        ///<summary>Recurses over the subfields and sub-subfields of a property to update its JSON.</summary>
        ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
        ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
        ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
        ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
        if (false === Csw.isNullOrEmpty(propVals)) {
            var wasModified = false;
            crawlObject(propVals, function (prop, key) {
                if (contains(attributes, key)) {
                    var attr = attributes[key];
                    //don't bother sending this to server unless it's changed
                    if (isPlainObject(attr)) {
                        wasModified = preparePropJsonForSaveRecursive(isMulti, propVals[key], attr) || wasModified;
                    } else if ((false === isMulti && propVals[key] !== attr) ||
                        (isMulti && false === isNullOrUndefined(attr) && attr !== CswMultiEditDefaultValue)) {
                        wasModified = true;
                        propVals[key] = attr;
                    }
                }
            }, false);
        } else {
            //debug
        }
        return wasModified;
    };

}());