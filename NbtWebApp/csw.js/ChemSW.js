/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

window.abandonHope = false;
(function (window, $, undefined) {
    'use strict';
    var document = window.document,
        navigator = window.navigator,
        location = window.location;
   
    window.ChemSW = window.Csw = (function () {

        var cswUniverse = {
            document: document,
            navigator: navigator,
            location: location,
            $: $,
            homeUrl: 'Main.html'
        };

        var methods = ['register'],
            protectedMethods = ['register', 'deregister', 'getGlobalProp', 'setGlobalProp'],
            ret = { };
       
        var register = function (name, obj, isProtected) {
            /// <summary>
            ///   Register an Object in the ChemSW namespace
            /// </summary>
            /// <param name="name" type="String"> Name of the object </param>
            /// <param name="obj" type="Object"> Object to pass </param>
            /// <param name="isProtected" type="Boolean"> If true, the object cannot be removed from the namespace </param>
            /// <returns type="Boolean">True if the object name did not already exist in the namespace.</returns>
            var succeeded = false;
            if (methods.indexOf(name) !== -1) {
                methods.push(name);
                obj[name] = true; //for shimming our own instanceof 
                if (isProtected && protectedMethods.indexOf(name) !== -1) {
                    protectedMethods.push(name);
                }
                ret[name] = obj;
                succeeded = true;
            }
            return succeeded;
        };
        ret.register = register;

        var deregister = function (name) {
            /// <summary>
            ///   Deregister an Object from the ChemSW namespace
            /// </summary>
            /// <param name="name" type="String"> Name of the object.</param>
            /// <returns type="Boolean">True if the object was removed.</returns>
            var succeeded = false;
            if (protectedMethods.indexOf (name) !== -1) {
                if (methods.indexOf (name) !== -1) {
                    methods.splice (name, 1);
                }
                delete ret[name];
                succeeded = true;
            }
            return succeeded;
        };
        register('deregister', deregister);
        ret.deregister = ret.deregister || deregister;

        var getGlobalProp = function (propName) {
            /// <summary>
            ///   Fetch a dereferenced copy of a property from the private universe collection
            /// </summary>
            /// <param name="propName" type="String"> Name of the property </param>
            /// <returns type="Object">A clone of the property.</returns>
            var retVal;
            if (propName && cswUniverse.hasOwnProperty (propName)) {
                retVal = cswUniverse[propName];
            } else {
                retVal = {};
                $.extend (retVal, cswUniverse);
            }
            return retVal;
        };
        register('getGlobalProp', getGlobalProp);
        ret.getGlobalProp = ret.getGlobalProp || getGlobalProp;

        var setGlobalProp = function (prop, val) {
            /// <summary>
            ///   Change the value of a property in the private universe collection
            /// </summary>
            /// <param name="name" type="String"> Name of the object </param>
            /// <returns type="Boolean">True if the property was updated.</returns>
            var success = false;
            if (prop && val && cswUniverse.hasOwnProperty (prop)) {
                cswUniverse[prop] = val;
                success = true;
            }
            return success;
        };
        register('setGlobalProp', setGlobalProp);
        ret.setGlobalProp = ret.setGlobalProp || setGlobalProp;
        
        var addGlobalProp = function (propName, val) {
            /// <summary>
            ///   Add a property to the private universe collection
            /// </summary>
            /// <param name="propName" type="String"> Name of the object </param>
            /// <param name="val" type="Object"> Value of the object </param>
            /// <returns type="Boolean">True if the property was added.</returns>
            var success = false;
            if (propName && val && false == cswUniverse.hasOwnProperty (propName)) {
                cswUniverse[propName] = val;
                success = true;
            }
            return success;
        };
        register('addGlobalProp', addGlobalProp);
        ret.addGlobalProp = ret.addGlobalProp || addGlobalProp;

        var getCswMethods = function () {
            /// <summary>
            ///   Fetch a dereferenced copy of the currently registered properties on the ChemSW namespace
            /// </summary>
            /// <returns type="Array">An array of property names.</returns>
            var retMethods = methods.slice (0);
            return retMethods;
        };
        register('getCswMethods', getCswMethods);
        ret.getCswMethods = ret.getCswMethods || getCswMethods;

        return ret;
                       
    }());                  
}(window, jQuery));