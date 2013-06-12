/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _modelIIFE() {


    /**
     * Define the properties which are available to Model.
    */
    var modelProperties = Object.create(null);
    modelProperties.fields = 'fields';
    Csw2.constant(Csw2.models, 'properties', modelProperties);

    /**
     * Private class representing the construnction of a model. It returns a Csw2.model.model instance with collections for adding columns and listeners.
     * @internal
     * @constructor
     * @param name {String} The ClassName of the model to associate with ExtJS
     * @param extend {String} [extend='Ext.model.Model'] An ExtJs class name to extend, usually the Model model
     * @param .dataTypeCollection {Csw2.dataTypes.collection} [fields=new Array()] An array of fields to load with the model. Fields can be a Csw2.dataTypes.collection or an array (e.g. ['name', 'string', 'Bob'])
    */
    var Model = function (name, extend, dataTypeCollection) {
        var extFieldsCollection = Csw2.dataTypes.collection();
        
        var that = window.Csw2.classDefinition({
            name: name,
            extend: extend || 'Ext.data.Model',
            onDefine: function (classDef) {
                if (dataTypeCollection && dataTypeCollection.length > 0) {
                    Csw2.each(dataTypeCollection, function _makeCswField(dataType) {
                        if (dataType instanceof Csw2.instanceOf.DataType) {
                            extFieldsCollection.add(dataType);
                        } else {
                            if (dataType && dataType[0]) {
                                var cswDataType = Csw2.dataTypes.type(dataType[0], dataType[1], dataType[2]);
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

    Csw2.instanceOf.lift('Model', Model);

    /**
     * Create a model object.
     * @param modelDef.name {String} The ClassName of the model to associate with ExtJS
     * @param modelDef.extend {String} [extend='Ext.model.Model'] An ExtJs class name to extend, usually the Model model
     * @param modelDef.dataTypeCollection {Csw2.dataTypes.collection} [fields=new Array()] An array of fields to load with the model. Fields can be a Csw2.dataTypes.collection or an array (e.g. ['name', 'string', 'Bob'])
     * @returns {Csw.models.model} A model object. Exposese listeners and columns collections. Self-initializes.
    */
    Csw2.models.lift('model', function (modelDef) {
        if (!(modelDef)) {
            throw new Error('Cannot instance a model without properties');
        }
        if (!(modelDef.name)) {
            throw new Error('Cannot instance a model without a classname');
        }
        
        var model = new Model(modelDef.name, modelDef.extend, modelDef.dataTypeCollection);
        return model.init();
    });


}());