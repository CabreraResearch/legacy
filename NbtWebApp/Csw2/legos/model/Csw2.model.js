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
     * @param name {String} The ClassName of the model to associate with ExtJS
     * @param extend {String} [extend='Ext.model.Model'] An ExtJs class name to extend, usually the Model model
     * @param fields {Array} [fields=new Array()] An array of fields to load with the model
    */
    var Model = function(name, extend, fields) {
        var that = window.Csw2.classDefinition({
            name: name,
            extend: extend || 'Ext.data.Model',
            onDefine: function (def) {
                def.fields = fields;
            }
        });

        return that;
    };

    Csw2.instanceOf.lift('Model', Model);

    /**
     * Create a model object.
     * @param modelDef.name {String} The ClassName of the model to associate with ExtJS
     * @param modelDef.extend {String} [extend='Ext.model.Model'] An ExtJs class name to extend, usually the Model model
     * @param modelDef.fields {Array} [fields=new Array()] An array of fields to load with the model. Fields can be a Csw2 field or an array (e.g. ['name', 'string', 'Bob'])
     * @returns {Csw.models.model} A model object. Exposese listeners and columns collections. Call init when ready to construct the model. 
    */
    Csw2.models.lift('model', function(modelDef) {
        if(!(modelDef)) {
            throw new Error('Cannot instance a model without properties');
        }
        if (!(modelDef.name)) {
            throw new Error('Cannot instance a model without a classname');
        }
        var fields = Csw2.fields.fields();
        if (modelDef.fields && modelDef.fields.length > 0) {
            Csw2.each(modelDef.fields, function _makeCswField(field) {
                if (field instanceof Csw2.instanceOf.Field) {
                    fields.add(field);
                } else {
                    if (field && field[0]) {
                        var cswField = Csw2.fields.field(field[0], field[1], field[2]);
                        fields.add(cswField);
                    }
                }
            });
        }
        var model = new Model(modelDef.name, modelDef.extend, fields.value);
        return model.init();
    });


}());