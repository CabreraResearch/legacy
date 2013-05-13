/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _fieldsIIFE() {

    //Csw2.dependsOn(['Csw2.models.field'], function () {

        /**
         * Defines a collection of fields
         */
        var Fields = function() {
            var that = this;
            Object.defineProperties(that, {
                /**
                 * Get the value of the fields collection
                */
                value: {
                    value: [],
                    writable: true,
                    configurable: true,
                    enumerable: true
                },
                /**
                 * Add a validated field to the collection
                */
                add: {
                    value: function (field) {
                        if (!(field instanceof Csw2.instanceof.Field)) {
                            throw new Error('Only fields can be added to the Fields collection');
                        }
                        that.value.push(field);
                        return that;
                    }
                }
            });
            return that;
        };

        Csw2.instanceof.lift('Fields', Fields);

        /**
         * A mechanism for generating fields
         */
        Csw2.grids.fields.lift('fields', function() {
            var ret = new Fields();
            return ret;
        });

    //});

}());