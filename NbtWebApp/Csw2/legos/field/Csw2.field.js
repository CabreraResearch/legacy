/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _fieldIIFE() {

     /**
      * The private constructor for a Field object.
      * @param defaultValue {String} [defaultValue] A default value
     */
      var Field = function (name, type, defaultValue) {
          var that = this;
          Csw2.property(that, 'type', type || 'string');
          Csw2.property(that, 'name', name);
         
          if(defaultValue) {
              Csw2.property(that, 'defaultValue', defaultValue);
          }
          return that;
      };

      Csw2.instanceOf.lift('Field', Field);

     /**
      * Create a new field
      * @param namme {String} A unique name for this field
      * @param type {String} [type='string'] The display type of this field
      * @param defaultValue {String} [defaultValue] A default value
     */
      Csw2.fields.lift('field', function (name, type, defaultValue){
          if (!name) {
              throw new Error('Cannot create a field without a name');
          }
          var ret = new Field(name, type, defaultValue);
          return ret;
      });


      }());