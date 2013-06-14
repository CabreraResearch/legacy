/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _dataTypeIIFE(n$) {

    /**
     * Create a new dataType
     * @param namme {String} A unique name for this dataType
     * @param type {String} [type='string'] The display type of this dataType
     * @param defaultValue {String} [defaultValue] A default value
    */
    n$.dataTypes.register('dataType', function (name, type, defaultValue) {
        return n$.dataTypes._dataType(name, type, defaultValue);
    });


}(window.$nameSpace$));
