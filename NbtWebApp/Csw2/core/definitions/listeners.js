/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _listenerIIFE(nameSpace) {

     /**
      * The private constructor for a Listeners object.
      * @param listenerType {String} The name of the listener to create
      * @param namespace {String} The NameSpace to which the listener belongs
     */
      var Listeners = function (listenerType, namespace) {
          if (!(nameSpace[namespace])) {
              throw new Error('No listener class "' + namespace + '" has been defined.');
          }
          if (!(nameSpace[namespace].constants.listeners)) {
              throw new Error('No listeners have been defined.');
          }
          
          var that = this;
          var listeners = [];
          nameSpace.property(that, 'add',
              /**
                   * For a known listener name, apply the appropriate arguments as defined by Ext to a method wrapper to be assigned as the listener.
                   * @param name {nameSpace.constants[listenerType]} Name of the listener
                   * @param method {Function} callback method
                  */
              function(name, method) {
                  if (!(nameSpace[namespace].constants.listeners.has(name))) {
                      throw new Error('ListenerType type ' + name + ' is not supported.');
                  }
                  if (-1 !== listeners.indexOf(name)) {
                      throw new Error(namespace + ' already containts a listenere for ' + name + '.');
                  }
                  listeners.push(name);

                  var listener = nameSpace[namespace].listeners[name](method);

                  nameSpace.property(that, name, listener);

                  return that;

              }, false, false, false);
      
          return that;
      };

      nameSpace.instanceOf.lift('Listeners', Listeners);

     /**
      * Create a new listeners collection. This returns a listeners object with an add method.
      * @param listenerType {String} The name of the listener to create
      * @param namespace {String} The NameSpace to which the listener belongs
     */
      nameSpace.lift('makeListeners', function (listenerType, namespace) {
          var ret = new Listeners(listenerType, namespace);
          return ret;
      });


}(window.$om$));