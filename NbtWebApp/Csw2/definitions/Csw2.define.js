/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _defineIIFE() {

    /**
     * Define declares a new class on the ExtJs namespace
     * @param name {String} The name of this class
     * @param props {Csw2.classDefinition} An instance of a definiton object to augment this class.
    */
    Csw2.lift('define', function _OjDefine(name, props) {
        if(!(props instanceof Csw2.instanceof.ClassDefinition)) {
            throw new Error('Cannot define a class without a valid definition');
        }
        if(!(typeof name === 'string')) {
            throw new Error('Cannot define a class without a name');
        }
        var ret = Ext.define(name, props);
        return ret;

    });

}());