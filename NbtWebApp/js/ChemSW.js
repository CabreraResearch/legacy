/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

    window.abandonHope = false;

    window.ChemSW = window.Csw = (function () {

        var internal = {
            document: window.document,
            navigator: window.navigator,
            location: window.location,
            $: $,
            homeUrl: 'Main.html',
            methods: ['register'],
            uniqueIdCount: 0,
            protectedmethods: ['register', 'deregister', 'getGlobalProp', 'setGlobalProp']
        };

        var external = {};

        /* 
        * We are deliberately not assigning this method to the 'global' internal or external collection 
        * We don't want to inadvertantly allow for cross-polination among namespaces. 
        * makeNameSpace's only tie to the Csw closure is against internal for default values.
        */
        function makeNameSpace(externalCollection, anInternalCollection) {
            var internalCollection = {
                document: internal.document,
                navigator: internal.navigator,
                location: internal.location,
                $: internal.$,
                homeUrl: 'Main.html',
                methods: ['register'],
                uniqueIdCount: 0,
                protectedmethods: ['register', 'deregister', 'getGlobalProp', 'setGlobalProp']
            };
            /* If supplied, expose the internalCollection to the namespace */
            if (anInternalCollection) {
                $.extend(anInternalCollection, internalCollection);
            }

            externalCollection = externalCollection || {};

            externalCollection.register = externalCollection.register ||
                function (name, obj, isProtected) {
                    /// <summary>
                    ///   Register an Object in the ChemSW namespace
                    /// </summary>
                    /// <param name="name" type="String"> Name of the object </param>
                    /// <param name="obj" type="Object"> Object to pass </param>
                    /// <param name="isProtected" type="Boolean"> If true, the object cannot be removed from the namespace </param>
                    /// <returns type="Boolean">True if the object name did not already exist in the namespace.</returns>
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
                externalCollection.register('deregister', function (name) {
                    /// <summary>
                    ///   Deregister an Object from the ChemSW namespace
                    /// </summary>
                    /// <param name="name" type="String"> Name of the object.</param>
                    /// <returns type="Boolean">True if the object was removed.</returns>
                    var succeeded = false;
                    if (internalCollection.protectedmethods.indexOf(name) === -1) {
                        if (internalCollection.methods.indexOf(name) !== -1) {
                            internalCollection.methods.splice(name, 1);
                        }
                        delete external[name];
                        succeeded = true;
                    }
                    return succeeded;
                });

            externalCollection.getGlobalProp = externalCollection.getGlobalProp ||
                externalCollection.register('getGlobalProp', function (propName) {
                    /// <summary>
                    ///   Fetch a dereferenced copy of a property from the private universe collection
                    /// </summary>
                    /// <param name="propName" type="String"> Name of the property </param>
                    /// <returns type="Object">A clone of the property.</returns>
                    var retVal;
                    if (propName && internalCollection.hasOwnProperty(propName)) {
                        retVal = internalCollection[propName];
                    } else {
                        retVal = {};
                        $.extend(retVal, internalCollection);
                    }
                    return retVal;
                });

            externalCollection.setGlobalProp = externalCollection.setGlobalProp ||
                externalCollection.register('setGlobalProp', function (prop, val) {
                    /// <summary>
                    ///   Change the value of a property in the private universe collection
                    /// </summary>
                    /// <param name="name" type="String"> Name of the object </param>
                    /// <returns type="Boolean">True if the property was updated.</returns>
                    var success = false;
                    if (prop && val && internalCollection.hasOwnProperty(prop)) {
                        internalCollection[prop] = val;
                        success = true;
                    }
                    return success;
                });

            externalCollection.addGlobalProp = externalCollection.addGlobalProp ||
                externalCollection.register('addGlobalProp', function (propName, val) {
                    /// <summary>
                    ///   Add a property to the private universe collection
                    /// </summary>
                    /// <param name="propName" type="String"> Name of the object </param>
                    /// <param name="val" type="Object"> Value of the object </param>
                    /// <returns type="Boolean">True if the property was added.</returns>
                    var success = false;
                    if (propName && val && false === internalCollection.hasOwnProperty(propName)) {
                        internalCollection[propName] = val;
                        success = true;
                    }
                    return success;
                });

            externalCollection.getCswInternalMethods = externalCollection.getCswInternalMethods ||
                externalCollection.register('getCswInternalMethods', function () {
                    /// <summary>
                    ///   Fetch a dereferenced copy of the currently registered properties on the ChemSW namespace
                    /// </summary>
                    /// <returns type="Array">An array of property names.</returns>
                    var methods = internalCollection.methods.slice(0);
                    return methods;
                });
            externalCollection.makeNameSpace = externalCollection.makeNameSpace ||
                externalCollection.register('makeNameSpace', makeNameSpace);
            return externalCollection;
        }

        makeNameSpace(external, internal);

        external.actions = external.actions || external.register('actions', makeNameSpace());
        external.controls = external.controls || external.register('controls', makeNameSpace());
        external.nbt = external.nbt || external.register('nbt', makeNameSpace());

        return external;

    } ());

