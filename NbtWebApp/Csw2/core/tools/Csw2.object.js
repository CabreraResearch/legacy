/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _propertyIIFE() {

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
        Csw2.property(obj, 'add', function(name, val, writable, configurable, enumerable) {
            return Csw2.property(obj, name, val, writable, configurable, enumerable);
        }, false, false, false);
        return obj;
    };

    Csw2.lift('object', object);

}());