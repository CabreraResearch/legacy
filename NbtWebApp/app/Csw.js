/*global Csw:true, jQuery: true, window: true */
(function (Csw, $) {

    var cswWindow = this;
    cswWindow.document = cswWindow.document || {};
    cswWindow.navigator = cswWindow.navigator || {};

    $ = $ || this.$;

    Object.defineProperties(Object.prototype, {
        getInstanceName: {
            value: function () {
                var funcNameRegex = /function (.{1,})\(/;
                var results = (funcNameRegex).exec((this).constructor.toString());
                return (results && results.length > 1) ? results[1] : "";
            }
        }
    });

    /**
     * An internal representation of the namespace tree
     * @name NsTree
     * @internal
    */
    var NsTree = {};

    /**
     *    The nameSpaceName  NameSpace, an IIFE
     *    @namespace
     *    @export
     *    @global */ +Csw + /*
     *    @return {window.nameSpaceName}
     */
    Object.defineProperty(window, Csw, {
        value:
            /**
             * Intializes the nameSpaceName namespace.
             * @return {window.nameSpaceName}
            */
            (function () {

                var nsInternal = {
                    dependents: []
                };
                /**
                    * Fetches the registered properties and methods on the namespace and its child namespaces
                    * @name  getNsMembers
                    * @interal
                    * @return {Array} An array of members defined as strings (e.g. 'namespace.constants.astringcnst')
                */
                Object.defineProperty(nsInternal, 'getNsMembers', {
                    value:
                        
                        function () {
                            var members = [];

                            function recurseTree(key, lastKey) {
                                if (typeof (key) === 'string') {
                                    members.push(lastKey + '.' + key);
                                }
                                if ($.isPlainObject(key)) {
                                    Object.keys(key).forEach(function (k) {
                                        if (typeof (k) === 'string') {
                                            members.push(lastKey + '.' + k);
                                        }
                                        if ($.isPlainObject(key[k])) {
                                            recurseTree(key[k], lastKey + '.' + k);
                                        }
                                    });
                                }
                            }
                            Object.keys(NsTree[Csw]).forEach(function (key) {
                                if ($.isPlainObject(NsTree[Csw][key])) {
                                    recurseTree(NsTree[Csw][key], Csw);
                                }
                            });
                            return members;
                        }
                });

                /**
                    * To support dependency management, when a property is lifted onto the namespace, notify dependents to initialize
                    * @name alertDependents
                    * @internal
                */
                Object.defineProperty(nsInternal, 'alertDependents', {
                    value: function (imports) {
                        var deps = nsInternal.dependents.filter(function (depOn) {
                            return false === depOn(imports);
                        });
                        if (Array.isArray(deps)) {
                            nsInternal.dependents = deps;
                        }
                    }
                });

                /**
                 * Internal nameSpaceName method to create new "sub" namespaces on arbitrary child objects.
                 * @internal	
                 * @param spacename {String} the namespace name
                 * @param tree {Object} the internal tree representation of the current level of the namespace
                 * @return namespace {Object} A new namespace
                 */
                function makeNameSpace(spacename, tree) {
                    /**
                     * An internal mechanism to represent the instance of this namespace
                     * @constructor
                    */
                    var Class = new Function(
                        "return function " + spacename + "(){}"
                    )();
                    
                    var deferred = Q.defer();

                    /**
                     * The derived instance to be constructed
                     * @name Base
                     * @constructor
                    */
                    function Base(nsName) {
                        var proto = this;
                        tree[nsName] = tree[nsName] || {};
                        var nsTree = tree[nsName];

                        /**
                        *	Register (e.g. "Lift") an Object into the prototype of the namespace.
                        *	This Object will be readable/executable but is otherwise immutable.
                        *   @name register
                        *   @param {String} name The name of the object to lift
                        *   @param {Object} obj Any, arbitrary Object to use as the value.
                        *   @return {Object} The value of the new property.
                        */
                        Object.defineProperty(this, 'register', {
                            value:
                                
                                function (name, obj, enumerable) {
                                    'use strict';
                                    if (!(typeof name === 'string') || name === '') {
                                        throw new Error('Cannot lift a new property without a valid name.');
                                    }
                                    if (!obj) {
                                        throw new Error('Cannot lift a new property without a valid property instance.');
                                    }
                                    if (proto[name]) {
                                        throw new Error('Property named ' + name + ' is already defined on ' + spacename + '.');
                                    }

                                    //Guard against obliterating the tree as the tree is recursively extended
                                    nsTree[name] = nsTree[name] || {
                                        name: name,
                                        type: typeof obj,
                                        instance: obj.getInstanceName ? obj.getInstanceName() : 'unknown'
                                    };

                                    Object.defineProperty(proto, name, {
                                        value: obj,
                                        enumerable: false !== enumerable
                                    });
                                    nsInternal.alertDependents(nsName + '.' + spacename + '.' + name);

                                    return obj;
                                }
                        });

                        /**
                        *	Create a new, static namespace on the current parent (e.g. nsName.to... || nsName.is...)
                        *   @name makeSubNameSpace
                        *   @param {String} subNameSpace The name of the new namespace.
                        *   @return {Object} The new namespace.
                        */
                        proto.register('makeSubNameSpace',
                            
                            function (subNameSpace) {
                                'use strict';
                                if (!(typeof subNameSpace === 'string') || subNameSpace === '') {
                                    throw new Error('Cannot create a new sub namespace without a valid name.');
                                }
                                if (proto.subNameSpace) {
                                    throw new Error('Sub namespace named ' + subNameSpace + ' is already defined on ' + spacename + '.');
                                }
                                nsInternal.alertDependents(nsName + '.' + subNameSpace);

                                var newNameSpace = makeNameSpace(subNameSpace, nsTree);

                                if (subNameSpace !== 'constants') {
                                    newNameSpace.register('constants', makeNameSpace('constants', nsTree), false);
                                }

                                proto.register(subNameSpace, newNameSpace, false);
                                return newNameSpace;
                            }, false);
                        
                        /**
                        *   @name onReady
                        */
                        proto.register('onReady', deferred.promise);
                        
                        /**
                        *   @name isReady
                        */
                        proto.register('isReady', function (ready) {
                            if (true === ready) {
                                deferred.resolve();
                            }
                            return proto.onReady;
                        });

                    }

                    Class.prototype = new Base(spacename);
                    //Class.prototype.parent = Base.prototype;

                    return new Class(spacename);
                };
        

                //Create the root of the tree as the current namespace
                NsTree[Csw] = {};

                //Define the core namespace and the return of this class
                var NsOut = makeNameSpace(Csw, NsTree[Csw]);
                Object.defineProperties(window, { $nameSpace$: { value: NsOut } });

                //Cache a handle on the vendor (probably jQuery) on the root namespace
                NsOut.register('?', $, false);
                
                //Cache the tree (useful for documentation/visualization/debugging)
                NsOut.register('tree', NsTree[Csw], false);
                
                //Cache the name space name
                NsOut.register('name', Csw, false);


                /**
                 *    "Depend" an Object upon another member of this namespace, upon another namespace,
                 *   or upon a member of another namespace
                 *   @param (Array) array of dependencies for this method
                 *   @param (Function) obj Any, arbitrary Object to use as the value
                 */
                function dependsOn(dependencies, callBack, imports) {
                    'use strict';
                    var ret = false;
                    var nsMembers = nsInternal.getNsMembers();
                    if (dependencies && dependencies.length > 0 && callBack) {
                        var missing = dependencies.filter(function (depen) {
                            return (nsMembers.indexOf(depen) === -1 && (!imports || imports !== depen));
                        });
                        if (missing.length === 0) {
                            ret = true;
                            callBack();
                        }
                        else {
                            nsInternal.dependents.push(function (imports) {
                                return dependsOn(missing, callBack, imports);
                            });
                        }
                    }
                    return ret;
                };
                NsOut.register('dependsOn', dependsOn, false);

                return NsOut;

            }())
    });

}('Csw', jQuery));