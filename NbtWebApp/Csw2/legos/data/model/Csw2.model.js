/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _modelIIFE(nameSpace) {


    /**
     * Define the properties which are available to Model.
    */
    var modelProperties = Object.create(null);
    modelProperties.fields = 'fields';
    nameSpace.constant(nameSpace.models, 'properties', modelProperties);

    /**
     * Private class representing the construnction of a model. It returns a nameSpace.model.model instance with collections for adding columns and listeners.
     * @internal
     * @constructor
     * @param name {String} The ClassName of the model to associate with ExtJS
     * @param extend {String} [extend='Ext.model.Model'] An ExtJs class name to extend, usually the Model model
     * @param .dataTypeCollection {nameSpace.dataTypes.collection} [fields=new Array()] An array of fields to load with the model. Fields can be a nameSpace.dataTypes.collection or an array (e.g. ['name', 'string', 'Bob'])
    */
    var Model = function (name, extend, dataTypeCollection) {
        var extFieldsCollection = nameSpace.dataTypes.collection();
        
        var that = nameSpace.classDefinition({
            name: name,
            extend: extend || 'Ext.data.Model',
            onDefine: function (classDef) {
                if (dataTypeCollection && dataTypeCollection.length > 0) {
                    nameSpace.each(dataTypeCollection, function _makeCswField(dataType) {
                        if (dataType instanceof nameSpace.instanceOf.DataType) {
                            extFieldsCollection.add(dataType);
                        } else {
                            if (dataType && dataType[0]) {
                                var cswDataType = nameSpace.dataTypes.type(dataType[0], dataType[1], dataType[2]);
                                extFieldsCollection.add(cswDataType);
                            }
                        }
                    });
                }
                classDef.fields = extFieldsCollection.value;
                delete classDef.initComponent;
            }
        });

        return that;
    };

    nameSpace.instanceOf.lift('Model', Model);

    /**
     * Create a model object.
     * @param modelDef.name {String} The ClassName of the model to associate with ExtJS
     * @param modelDef.extend {String} [extend='Ext.model.Model'] An ExtJs class name to extend, usually the Model model
     * @param modelDef.dataTypeCollection {nameSpace.dataTypes.collection} [fields=new Array()] An array of fields to load with the model. Fields can be a nameSpace.dataTypes.collection or an array (e.g. ['name', 'string', 'Bob'])
     * @returns {Csw.models.model} A model object. Exposese listeners and columns collections. Self-initializes.
    */
    nameSpace.models.lift('model', function (modelDef) {
        if (!(modelDef)) {
            throw new Error('Cannot instance a model without properties');
        }
        if (!(modelDef.name)) {
            throw new Error('Cannot instance a model without a classname');
        }
        
        var model = new Model(modelDef.name, modelDef.extend, modelDef.dataTypeCollection);
        return model.init();
    });


}(window.$om$));