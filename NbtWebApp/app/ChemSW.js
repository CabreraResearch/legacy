/// <reference path="~/app/CswApp-vsdoc.js" />

(function() {

    window.internetExplorerVersionNo = window.internetExplorerVersionNo || -1;

    /**
        The Csw Namespace object
        @global
        @module
        @namespace
        @exports ChemSW Namespace
    */
    window.ChemSW = window.Csw = (function() {
        'use strict';
        var cswPrivate = {
            document: window.document,
            navigator: window.navigator,
            location: window.location,
            $: $,
            homeUrl: 'Main.html',
            methods: ['register'],
            uniqueIdCount: 0,
            protectedmethods: ['register', 'deregister', 'getGlobalProp', 'setGlobalProp', 'displayAllExceptions']
        };

        var cswPublic = { };

        /* 
        * We are deliberately not assigning this method to the 'global' internal or external collection 
        * We don't want to inadvertantly allow for cross-pollination among namespaces. 
        * makeNameSpace's only tie to the Csw closure is against internal for default values.
        */

        function makeNameSpace(externalCollection, anInternalCollection) {
            'use strict';
            var internalCollection = {
                document: cswPrivate.document,
                navigator: cswPrivate.navigator,
                location: cswPrivate.location,
                $: cswPrivate.$,
                homeUrl: 'Main.html',
                methods: ['register'],
                uniqueIdCount: 0,
                protectedmethods: ['register', 'deregister', 'getGlobalProp', 'setGlobalProp']
            };
            /* If supplied, expose the internalCollection to the namespace */
            if (anInternalCollection) {
                $.extend(anInternalCollection, internalCollection);
            }

            externalCollection = externalCollection || { };

            externalCollection.register = externalCollection.register ||
                function(name, obj, isProtected) {
                    /// <summary>
                    ///   Register an Object in the ChemSW namespace
                    /// </summary>
                    /// <param name="name" type="String"> Name of the object </param>
                    /// <param name="obj" type="Object"> Object to pass </param>
                    /// <param name="isProtected" type="Boolean"> If true, the object cannot be removed from the namespace </param>
                    /// <returns type="Boolean">True if the object name did not already exist in the namespace.</returns>
                    'use strict';
                    var succeeded = false;
                    if (internalCollection.methods.indexOf(name) === -1) {
                        internalCollection.methods.push(name);
                        obj[name] = true; //for shimming our own instanceof
                        if (isProtected && internalCollection.protectedmethods.indexOf(name) === -1) {
                            internalCollection.protectedmethods.push(name);
                        }
                        externalCollection[name] = obj;
                        succeeded = true;
                    }
                    return obj;
                };

            externalCollection.deregister = externalCollection.deregister ||
                externalCollection.register('deregister', function(name) {
                    /// <summary>
                    ///   Deregister an Object from the ChemSW namespace
                    /// </summary>
                    /// <param name="name" type="String"> Name of the object.</param>
                    /// <returns type="Boolean">True if the object was removed.</returns>
                    'use strict';
                    var succeeded = false;
                    if (internalCollection.protectedmethods.indexOf(name) === -1) {
                        if (internalCollection.methods.indexOf(name) !== -1) {
                            internalCollection.methods.splice(name, 1);
                        }
                        delete cswPublic[name];
                        succeeded = true;
                    }
                    return succeeded;
                });

            externalCollection.getGlobalProp = externalCollection.getGlobalProp ||
                externalCollection.register('getGlobalProp', function(propName) {
                    /// <summary>
                    ///   Fetch a dereferenced copy of a property from the private universe collection
                    /// </summary>
                    /// <param name="propName" type="String"> Name of the property </param>
                    /// <returns type="Object">A clone of the property.</returns>
                    'use strict';
                    var retVal;
                    if (propName && internalCollection.hasOwnProperty(propName)) {
                        retVal = internalCollection[propName];
                    } else {
                        retVal = { };
                        $.extend(retVal, internalCollection);
                    }
                    return retVal;
                });

            externalCollection.setGlobalProp = externalCollection.setGlobalProp ||
                externalCollection.register('setGlobalProp', function(prop, val) {
                    /// <summary>
                    ///   Change the value of a property in the private universe collection
                    /// </summary>
                    /// <param name="name" type="String"> Name of the object </param>
                    /// <returns type="Boolean">True if the property was updated.</returns>
                    'use strict';
                    var success = false;
                    if (prop && val && internalCollection.hasOwnProperty(prop)) {
                        internalCollection[prop] = val;
                        success = true;
                    }
                    return success;
                });

            externalCollection.addGlobalProp = externalCollection.addGlobalProp ||
                externalCollection.register('addGlobalProp', function(propName, val) {
                    /// <summary>
                    ///   Add a property to the private universe collection
                    /// </summary>
                    /// <param name="propName" type="String"> Name of the object </param>
                    /// <param name="val" type="Object"> Value of the object </param>
                    /// <returns type="Boolean">True if the property was added.</returns>
                    'use strict';
                    var success = false;
                    if (propName && val && false === internalCollection.hasOwnProperty(propName)) {
                        internalCollection[propName] = val;
                        success = true;
                    }
                    return success;
                });

            externalCollection.getCswInternalMethods = externalCollection.getCswInternalMethods ||
                externalCollection.register('getCswInternalMethods', function() {
                    /// <summary>
                    ///   Fetch a dereferenced copy of the currently registered properties on the ChemSW namespace
                    /// </summary>
                    /// <returns type="Array">An array of property names.</returns>
                    'use strict';
                    var methods = internalCollection.methods.slice(0);
                    return methods;
                });
            externalCollection.makeNameSpace = externalCollection.makeNameSpace ||
                externalCollection.register('makeNameSpace', makeNameSpace);
            return externalCollection;
        }

        makeNameSpace(cswPublic, cswPrivate);

        cswPublic.actions = cswPublic.actions || cswPublic.register('actions', makeNameSpace());
        cswPublic.ajax = cswPublic.ajax || cswPublic.register('ajax', makeNameSpace());
        cswPublic.ajaxWcf = cswPublic.ajaxWcf || cswPublic.register('ajaxWcf', makeNameSpace());
        cswPublic.actions = cswPublic.actions || cswPublic.register('actions', makeNameSpace());
        cswPublic.clientChanges = cswPublic.clientChanges || cswPublic.register('clientChanges', makeNameSpace());
        cswPublic.clientSession = cswPublic.clientSession || cswPublic.register('clientSession', makeNameSpace());
        cswPublic.clientState = cswPublic.clientState || cswPublic.register('clientState', makeNameSpace());
        cswPublic.clientDb = cswPublic.clientDb || cswPublic.register('clientDb', makeNameSpace());
        cswPublic.composites = cswPublic.composites || cswPublic.register('composites', makeNameSpace());
        cswPublic.controls = cswPublic.controls || cswPublic.register('controls', makeNameSpace());
        cswPublic.cookie = cswPublic.cookie || cswPublic.register('cookie', makeNameSpace());
        cswPublic.enums = cswPublic.enums || cswPublic.register('enums', makeNameSpace());
        cswPublic.error = cswPublic.error || cswPublic.register('error', makeNameSpace());
        cswPublic.layouts = cswPublic.layouts || cswPublic.register('layouts', makeNameSpace());
        cswPublic.literals = cswPublic.literals || cswPublic.register('literals', makeNameSpace());
        cswPublic.main = cswPublic.main || cswPublic.register('main', makeNameSpace());
        cswPublic.nbt = cswPublic.nbt || cswPublic.register('nbt', makeNameSpace());
        cswPublic.properties = cswPublic.properties || cswPublic.register('properties', makeNameSpace());
        cswPublic.window = cswPublic.window || cswPublic.register('window', makeNameSpace());
        cswPublic.wizard = cswPublic.wizard || cswPublic.register('wizard', makeNameSpace());

        cswPublic.isFunction = cswPublic.isFunction ||
            cswPublic.register('isFunction', function(obj) {
                'use strict';
                /// <summary> Returns true if the object is a function</summary>
                /// <param name="obj" type="Object"> Object to test</param>
                /// <returns type="Boolean" />
                var ret = ($.isFunction(obj));
                return ret;
            });

        cswPublic.tryExec = cswPublic.tryExec ||
            cswPublic.register('tryExec', function(func) {
                'use strict';
                /// <summary> If the supplied argument is a function, execute it. </summary>
                /// <param name="func" type="Function"> Function to evaluate </param>
                /// <returns type="undefined" />
                var ret = false;
                try {
                    if (Csw.isFunction(func)) {
                        ret = func.apply(this, Array.prototype.slice.call(arguments, 1));
                    }
                } catch(exception) {
                    if ((exception.name !== 'TypeError' ||
                        exception.type !== 'called_non_callable') &&
                        exception.type !== 'non_object_property_load') { /* ignore errors failing to exec self-executing functions */
                        Csw.error.catchException(exception);
                    }
                } finally {
                    return ret;
                }
            });

        cswPublic.method = cswPublic.method ||
            cswPublic.register('method', function(func) {
                'use strict';
                var that = this;
                return function() {
                    var args = Array.prototype.slice.call(arguments, 0);
                    args.unshift(func);
                    return Csw.tryExec.apply(that, args);
                };
            });

        return cswPublic;

    }());

}());

