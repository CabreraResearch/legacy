/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function (nameSpace) {

    /**
     * Create a new object with constant properties.
     * @param props {Object} an object represent the enun members
    */
    var Constant = function (props) {
        var that = null;
        var keys = [];

        if (props) {
            that = this;
            nameSpace.property(that, 'has',
                /**
                * Assert that the provided key is a member of the enum
                * @param key {String} enum property name
                */
                function (key) {
                    return keys.indexOf(key) !== -1;
                });

            nameSpace.each(props, function (propVal, propName) {
                keys.push(propVal);
                Object.defineProperty(that, propName, {
                    value: propVal
                });
            });
        }
        return that;
    };

    /**
     * Create a new enum on the constants namespace.
     * Enums are objects consisting of read-only, non-configurable, non-enumerable properties.
     * @param name {String} the name of the enum
     * @param props {Object} the properties of the enum
    */
    nameSpace.lift('constant', function (nameSpace, name, props) {
        var ret = new Constant(props);
        nameSpace = nameSpace || nameSpace;
        if (ret && nameSpace.constants && nameSpace.constants.lift && name) {
            nameSpace.constants.lift(name, ret);
            Object.seal(ret);
            Object.freeze(ret);
        }
        return ret;
    });

}(window.$om$));