/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _propertyIIFE(nameSpace) {

    /**
     * Create an instance of Object
     * @param properties {Object} [properties={}] properties to define on the Object
     * @param inheritsFromPrototype {Prototype} [inheritsFromPrototype=null] The prototype to inherit from
    */
    var object = function (properties, inheritsFromPrototype) {
        
        if (!inheritsFromPrototype) {
            inheritsFromPrototype = null;
        }
        if (!properties) {
            properties = {};
        }
        var obj = Object.create(inheritsFromPrototype, properties);

        nameSpace.property(obj, 'add',
            /**
             * Add a property to the object and return it
            */
            function (name, val, writable, configurable, enumerable) {
            return nameSpace.property(obj, name, val, writable, configurable, enumerable);
        }, false, false, false);

        return obj;
    };

    nameSpace.lift('object', object);

}(window.$om$));