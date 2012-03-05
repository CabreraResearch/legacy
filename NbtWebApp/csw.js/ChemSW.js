/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

window.abandonHope = false;
(function(window) {
    'use strict';
    var document = window.document,
        navigator = window.navigator,
        location = window.location;

    window.ChemSW = window.Csw = (function() {

        var internal = {
            document: document,
            navigator: navigator,
            location: location,
            $: $,
            homeUrl: 'Main.html',
            methods: ['register'],
            uniqueIdCount: 0,
            protectedmethods: ['register', 'deregister', 'getGlobalProp', 'setGlobalProp']
        };

        var external = {};

        external.register = function(name, obj, isProtected) {
            /// <summary>
            ///   Register an Object in the ChemSW namespace
            /// </summary>
            /// <param name="name" type="String"> Name of the object </param>
            /// <param name="obj" type="Object"> Object to pass </param>
            /// <param name="isProtected" type="Boolean"> If true, the object cannot be removed from the namespace </param>
            /// <returns type="Boolean">True if the object name did not already exist in the namespace.</returns>
            var succeeded = false;
            if (internal.methods.indexOf(name) === -1) {
                internal.methods.push(name);
                obj[name] = true; //for shimming our own instanceof
                if (isProtected && internal.protectedmethods.indexOf(name) === -1) {
                    internal.protectedmethods.push(name);
                }
                external[name] = obj;
                succeeded = true;
            }
            return succeeded;
        };

        external.deregister = function(name) {
            /// <summary>
            ///   Deregister an Object from the ChemSW namespace
            /// </summary>
            /// <param name="name" type="String"> Name of the object.</param>
            /// <returns type="Boolean">True if the object was removed.</returns>
            var succeeded = false;
            if (internal.protectedmethods.indexOf(name) === -1) {
                if (internal.methods.indexOf(name) !== -1) {
                    internal.methods.splice(name, 1);
                }
                delete external[name];
                succeeded = true;
            }
            return succeeded;
        };
        external.register('deregister', external.deregister);

        external.getGlobalProp = function(propName) {
            /// <summary>
            ///   Fetch a dereferenced copy of a property from the private universe collection
            /// </summary>
            /// <param name="propName" type="String"> Name of the property </param>
            /// <returns type="Object">A clone of the property.</returns>
            var retVal;
            if (propName && internal.hasOwnProperty(propName)) {
                retVal = internal[propName];
            } else {
                retVal = {};
                $.extend(retVal, internal);
            }
            return retVal;
        };
        external.register('getGlobalProp', external.getGlobalProp);

        external.setGlobalProp = function(prop, val) {
            /// <summary>
            ///   Change the value of a property in the private universe collection
            /// </summary>
            /// <param name="name" type="String"> Name of the object </param>
            /// <returns type="Boolean">True if the property was updated.</returns>
            var success = false;
            if (prop && val && internal.hasOwnProperty(prop)) {
                internal[prop] = val;
                success = true;
            }
            return success;
        };
        external.register('setGlobalProp', external.setGlobalProp);

        external.addGlobalProp = function(propName, val) {
            /// <summary>
            ///   Add a property to the private universe collection
            /// </summary>
            /// <param name="propName" type="String"> Name of the object </param>
            /// <param name="val" type="Object"> Value of the object </param>
            /// <returns type="Boolean">True if the property was added.</returns>
            var success = false;
            if (propName && val && false === internal.hasOwnProperty(propName)) {
                internal[propName] = val;
                success = true;
            }
            return success;
        };
        external.register('addGlobalProp', external.addGlobalProp);

        external.getCswInternalMethods = function() {
            /// <summary>
            ///   Fetch a dereferenced copy of the currently registered properties on the ChemSW namespace
            /// </summary>
            /// <returns type="Array">An array of property names.</returns>
            var methods = internal.methods.slice(0);
            return methods;
        };
        external.register('getCswInternalMethods', external.getCswInternalMethods);

        external.controls = $.extend({}, external);
        external.register('controls', external.controls);

        external.actions = $.extend({}, external);
        external.register('actions', external.actions);

        external.nbt = $.extend({}, external);
        external.register('nbt', external.nbt);
        
        return external;

    }());
}(window));
