/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _classDefinitionIIFE(nameSpace) {

    /**
     * Private constructor to create an object suitable for defining a new class
     * @param name {String} The name of this class
     * @param extend {String} The ExtJS class to extend/copy
     * @param requires {Array} [requires] An array of dependencies
     * @param alias {Array} [alias] An array of alternate names for this class
     * @param id {String} [id] A unique id for this class
     * @param store {nameSpace.store} [store] A data store for this class
     * @param plugins {Array} [plugins] An array of plugins to initialize with new instances of this class
     * @param constant {String} [constant] A nameSpace.constants constant to constrain property additions
     * @param namespace {String} A nameSpace namespace to constrain listeners
     * @param onDefine {Function} [onDefine] A method to call when the class definition is defined on the Ext namespace
     * @param debug {Boolean} [debug=false] For development debugging purposes. If true, output log content.
    */
    var ClassDefinition = function(name, extend, requires, alias, id, store, plugins, constant, namespace, onDefine, debug) {
        var that = this;
        var classDef = {};

        /**
         * Set of properties most Ext classes share
        */
        if (extend)     { nameSpace.property(classDef, 'extend', extend); }
        if (requires)   { nameSpace.property(classDef, 'requires', requires); }
        if (alias)      { nameSpace.property(classDef, 'alias', alias); }
        if (id)         { nameSpace.property(classDef, 'id', id); }
        if (plugins)    { nameSpace.property(classDef, 'plugins', plugins); }
        if (store)      { nameSpace.property(classDef, 'store', store); }

        /**
         * initComponents are created when the class is instanced; they are not part of the class definition--except as callbacks
         * This is unusual. Most classes do not need this mechanism. See tableGrid for example.
        */
        var initComponents = [];
        nameSpace.property(that, 'addInitComponent', function (method) {
            if (method) {
                initComponents.push(method);
            }
        }, false, false, false);

        /**
         * We don't allow listeners to be defined ad hoc; and if they are defined, they must be defined on the namespace listener object
        */
        if (namespace && nameSpace[namespace]) {
            var listeners = nameSpace[namespace].listeners.listeners();
            nameSpace.property(that, 'listeners', listeners);
            nameSpace.property(that.listeners, 'exception', function() {
                nameSpace.console.error('An error occurred in ' + name + '.', arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
            });
            /**
             * Interface to Add to the properties that will become part of the Ext class
            */
            if (nameSpace[namespace].constants.properties) {
                nameSpace.property(that, 'addProp', function (propName, value) {
                    if (!(nameSpace[namespace].constants.properties.has(propName))) {
                        throw new Error('Property named "' + propName + '" has not be defined on nameSpace.' + namespace + '.constants.properties.');
                    }
                    nameSpace.property(classDef, propName, value);
                }, false, false, false);
            }
        }
        
        /**
         * init must be manually called when the class is ready to be constructed (e.g. defined on Ext)
        */
        nameSpace.property(that, 'init', function () {
            nameSpace.property(classDef, 'initComponent', function () {
                var them = this;
                if (initComponents.length > 0) {
                    nameSpace.each(initComponents, function (func) {
                        func(them);
                    });
                }
                them.callParent(arguments);
            });

            if (listeners && Object.keys(listeners).length > 0) {
                /**
                 * Bit of a hack; but grids are a special case.
                */
                if (namespace === 'grids') {
                    nameSpace.property(classDef, 'viewConfig', {});
                    nameSpace.property(classDef.viewConfig, 'listeners', that.listeners);
                } else {
                    nameSpace.property(classDef, 'listeners', listeners);
                }
            }
            
            if (onDefine) {
                onDefine(classDef, that);
            }

            var ret = Ext.define(name, classDef);
            
            return ret;
        });

        return that;
    };

    nameSpace.instanceOf.lift('ClassDefinition', ClassDefinition);

    /**
     * Define declares a new class on the ExtJs namespace
     * @param def {Object} defintion object with possible properties: def.name def.extend, def.requires, def.alias, def.id, def.store, def.plugins, def.constant, def.onDefine
     * @param def.name {String} The name of this class
     * @param def.extend {String} The ExtJS class to extend/copy
     * @param def.requires {Array} [def.requires] An array of dependencies
     * @param def.alias {Array} [def.alias] An array of alternate names for this class
     * @param def.id {String} [def.id] A unique id for this class
     * @param def.store {nameSpace.store} [def.store] A data store for this class
     * @param def.plugins {Array} [def.plugins] An array of plugins to initialize with new instances of this class
     * @param def.constant {String} [def.constant] A nameSpace.constants constant to constrain property additions
     * @param def.namespace [String] A nameSpace namespace to constrain listeners
     * @param def.onDefine {Function} [def.onDefine] A method to call when the class definition is defined on the Ext namespace
    */
    nameSpace.lift('classDefinition', function(def) {
        if(!def) {
            throw new Error('Cannot create a definition without parameters.');
        }
        if (!(typeof def.name === 'string')) {
            throw new Error('Cannot define a class without a name');
        }
        var ret = new ClassDefinition(def.name, def.extend, def.requires, def.alias, def.id, def.store, def.plugins, def.constant, def.namespace, def.onDefine);
        return ret;
    });

}(window.$om$));