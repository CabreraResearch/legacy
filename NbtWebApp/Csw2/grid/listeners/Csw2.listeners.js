(function _listenerIIFE(){

     /**
      * The private constructor for a Listeners object.
     */
      var GridListeners = function () {
          var that = this;
          var listeners = [];
          Object.defineProperties(that, {
              add: {
                  /**
                   * For a known listener name, apply the appropriate arguments as defined by Ext to a method wrapper to be assigned as the listener.
                   * @param name {Csw2.constants.gridListeners} Name of the listener
                   * @param method {Function} callback method
                  */
                  value: function(name, method) {
                      if (!(Csw2.constants.gridListeners.has(name))) {
                          throw new Error('Grid listener type ' + name + ' is not supported.');
                      }
                      if (-1 !== listeners.indexOf(name)) {
                          throw new Error('Grid already containts a listenere for ' + name + '.');
                      }
                      listeners.push(name);
                      
                      var listener = Csw2.grids.listeners[name](method);

                      Object.defineProperty(that, name, {
                          value: listener,
                          writable: true,
                          configurable: true,
                          enumerable: true
                      });

                      return that;
                  }
              }
          });
          return that;
      };

      Csw2.instanceof.lift('GridListeners', GridListeners);

     /**
      * Create a new listeners collection. This returns a listeners object with an add method.
     */
      Csw2.grids.listeners.lift('listeners', function (){
          var ret = new GridListeners();
          return ret;
      });


      }());