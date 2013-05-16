/*global nameSpaceName:true, jQuery: true, window: true */
(function (nameSpaceName, domVendor) {

    /**
     *    The nameSpaceName  NameSpace, an IIFE
     *    @namespace
     *    @export
     *    @return {window.nameSpaceName}
     */
    Object.defineProperty(window, nameSpaceName, {
        value: (function () {
            ///<summary>(IIFE) Intializes the nameSpaceName namespace.</summary>
            ///<returns type="window.nameSpaceName">The nameSpaceName namespace.</returns>

            var nsInternal = {
                dependents: []
            };

            Object.defineProperty(nsInternal, 'getNsMembers', {
                value: function () {
                    var members = [];

                    function recurseTree(key, lastKey) {
                        if (typeof (key) === 'string') {
                            members.push(lastKey + '.' + key);
                        }
                        if (domVendor.isPlainObject(key)) {
                            Object.keys(key).forEach(function (k) {
                                if (typeof (k) === 'string') {
                                    members.push(lastKey + '.' + k);
                                }
                                if (domVendor.isPlainObject(key[k])) {
                                    recurseTree(key[k], lastKey + '.' + k);
                                }
                            });
                        }
                    }
                    Object.keys(NsTree[nameSpaceName]).forEach(function (key) {
                        if (domVendor.isPlainObject(NsTree[nameSpaceName][key])) {
                            recurseTree(NsTree[nameSpaceName][key], nameSpaceName);
                        }
                    });
                    return members;
                }
            });

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

            var NsTree = Object.create(null);
            NsTree[nameSpaceName] = Object.create(null);

            /**
             *    Internal nameSpaceName method to create new "sub" namespaces on arbitrary child objects.
             *	@param (Object) proto An instance of an Object to use as the basis of the new namespace prototype
             */
            var makeNameSpace = function (tree, spacename) {
                /// <summary>Internal nameSpaceName method to create new "sub" namespaces on arbitrary child objects.</summary>
                /// <param name="proto" type="Object"> String to parse </param>
                /// <returns type="Object">The new child namespace.</returns>
                var Class = new Function(
                    "return function " + spacename + "(){}"
                )();
                function NsClass(tree, nameSpaceName) {
                    var proto = this;
                    Object.defineProperty(proto, 'lift', {
                        value:
                            /**
                             *	"Lift" an Object into the prototype of the namespace.
                             *	This Object will be readable/executable but is otherwise immutable.
                             *   @param (String) name The name of the object to lift
                             *   @param (Object) obj Any, arbitrary Object to use as the value.
                             *   @return (Object) The value of the new property.
                             */
                            function (name, obj) {
                                'use strict';
                                /// <summary>"Lift" an Object into the prototype of the namespace. This Object will be readable/executable but is otherwise immutable.</summary>
                                /// <param name="name" type="String">The name of the object to lift.</param>
                                /// <param name="obj" type="Object">Any, arbitrary Object to use as the value.</param>
                                /// <returns type="Object">The value of the new property.</returns>
                                if (name && obj) {
                                    Object.defineProperty(proto, name, {
                                        value: obj
                                    });
                                    tree[name] = typeof (obj);
                                    nsInternal.alertDependents(nameSpaceName + '.' + spacename + '.' + name);
                                }
                                return obj;
                            }
                    });

                    proto.lift('makeSubNameSpace',
                        /**
                         *	Create a new, static namespace on the current parent (e.g. nameSpaceName.to... || nameSpaceName.is...)
                         *   @param (String) subNameSpace The name of the new namespace.
                         *   @return (Object) The new namespace.
                         */
                        function (subNameSpace) {
                            'use strict';
                            /// <summary>Create a new, static namespace on the current parent (e.g. nameSpaceName.to... || nameSpaceName.is...).</summary>
                            /// <param name="subNameSpace" type="String">The name of the new namespace.</param>
                            /// <returns type="Object">The new namespace.</returns>
                            tree[subNameSpace] = Object.create(null);
                            nsInternal.alertDependents(nameSpaceName + '.' + subNameSpace);
                            
                            var newNameSpace = makeNameSpace(tree[subNameSpace], subNameSpace);
                            tree[subNameSpace].constants = Object.create(null);
                            newNameSpace.lift('constants', makeNameSpace(tree[subNameSpace].constants, 'constants'));
                            
                            proto.lift(subNameSpace, newNameSpace);
                            return newNameSpace;
                        });

                    
                }
                NsClass.prototype = Class;

                //var Base = Object.create(proto.prototype);
                var ret = new NsClass(tree, spacename);
                

                return ret;
            };

            var NsOut = makeNameSpace(NsTree[nameSpaceName], nameSpaceName);
            NsTree[nameSpaceName].constants = Object.create(null);
            NsOut.lift('constants', makeNameSpace(NsTree[nameSpaceName].constants, 'constants'));
            
            NsOut.lift('?', domVendor);


            /**
             *    "Depend" an Object upon another member of this namespace, upon another namespace,
             *   or upon a member of another namespace
             *   @param (Array) array of dependencies for this method
             *   @param (Function) obj Any, arbitrary Object to use as the value
             */
            var dependsOn = function (dependencies, callBack, imports) {
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
            Object.defineProperty(NsOut, 'dependsOn', {
                value: dependsOn
            });


            Object.defineProperty(NsOut, 'tree', {
                value: NsTree
            });

            return NsOut;

        }())
    });

    /**
     * Custom Errors
    */
    window[nameSpaceName].makeSubNameSpace('errors');

    /**
     * Type checking
    */
    window[nameSpaceName].makeSubNameSpace('is');

    /**
     *Grids
    */
    window[nameSpaceName].makeSubNameSpace('grids');

        /**
         * Grids Fields
        */
        window[nameSpaceName].grids.makeSubNameSpace('fields');

        /**
         * Grids Columns
        */
        window[nameSpaceName].grids.makeSubNameSpace('columns');

        /**
         * Grids Listeners
        */
        window[nameSpaceName].grids.makeSubNameSpace('listeners');

        /**
         * Grids Stores
        */
        window[nameSpaceName].grids.makeSubNameSpace('stores');

    /**
     * Panels
    */
    window[nameSpaceName].makeSubNameSpace('panels');

        /**
         * Panel Listeners
        */
        window[nameSpaceName].panels.makeSubNameSpace('listeners');

    /**
     * Trees
    */
        window[nameSpaceName].makeSubNameSpace('trees');

    /**
     * Tree Listeners
    */
        window[nameSpaceName].trees.makeSubNameSpace('listeners');

    /**
     * To instance check classes
    */
    window[nameSpaceName].makeSubNameSpace('instanceOf');

    /**
     * The MetaData namespace. Represents the structures of nameSpaceName nodes, elements and properties.
     */
    window[nameSpaceName].makeSubNameSpace('metadata');

    /**
     * The node namespace. Represents an nameSpaceName Node and its properties.
     * [1]: This class is responsible for constructing the DOM getters (properties on this object which reference Nodes in the DOM tree)
     * [2]: This class exposes helper methods which can get/set properties on this instance of the node.
     * [3]: This class validates the execution of these methods (e.g. Is the node still in the DOM; has it been GC'd behind our backs)
     * [4]: Maintaining an im-memory representation of tree with children/parents
     */
    window[nameSpaceName].makeSubNameSpace('node');

    window[nameSpaceName].makeSubNameSpace('to');


}('Csw2', jQuery));