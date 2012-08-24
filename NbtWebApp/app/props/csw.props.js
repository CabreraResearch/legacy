
/// <reference path="~/app/CswApp-vsdoc.js" />

(function() {
    'use strict';

    var preparePropJsonForSaveRecursive = function(isMulti, propVals, attributes) {
        ///<summary>Recurses over the subfields and sub-subfields of a property to update its JSON.</summary>
        ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
        ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
        ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
        ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
        var wasModified = false;
        if (false === Csw.isNullOrEmpty(propVals)) {
            Csw.crawlObject(propVals, function(prop, key) {
                if (Csw.contains(attributes, key)) {
                    var attr = attributes[key];
                    //don't bother sending this to server unless it's changed
                    if (Csw.isPlainObject(attr)) {
                        wasModified = preparePropJsonForSaveRecursive(isMulti, propVals[key], attr) || wasModified;
                    } else if ((false === isMulti && propVals[key] !== attr) ||
                        (isMulti && false === Csw.isNullOrUndefined(attr) && attr !== Csw.enums.multiEditDefaultValue)) {
                        wasModified = true;
                        propVals[key] = attr;
                    }
                }
            }, false);
        }
        return wasModified;
    };

    var preparePropJsonForSave = function(isMulti, propData, attributes) {
        ///<summary>Takes property JSON from the form and modifies it in order to send back to the server.</summary>
        ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
        ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
        ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
        ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
        var wasModified = false;
        if (false === Csw.isNullOrEmpty(propData)) {
            if (Csw.contains(propData, 'values')) {
                var propVals = propData.values;
                wasModified = preparePropJsonForSaveRecursive(isMulti, propVals, attributes);
            }
            propData.wasmodified = propData.wasmodified || wasModified;
        }
    };
    Csw.register('preparePropJsonForSave', preparePropJsonForSave);
    Csw.preparePropJsonForSave = Csw.preparePropJsonForSave || preparePropJsonForSave;


}());
