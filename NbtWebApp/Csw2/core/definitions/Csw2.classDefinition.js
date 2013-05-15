/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _classDefinitionIIFE() {

    /**
     * Private constructor to create an object suitable for defining a new class
     * @param name {String} The name of this class
     * @param extend {String} The ExtJS class to extend/copy
     * @param requires {Array} [requires] An array of dependencies
     * @param alias {Array} [alias] An array of alternate names for this class
     * @param id {String} [id] A unique id for this class
     * @param store {Csw2.store} [store] A data store for this class
     * @param plugins [Array] [plugins] An array of plugins to initialize with new instances of this class
     * @param constant [String] [constant] A Csw.constants constant to constrain property additions
     * @param namespace [String] A Csw namespace to constrain listeners
     * @param onDefine [Function] [onDefine] A method to call when the class definition is defined on the Ext namespace
    */
    var ClassDefinition = function(name, extend, requires, alias, id, store, plugins, constant, namespace, onDefine) {
        var that = this;
        var classDef = {};

        /**
         * Set of properties most Ext classes share
        */
        if (extend)     { Csw2.property(classDef, 'extend', extend); }
        if (requires)   { Csw2.property(classDef, 'requires', requires); }
        if (alias)      { Csw2.property(classDef, 'alias', alias); }
        if (id)         { Csw2.property(classDef, 'id', id); }
        if (plugins)    { Csw2.property(classDef, 'plugins', plugins); }
        if (store)      { Csw2.property(classDef, 'store', store); }

        /**
         * initComponents are created when the class is instanced; they are not part of the class definition--except as callbacks
         * This is unusual. Most classes do not need this mechanism. See tableGrid for example.
        */
        var initComponents = [];
        Csw2.property(that, 'addInitComponent', function (method) {
            if (method) {
                initComponents.push(method);
            }
        }, false, false, false);

        /**
         * We don't allow listeners to be defined ad hoc; and if they are defined, they must be defined on the namespace listener object
        */
        if (namespace) {
            var listeners = Csw2[namespace].listeners.listeners();
            Csw2.property(that, 'listeners', listeners);
        }
        
        /**
         * Interface to Add to the properties that will become part of the Ext class
        */
        if (constant && Csw2.constants[constant]) {
            Csw2.property(that, 'addProp', function(propName, value) {
                if (!(Csw2.constants[constant].has(propName))) {
                    throw new Error('Property named "' + propName + '" has not be defined on Csw2.constants.' + constant + '.');
                }
                Csw2.property(classDef, propName, value);
            }, false, false, false);
        }
        
        /**
         * init must be manually called when the class is ready to be constructed (e.g. defined on Ext)
        */
        Csw2.property(that, 'init', function () {
            Csw2.property(classDef, 'initComponent', function () {
                var them = this;
                if (initComponents.length > 0) {
                    Csw2.each(initComponents, function (func) {
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
                    Csw2.property(classDef, 'viewConfig', {});
                    Csw2.property(classDef.viewConfig, 'listeners', that.listeners);
                } else {
                    Csw2.property(classDef, 'listeners', listeners);
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

    Csw2.instanceof.lift('ClassDefinition', ClassDefinition);

    /**
     * Define declares a new class on the ExtJs namespace
     * @param def {Object} defintion object with possible properties: def.name def.extend, def.requires, def.alias, def.id, def.store, def.plugins, def.constant, def.onDefine
     * @param def.name {String} The name of this class
     * @param def.extend {String} The ExtJS class to extend/copy
     * @param def.requires {Array} [def.requires] An array of dependencies
     * @param def.alias {Array} [def.alias] An array of alternate names for this class
     * @param def.id {String} [def.id] A unique id for this class
     * @param def.store {Csw2.store} [def.store] A data store for this class
     * @param def.plugins {Array} [def.plugins] An array of plugins to initialize with new instances of this class
     * @param def.constant {String} [def.constant] A Csw.constants constant to constrain property additions
     * @param def.namespace [String] A Csw namespace to constrain listeners
     * @param def.onDefine {Function} [def.onDefine] A method to call when the class definition is defined on the Ext namespace
    */
    Csw2.lift('classDefinition', function(def) {
        if(!def) {
            throw new Error('Cannot create a definition without parameters.');
        }
        if (!(typeof def.name === 'string')) {
            throw new Error('Cannot define a class without a name');
        }
        var ret = new ClassDefinition(def.name, def.extend, def.requires, def.alias, def.id, def.store, def.plugins, def.constant, def.namespace, def.onDefine);
        return ret;
    });

}());