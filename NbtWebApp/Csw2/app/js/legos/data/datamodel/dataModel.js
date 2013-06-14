/* jshint undef: true, unused: true */
/// <reference path="../release/nsApp-vsdoc.js" />
/* global n$:true, window:true, Ext:true, $: true */

(function _modelIIFE(n$) {

    /**
     * Create a dataModel object.
     * @param modelDef.name {String} The ClassName of the dataModel to associate with ExtJS
     * @param modelDef.extend {String} [extend='Ext.model.Model'] An ExtJs class name to extend, usually the Model model
     * @param modelDef.dataTypeCollection {n$.dataTypes.collection} [fields=new Array()] An array of fields to load with the dataModel. Fields can be a n$.dataTypes.collection or an array (e.g. ['name', 'string', 'Bob'])
     * @returns {Csw.dataModels.dataModel} A dataModel object. Exposese subscribers and columns collections. Self-initializes.
    */
    n$.dataModels.register('dataModel', function (modelDef) {
        return n$.dataModels._dataModel(modelDef);
    });


}(window.$nameSpace$));
